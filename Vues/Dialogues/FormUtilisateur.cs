using System.Windows.Forms;
using GestionRH.Modeles;
using GestionRH.Services;

namespace GestionRH.Vues
{
    public class FormUtilisateur : FormEditionBase
    {
        private readonly Utilisateur utilisateur;
        private readonly TextBox login, nomAffiche, motDePasse, confirmation;
        private readonly ComboBox role;

        public FormUtilisateur(Utilisateur u) : base(u == null ? "Nouvel utilisateur" : "Modifier l'utilisateur")
        {
            utilisateur = u ?? new Utilisateur();
            login = AjouterChamp("Nom d'utilisateur *", Texte(utilisateur.NomUtilisateur));
            nomAffiche = AjouterChamp("Nom affiché *", Texte(utilisateur.NomAffiche));
            role = AjouterChamp("Rôle", Liste());
            ElementListe.Remplir(role, ElementListe.DepuisEnum<RoleUtilisateur>());
            ElementListe.Selectionner(role, utilisateur.Role);
            motDePasse = AjouterChamp(u == null ? "Mot de passe *" : "Nouveau mot de passe", new TextBox { UseSystemPasswordChar = true });
            confirmation = AjouterChamp("Confirmation", new TextBox { UseSystemPasswordChar = true });
            if (u != null)
                AjouterChamp("", new Label { AutoSize = true, ForeColor = Theme.Gris, Text = "Laisser vide pour conserver le mot de passe actuel." });
            TerminerMiseEnPage();
        }

        protected override void Enregistrer()
        {
            utilisateur.NomUtilisateur = login.Text.Trim();
            utilisateur.NomAffiche = nomAffiche.Text.Trim();
            utilisateur.Role = (RoleUtilisateur)ElementListe.ValeurDe(role);
            if (utilisateur.Id == 0 || motDePasse.Text.Length > 0)
            {
                Exiger(motDePasse.Text == confirmation.Text, "Les deux mots de passe ne correspondent pas.");
                ServiceAuthentification.DefinirMotDePasse(utilisateur, motDePasse.Text);
            }
            new ServiceAuthentification().Enregistrer(utilisateur);
        }
    }
}
