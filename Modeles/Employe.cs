using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionRH.Modeles
{
    public class Employe : IEntite
    {
        public int Id { get; set; }

        [Required, MaxLength(20)]
        public string Matricule { get; set; }

        [Required, MaxLength(60)]
        public string Nom { get; set; }

        [Required, MaxLength(60)]
        public string Prenom { get; set; }

        [MaxLength(120)]
        public string Email { get; set; }

        [MaxLength(30)]
        public string Telephone { get; set; }

        [MaxLength(80)]
        public string Poste { get; set; }

        public DateTime DateEmbauche { get; set; } = DateTime.Today;

        /// <summary>Salaire de base mensuel (TND).</summary>
        public decimal SalaireBase { get; set; }

        /// <summary>Solde de congés annuels restant, en jours.</summary>
        public double SoldeConges { get; set; } = 24;

        public StatutEmploye Statut { get; set; } = StatutEmploye.Actif;

        public int? DepartementId { get; set; }
        public Departement Departement { get; set; }

        public List<DemandeConge> Conges { get; set; } = new List<DemandeConge>();
        public List<FichePaie> FichesPaie { get; set; } = new List<FichePaie>();
        public List<Evaluation> Evaluations { get; set; } = new List<Evaluation>();

        [NotMapped]
        public string NomComplet => Prenom + " " + Nom;

        public override string ToString() => Matricule + " - " + NomComplet;
    }
}
