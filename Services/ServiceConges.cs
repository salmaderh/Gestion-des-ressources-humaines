using System;
using System.Collections.Generic;
using System.Linq;
using GestionRH.Donnees;
using GestionRH.Modeles;
using Microsoft.EntityFrameworkCore;

namespace GestionRH.Services
{
    /// <summary>
    /// Demandes de congé. Règles : dates cohérentes, pas de chevauchement avec une autre demande
    /// non refusée, solde suffisant pour un congé annuel (débité à l'approbation).
    /// </summary>
    public class ServiceConges : ServiceCrud<DemandeConge>
    {
        protected override IQueryable<DemandeConge> Requete(HrmContext db)
        {
            return db.DemandesConge.Include(c => c.Employe).OrderByDescending(c => c.DateDemande);
        }

        public List<DemandeConge> Lister(StatutDemande? statut)
        {
            using (HrmContext db = NouveauContexte())
            {
                IQueryable<DemandeConge> q = Requete(db).AsNoTracking();
                if (statut.HasValue) q = q.Where(c => c.Statut == statut.Value);
                return q.ToList();
            }
        }

        protected override void Valider(DemandeConge c, HrmContext db)
        {
            Exiger(c.EmployeId > 0, "Choisissez un employé.");
            Exiger(c.DateFin.Date >= c.DateDebut.Date, "La date de fin doit être après la date de début.");
            Exiger(c.NombreJours > 0, "La période ne contient aucun jour ouvré.");

            if (c.Id != 0)
            {
                StatutDemande statutActuel = db.DemandesConge.Where(x => x.Id == c.Id)
                                                             .Select(x => x.Statut).FirstOrDefault();
                Exiger(statutActuel == StatutDemande.EnAttente, "Seule une demande en attente peut être modifiée.");
            }

            bool chevauchement = db.DemandesConge.Any(x => x.EmployeId == c.EmployeId && x.Id != c.Id &&
                                                           x.Statut != StatutDemande.Refusee &&
                                                           x.DateDebut <= c.DateFin && x.DateFin >= c.DateDebut);
            Exiger(!chevauchement, "Cette période chevauche une autre demande de l'employé.");

            if (c.Type == TypeConge.Annuel)
            {
                Employe e = db.Employes.Find(c.EmployeId);
                Exiger(e != null, "Employé introuvable.");
                Exiger(c.NombreJours <= e.SoldeConges,
                       "Solde insuffisant : " + e.SoldeConges + " jour(s) disponible(s), " + c.NombreJours + " demandé(s).");
            }
        }

        public void Approuver(int id, string commentaire)
        {
            using (HrmContext db = NouveauContexte())
            {
                DemandeConge c = db.DemandesConge.Include(x => x.Employe).FirstOrDefault(x => x.Id == id);
                Exiger(c != null, "Demande introuvable.");
                Exiger(c.Statut == StatutDemande.EnAttente, "Cette demande a déjà été traitée.");
                if (c.Type == TypeConge.Annuel)
                {
                    Exiger(c.NombreJours <= c.Employe.SoldeConges, "Solde de congés insuffisant.");
                    c.Employe.SoldeConges -= c.NombreJours;
                }
                c.Statut = StatutDemande.Approuvee;
                c.CommentaireDecision = commentaire;
                db.SaveChanges();
            }
        }

        public void Refuser(int id, string commentaire)
        {
            using (HrmContext db = NouveauContexte())
            {
                DemandeConge c = db.DemandesConge.Find(id);
                Exiger(c != null, "Demande introuvable.");
                Exiger(c.Statut == StatutDemande.EnAttente, "Cette demande a déjà été traitée.");
                c.Statut = StatutDemande.Refusee;
                c.CommentaireDecision = commentaire;
                db.SaveChanges();
            }
        }

        protected override void AvantSuppression(DemandeConge c, HrmContext db)
        {
            // Supprimer un congé annuel approuvé rend les jours à l'employé
            if (c.Statut == StatutDemande.Approuvee && c.Type == TypeConge.Annuel)
            {
                Employe e = db.Employes.Find(c.EmployeId);
                if (e != null) e.SoldeConges += c.NombreJours;
            }
        }

        /// <summary>Jours ouvrés de congé sans solde approuvés entre deux dates (utilisé par la paie).</summary>
        public static int JoursSansSolde(HrmContext db, int employeId, DateTime debut, DateTime fin)
        {
            List<DemandeConge> demandes = db.DemandesConge
                .Where(c => c.EmployeId == employeId && c.Type == TypeConge.SansSolde &&
                            c.Statut == StatutDemande.Approuvee && c.DateDebut <= fin && c.DateFin >= debut)
                .ToList();
            int jours = 0;
            foreach (DemandeConge c in demandes)
            {
                DateTime d = c.DateDebut > debut ? c.DateDebut : debut;
                DateTime f = c.DateFin < fin ? c.DateFin : fin;
                jours += DemandeConge.CompterJoursOuvres(d, f);
            }
            return jours;
        }

        /// <summary>Employés en congé approuvé à une date donnée.</summary>
        public List<DemandeConge> EnCongeLe(DateTime date)
        {
            using (HrmContext db = NouveauContexte())
                return Requete(db).AsNoTracking()
                                  .Where(c => c.Statut == StatutDemande.Approuvee &&
                                              c.DateDebut <= date && c.DateFin >= date)
                                  .ToList();
        }
    }
}
