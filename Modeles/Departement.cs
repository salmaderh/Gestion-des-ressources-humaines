using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GestionRH.Modeles
{
    public class Departement : IEntite
    {
        public int Id { get; set; }

        [Required, MaxLength(80)]
        public string Nom { get; set; }

        [MaxLength(250)]
        public string Description { get; set; }

        public List<Employe> Employes { get; set; } = new List<Employe>();
        public List<OffreEmploi> Offres { get; set; } = new List<OffreEmploi>();

        public override string ToString() => Nom;
    }
}
