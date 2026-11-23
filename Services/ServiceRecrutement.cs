using System;
using System.Collections.Generic;
using System.Linq;
using GestionRH.Donnees;
using GestionRH.Modeles;
using Microsoft.EntityFrameworkCore;

namespace GestionRH.Services
{
    /// <summary>Offres d'emploi.</summary>
    public class ServiceOffres : ServiceCrud<OffreEmploi>
    {
        protected override IQueryable<OffreEmploi> Requete(HrmContext db)
        {
            return db.OffresEmploi.Include(o => o.Departement).Include(o => o.Candidatures)
                     .OrderByDescending(o => o.DatePublication);
        }

        protected override void Valider(OffreEmploi o, HrmContext db)
        {
            Exiger(!string.IsNullOrWhiteSpace(o.Titre), "Le titre de l'offre est obligatoire.");
            Exiger(o.DateLimite.Date >= o.DatePublication.Date, "La date limite doit suivre la date de publication.");
        }
    }

    /// <summary>Candidatures, entretiens et embauche.</summary>
    public class ServiceCandidatures : ServiceCrud<Candidature>
    {
        protected override IQueryable<Candidature> Requete(HrmContext db)
        {
            return db.Candidatures.Include(c => c.OffreEmploi).OrderByDescending(c => c.DateCandidature);
        }

        public List<Candidature> ListerParOffre(int offreId)
        {
            using (HrmContext db = NouveauContexte())
                return Requete(db).AsNoTracking().Where(c => c.OffreEmploiId == offreId).ToList();
        }

        protected override void Valider(Candidature c, HrmContext db)
        {
            Exiger(c.OffreEmploiId > 0, "Offre introuvable.");
            Exiger(!string.IsNullOrWhiteSpace(c.Nom) && !string.IsNullOrWhiteSpace(c.Prenom),
                   "Le nom et le prénom du candidat sont obligatoires.");
            Exiger(!c.NoteEntretien.HasValue || (c.NoteEntretien >= 0 && c.NoteEntretien <= 20),
                   "La note d'entretien doit être comprise entre 0 et 20.");
        }

        public void ChangerStatut(int id, StatutCandidature statut)
        {
            using (HrmContext db = NouveauContexte())
            {
                Candidature c = db.Candidatures.Find(id);
                Exiger(c != null, "Candidature introuvable.");
                Exiger(c.Statut != StatutCandidature.Embauchee, "Ce candidat a déjà été embauché.");
                c.Statut = statut;
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Transforme une candidature en employé : crée l'employé (matricule automatique,
        /// département de l'offre), marque la candidature « embauchée » et l'offre « pourvue ».
        /// </summary>
        public Employe Embaucher(int candidatureId, string poste, decimal salaire, DateTime dateEmbauche)
        {
            Exiger(salaire > 0, "Le salaire doit être positif.");
            using (HrmContext db = NouveauContexte())
            {
                Candidature c = db.Candidatures.Include(x => x.OffreEmploi).FirstOrDefault(x => x.Id == candidatureId);
                Exiger(c != null, "Candidature introuvable.");
                Exiger(c.Statut != StatutCandidature.Embauchee, "Ce candidat a déjà été embauché.");
                Exiger(c.Statut != StatutCandidature.Refusee, "Ce candidat a été refusé.");

                Employe e = new Employe
                {
                    Matricule = ServiceEmployes.ProchainMatricule(db),
                    Nom = c.Nom, Prenom = c.Prenom, Email = c.Email, Telephone = c.Telephone,
                    Poste = string.IsNullOrWhiteSpace(poste) ? c.OffreEmploi.Titre : poste,
                    DepartementId = c.OffreEmploi.DepartementId,
                    SalaireBase = salaire, DateEmbauche = dateEmbauche
                };
                db.Employes.Add(e);
                c.Statut = StatutCandidature.Embauchee;
                c.OffreEmploi.Statut = StatutOffre.Pourvue;
                db.SaveChanges();
                return e;
            }
        }
    }
}
