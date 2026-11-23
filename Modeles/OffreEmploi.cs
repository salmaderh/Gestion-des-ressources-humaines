using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GestionRH.Modeles
{
    public class OffreEmploi : IEntite
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Titre { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [MaxLength(30)]
        public string TypeContrat { get; set; } = "CDI";

        public int? DepartementId { get; set; }
        public Departement Departement { get; set; }

        public DateTime DatePublication { get; set; } = DateTime.Today;
        public DateTime DateLimite { get; set; } = DateTime.Today.AddMonths(1);
        public StatutOffre Statut { get; set; } = StatutOffre.Ouverte;

        public List<Candidature> Candidatures { get; set; } = new List<Candidature>();

        public override string ToString() => Titre;
    }
}
