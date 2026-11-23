using System;
using System.Linq;
using System.Security.Cryptography;
using GestionRH.Donnees;
using GestionRH.Modeles;

namespace GestionRH.Services
{
    /// <summary>Connexion et gestion des comptes (mot de passe haché avec PBKDF2 + sel aléatoire).</summary>
    public class ServiceAuthentification : ServiceCrud<Utilisateur>
    {
        private const int Iterations = 100000;

        protected override IQueryable<Utilisateur> Requete(HrmContext db)
        {
            return db.Utilisateurs.OrderBy(u => u.NomUtilisateur);
        }

        /// <summary>Retourne l'utilisateur si les identifiants sont corrects, sinon null.</summary>
        public Utilisateur Connecter(string nomUtilisateur, string motDePasse)
        {
            using (HrmContext db = NouveauContexte())
            {
                Utilisateur u = db.Utilisateurs.FirstOrDefault(x => x.NomUtilisateur == nomUtilisateur);
                if (u == null || u.MotDePasseHash != Hacher(motDePasse, u.Sel)) return null;
                return u;
            }
        }

        public static Utilisateur CreerCompte(string nomUtilisateur, string motDePasse, string nomAffiche, RoleUtilisateur role)
        {
            Utilisateur u = new Utilisateur { NomUtilisateur = nomUtilisateur, NomAffiche = nomAffiche, Role = role };
            DefinirMotDePasse(u, motDePasse);
            return u;
        }

        public static void DefinirMotDePasse(Utilisateur u, string motDePasse)
        {
            Exiger(!string.IsNullOrWhiteSpace(motDePasse) && motDePasse.Length >= 6,
                   "Le mot de passe doit contenir au moins 6 caractères.");
            u.Sel = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));
            u.MotDePasseHash = Hacher(motDePasse, u.Sel);
        }

        public static string Hacher(string motDePasse, string sel)
        {
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(motDePasse ?? "", Convert.FromBase64String(sel),
                                                    Iterations, HashAlgorithmName.SHA256, 32);
            return Convert.ToBase64String(hash);
        }

        protected override void Valider(Utilisateur u, HrmContext db)
        {
            Exiger(!string.IsNullOrWhiteSpace(u.NomUtilisateur), "Le nom d'utilisateur est obligatoire.");
            Exiger(!string.IsNullOrWhiteSpace(u.NomAffiche), "Le nom affiché est obligatoire.");
            Exiger(!string.IsNullOrEmpty(u.MotDePasseHash), "Le mot de passe est obligatoire.");
            Exiger(!db.Utilisateurs.Any(x => x.NomUtilisateur == u.NomUtilisateur && x.Id != u.Id),
                   "Ce nom d'utilisateur existe déjà.");
        }

        protected override void AvantSuppression(Utilisateur u, HrmContext db)
        {
            Exiger(!(u.Role == RoleUtilisateur.Administrateur &&
                     db.Utilisateurs.Count(x => x.Role == RoleUtilisateur.Administrateur) <= 1),
                   "Impossible de supprimer le dernier administrateur.");
        }
    }
}
