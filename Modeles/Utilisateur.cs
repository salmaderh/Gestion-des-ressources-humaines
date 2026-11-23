using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionRH.Modeles
{
    /// <summary>Compte de connexion à l'application (mot de passe stocké haché + salé).</summary>
    public class Utilisateur : IEntite
    {
        public int Id { get; set; }

        [Required, MaxLength(40)]
        public string NomUtilisateur { get; set; }

        [Required, MaxLength(80)]
        public string NomAffiche { get; set; }

        [Required]
        public string MotDePasseHash { get; set; }

        [Required]
        public string Sel { get; set; }

        public RoleUtilisateur Role { get; set; } = RoleUtilisateur.GestionnaireRH;

        [NotMapped]
        public bool EstAdministrateur => Role == RoleUtilisateur.Administrateur;
    }
}
