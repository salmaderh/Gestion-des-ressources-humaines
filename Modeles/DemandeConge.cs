using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionRH.Modeles
{
    public class DemandeConge : IEntite
    {
        public int Id { get; set; }

        public int EmployeId { get; set; }
        public Employe Employe { get; set; }

        public TypeConge Type { get; set; } = TypeConge.Annuel;
        public DateTime DateDebut { get; set; } = DateTime.Today;
        public DateTime DateFin { get; set; } = DateTime.Today;

        [MaxLength(250)]
        public string Motif { get; set; }

        public StatutDemande Statut { get; set; } = StatutDemande.EnAttente;
        public DateTime DateDemande { get; set; } = DateTime.Now;

        [MaxLength(250)]
        public string CommentaireDecision { get; set; }

        /// <summary>Nombre de jours ouvrés (lundi à vendredi) couverts par la demande.</summary>
        [NotMapped]
        public int NombreJours => CompterJoursOuvres(DateDebut, DateFin);

        public static int CompterJoursOuvres(DateTime debut, DateTime fin)
        {
            int jours = 0;
            for (DateTime d = debut.Date; d <= fin.Date; d = d.AddDays(1))
                if (d.DayOfWeek != DayOfWeek.Saturday && d.DayOfWeek != DayOfWeek.Sunday)
                    jours++;
            return jours;
        }

        public bool Chevauche(DateTime debut, DateTime fin)
        {
            return DateDebut.Date <= fin.Date && DateFin.Date >= debut.Date;
        }
    }
}
