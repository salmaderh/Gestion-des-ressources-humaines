using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionRH.Modeles
{
    /// <summary>Évaluation périodique : trois critères notés de 1 à 5.</summary>
    public class Evaluation : IEntite
    {
        public int Id { get; set; }

        public int EmployeId { get; set; }
        public Employe Employe { get; set; }

        [Required, MaxLength(20)]
        public string Periode { get; set; }

        public DateTime DateEvaluation { get; set; } = DateTime.Today;

        [MaxLength(80)]
        public string Evaluateur { get; set; }

        public int NoteCompetences { get; set; } = 3;
        public int NoteObjectifs { get; set; } = 3;
        public int NoteComportement { get; set; } = 3;

        [MaxLength(300)]
        public string ObjectifsSuivants { get; set; }

        [MaxLength(500)]
        public string Commentaire { get; set; }

        [NotMapped]
        public double NoteGlobale => Math.Round((NoteCompetences + NoteObjectifs + NoteComportement) / 3.0, 2);

        [NotMapped]
        public string Appreciation
        {
            get
            {
                double n = NoteGlobale;
                if (n >= 4.5) return "Excellent";
                if (n >= 3.5) return "Très bien";
                if (n >= 2.5) return "Satisfaisant";
                if (n >= 1.5) return "À améliorer";
                return "Insuffisant";
            }
        }
    }
}
