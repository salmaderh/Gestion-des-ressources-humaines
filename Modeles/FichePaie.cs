using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionRH.Modeles
{
    /// <summary>Bulletin de paie mensuel d'un employé. Les montants sont figés au moment de la génération.</summary>
    public class FichePaie : IEntite
    {
        public int Id { get; set; }

        public int EmployeId { get; set; }
        public Employe Employe { get; set; }

        public int Mois { get; set; }
        public int Annee { get; set; }

        public decimal SalaireBase { get; set; }
        public decimal Primes { get; set; }
        public decimal HeuresSupplementaires { get; set; }
        public int JoursSansSolde { get; set; }
        public decimal RetenueAbsences { get; set; }
        public decimal SalaireBrut { get; set; }
        public decimal CotisationsSociales { get; set; }
        public decimal ImpotRevenu { get; set; }
        public decimal SalaireNet { get; set; }
        public DateTime DateGeneration { get; set; } = DateTime.Now;

        [NotMapped]
        public string Periode => Mois.ToString("00") + "/" + Annee;
    }
}
