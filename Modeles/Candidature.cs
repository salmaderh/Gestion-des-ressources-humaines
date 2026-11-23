using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionRH.Modeles
{
    public class Candidature : IEntite
    {
        public int Id { get; set; }

        public int OffreEmploiId { get; set; }
        public OffreEmploi OffreEmploi { get; set; }

        [Required, MaxLength(60)]
        public string Nom { get; set; }

        [Required, MaxLength(60)]
        public string Prenom { get; set; }

        [MaxLength(120)]
        public string Email { get; set; }

        [MaxLength(30)]
        public string Telephone { get; set; }

        public DateTime DateCandidature { get; set; } = DateTime.Today;
        public StatutCandidature Statut { get; set; } = StatutCandidature.Recue;

        /// <summary>Note d'entretien sur 20 (vide tant qu'il n'y a pas eu d'entretien).</summary>
        public int? NoteEntretien { get; set; }

        [MaxLength(500)]
        public string Commentaire { get; set; }

        [NotMapped]
        public string NomComplet => Prenom + " " + Nom;
    }
}
