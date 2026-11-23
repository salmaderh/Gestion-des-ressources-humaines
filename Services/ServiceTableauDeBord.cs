using System;
using System.Linq;
using GestionRH.Donnees;
using GestionRH.Modeles;

namespace GestionRH.Services
{
    /// <summary>Indicateurs affichés sur le tableau de bord.</summary>
    public class Indicateurs
    {
        public int EmployesActifs { get; set; }
        public int Departements { get; set; }
        public int CongesEnAttente { get; set; }
        public int EnCongeAujourdhui { get; set; }
        public int OffresOuvertes { get; set; }
        public int CandidaturesEnCours { get; set; }
        public decimal MasseSalarialeMoisPrecedent { get; set; }
        public double NoteMoyenne { get; set; }
    }

    public class ServiceTableauDeBord : ServiceBase
    {
        public Indicateurs Calculer()
        {
            DateTime aujourdhui = DateTime.Today;
            DateTime moisPrecedent = aujourdhui.AddMonths(-1);
            using (HrmContext db = NouveauContexte())
            {
                var notes = db.Evaluations
                              .Select(v => new { v.NoteCompetences, v.NoteObjectifs, v.NoteComportement })
                              .ToList();
                return new Indicateurs
                {
                    EmployesActifs = db.Employes.Count(e => e.Statut == StatutEmploye.Actif),
                    Departements = db.Departements.Count(),
                    CongesEnAttente = db.DemandesConge.Count(c => c.Statut == StatutDemande.EnAttente),
                    EnCongeAujourdhui = db.DemandesConge.Count(c => c.Statut == StatutDemande.Approuvee &&
                                                                    c.DateDebut <= aujourdhui && c.DateFin >= aujourdhui),
                    OffresOuvertes = db.OffresEmploi.Count(o => o.Statut == StatutOffre.Ouverte),
                    CandidaturesEnCours = db.Candidatures.Count(c => c.Statut == StatutCandidature.Recue ||
                                                                     c.Statut == StatutCandidature.Entretien),
                    MasseSalarialeMoisPrecedent = db.FichesPaie
                        .Where(f => f.Mois == moisPrecedent.Month && f.Annee == moisPrecedent.Year)
                        .Select(f => f.SalaireNet).ToList().Sum(),
                    NoteMoyenne = notes.Count == 0 ? 0
                        : Math.Round(notes.Average(n => (n.NoteCompetences + n.NoteObjectifs + n.NoteComportement) / 3.0), 2)
                };
            }
        }
    }
}
