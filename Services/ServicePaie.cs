using System;
using System.Collections.Generic;
using System.Linq;
using GestionRH.Donnees;
using GestionRH.Modeles;
using Microsoft.EntityFrameworkCore;

namespace GestionRH.Services
{
    /// <summary>
    /// Génération des fiches de paie.
    /// Calcul (simplifié, taux modifiables ci-dessous) :
    ///   brut        = salaire de base + primes + heures sup. − retenue pour congé sans solde
    ///   cotisations = brut × TauxCotisations
    ///   impôt       = (brut − cotisations) × TauxImpot
    ///   net         = brut − cotisations − impôt
    /// </summary>
    public class ServicePaie : ServiceCrud<FichePaie>
    {
        public const decimal TauxCotisations = 0.0918m;   // part salariale des cotisations sociales
        public const decimal TauxImpot = 0.15m;           // taux d'imposition simplifié (à adapter)

        protected override IQueryable<FichePaie> Requete(HrmContext db)
        {
            return db.FichesPaie.Include(f => f.Employe)
                     .OrderByDescending(f => f.Annee).ThenByDescending(f => f.Mois)
                     .ThenBy(f => f.Employe.Nom);
        }

        public List<FichePaie> Lister(int? mois, int? annee)
        {
            using (HrmContext db = NouveauContexte())
            {
                IQueryable<FichePaie> q = Requete(db).AsNoTracking();
                if (mois.HasValue) q = q.Where(f => f.Mois == mois.Value);
                if (annee.HasValue) q = q.Where(f => f.Annee == annee.Value);
                return q.ToList();
            }
        }

        public FichePaie ObtenirAvecEmploye(int id)
        {
            using (HrmContext db = NouveauContexte())
                return db.FichesPaie.Include(f => f.Employe).ThenInclude(e => e.Departement)
                         .AsNoTracking().FirstOrDefault(f => f.Id == id);
        }

        /// <summary>Calcul pur d'une fiche (sans accès à la base).</summary>
        public static FichePaie Calculer(Employe e, int mois, int annee, decimal primes,
                                         decimal heuresSupplementaires, int joursSansSolde)
        {
            DateTime debut = new DateTime(annee, mois, 1);
            DateTime fin = debut.AddMonths(1).AddDays(-1);
            int joursOuvres = DemandeConge.CompterJoursOuvres(debut, fin);

            decimal retenue = joursOuvres == 0 ? 0m
                : Math.Round(e.SalaireBase / joursOuvres * Math.Min(joursSansSolde, joursOuvres), 3);
            decimal brut = Math.Max(0m, e.SalaireBase + primes + heuresSupplementaires - retenue);
            decimal cotisations = Math.Round(brut * TauxCotisations, 3);
            decimal impot = Math.Round((brut - cotisations) * TauxImpot, 3);

            return new FichePaie
            {
                Employe = e, EmployeId = e.Id, Mois = mois, Annee = annee,
                SalaireBase = e.SalaireBase, Primes = primes, HeuresSupplementaires = heuresSupplementaires,
                JoursSansSolde = joursSansSolde, RetenueAbsences = retenue, SalaireBrut = brut,
                CotisationsSociales = cotisations, ImpotRevenu = impot,
                SalaireNet = brut - cotisations - impot, DateGeneration = DateTime.Now
            };
        }

        public FichePaie Generer(int employeId, int mois, int annee, decimal primes, decimal heuresSupplementaires)
        {
            Exiger(mois >= 1 && mois <= 12, "Mois invalide.");
            Exiger(primes >= 0 && heuresSupplementaires >= 0, "Les montants ne peuvent pas être négatifs.");
            using (HrmContext db = NouveauContexte())
            {
                Employe e = db.Employes.Find(employeId);
                Exiger(e != null, "Choisissez un employé.");
                Exiger(!db.FichesPaie.Any(f => f.EmployeId == employeId && f.Mois == mois && f.Annee == annee),
                       "Une fiche existe déjà pour " + e.NomComplet + " en " + mois.ToString("00") + "/" + annee + ".");
                FichePaie fiche = CreerFiche(db, e, mois, annee, primes, heuresSupplementaires);
                db.FichesPaie.Add(fiche);
                db.SaveChanges();
                return fiche;
            }
        }

        /// <summary>Génère la paie du mois pour tous les employés actifs qui n'ont pas encore de fiche.</summary>
        public int GenererPourTous(int mois, int annee)
        {
            Exiger(mois >= 1 && mois <= 12, "Mois invalide.");
            using (HrmContext db = NouveauContexte())
            {
                HashSet<int> dejaPayes = new HashSet<int>(
                    db.FichesPaie.Where(f => f.Mois == mois && f.Annee == annee).Select(f => f.EmployeId));
                List<Employe> employes = db.Employes.Where(e => e.Statut == StatutEmploye.Actif).ToList();
                int nombre = 0;
                foreach (Employe e in employes)
                {
                    if (dejaPayes.Contains(e.Id)) continue;
                    db.FichesPaie.Add(CreerFiche(db, e, mois, annee, 0m, 0m));
                    nombre++;
                }
                db.SaveChanges();
                return nombre;
            }
        }

        private static FichePaie CreerFiche(HrmContext db, Employe e, int mois, int annee,
                                            decimal primes, decimal heuresSup)
        {
            DateTime debut = new DateTime(annee, mois, 1);
            DateTime fin = debut.AddMonths(1).AddDays(-1);
            int joursSansSolde = ServiceConges.JoursSansSolde(db, e.Id, debut, fin);
            return Calculer(e, mois, annee, primes, heuresSup, joursSansSolde);
        }

        public decimal MasseSalariale(int mois, int annee)
        {
            using (HrmContext db = NouveauContexte())
                return db.FichesPaie.Where(f => f.Mois == mois && f.Annee == annee)
                         .Select(f => f.SalaireNet).ToList().Sum();
        }
    }
}
