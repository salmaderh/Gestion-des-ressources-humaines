using System.Linq;
using GestionRH.Modeles;
using GestionRH.Services;

namespace GestionRH.Vues
{
    /// <summary>Gestion des comptes (réservé aux administrateurs).</summary>
    public class UtilisateursControl : ModuleBase
    {
        private readonly ServiceAuthentification service = new ServiceAuthentification();
        private readonly int idConnecte;

        public UtilisateursControl(int idUtilisateurConnecte) : base("Utilisateurs")
        {
            idConnecte = idUtilisateurConnecte;
            AjouterBouton("Ajouter", Theme.Principal, () => OuvrirDialogue(new FormUtilisateur(null)));
            AjouterBouton("Modifier", Theme.Avertissement, Modifier);
            AjouterBouton("Supprimer", Theme.Danger, Supprimer);
        }

        public override void Rafraichir()
        {
            Afficher(service.Lister().Select(u => new
            {
                u.Id,
                Identifiant = u.NomUtilisateur,
                Nom_affiché = u.NomAffiche,
                Rôle = u.Role.Libelle()
            }));
        }

        protected override void DoubleClicLigne() => Modifier();

        private void Modifier()
        {
            int? id = ExigerSelection();
            if (id != null) OuvrirDialogue(new FormUtilisateur(service.Obtenir(id.Value)));
        }

        private void Supprimer()
        {
            int? id = ExigerSelection();
            if (id == null) return;
            if (id.Value == idConnecte)
            {
                System.Windows.Forms.MessageBox.Show(this, "Vous ne pouvez pas supprimer votre propre compte.");
                return;
            }
            if (Confirmer("Supprimer ce compte utilisateur ?"))
                Executer(() => service.Supprimer(id.Value));
        }
    }
}
