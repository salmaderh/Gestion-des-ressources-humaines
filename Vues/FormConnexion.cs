using System.Drawing;
using System.Windows.Forms;
using GestionRH.Modeles;
using GestionRH.Services;

namespace GestionRH.Vues
{
    /// <summary>Fenêtre de connexion.</summary>
    public class FormConnexion : Form
    {
        private readonly TextBox identifiant;
        private readonly TextBox motDePasse;
        private readonly Label erreur;

        public Utilisateur UtilisateurConnecte { get; private set; }

        public FormConnexion()
        {
            Text = "Connexion — Gestion RH";
            Font = Theme.Police;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(420, 400);
            BackColor = Color.White;

            Panel entete = new Panel { Dock = DockStyle.Top, Height = 110, BackColor = Theme.Menu };
            entete.Controls.Add(new Label
            {
                Text = "Gestion des Ressources Humaines", ForeColor = Color.White,
                Font = new Font("Segoe UI", 15F, FontStyle.Bold), Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            });

            Label lblId = new Label { Text = "Nom d'utilisateur", Location = new Point(50, 135), AutoSize = true };
            identifiant = new TextBox { Location = new Point(50, 160), Width = 320 };
            Label lblMdp = new Label { Text = "Mot de passe", Location = new Point(50, 200), AutoSize = true };
            motDePasse = new TextBox { Location = new Point(50, 225), Width = 320, UseSystemPasswordChar = true };
            erreur = new Label { Location = new Point(50, 258), Width = 320, Height = 22, ForeColor = Theme.Danger };

            Button connecter = Theme.CreerBouton("Se connecter", Theme.Principal);
            connecter.AutoSize = false;
            connecter.Location = new Point(50, 285);
            connecter.Size = new Size(320, 40);
            connecter.Click += (s, e) => Connecter();

            Label aide = new Label
            {
                Text = "Comptes de démonstration : admin / admin123  —  rh / rh1234",
                Location = new Point(50, 345), Width = 320, Height = 36, ForeColor = Theme.Gris
            };

            Controls.AddRange(new Control[] { entete, lblId, identifiant, lblMdp, motDePasse, erreur, connecter, aide });
            AcceptButton = connecter;
        }

        private void Connecter()
        {
            Utilisateur u = new ServiceAuthentification().Connecter(identifiant.Text.Trim(), motDePasse.Text);
            if (u == null)
            {
                erreur.Text = "Identifiants incorrects.";
                motDePasse.SelectAll();
                motDePasse.Focus();
                return;
            }
            UtilisateurConnecte = u;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
