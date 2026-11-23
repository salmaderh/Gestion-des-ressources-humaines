using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GestionRH.Modeles;

namespace GestionRH.Vues
{
    /// <summary>Fenêtre principale : menu latéral + zone d'affichage des modules.</summary>
    public class FormPrincipal : Form
    {
        private readonly Utilisateur utilisateur;
        private readonly Panel contenu;
        private readonly FlowLayoutPanel boutonsMenu;
        private readonly Dictionary<string, ModuleBase> modules = new Dictionary<string, ModuleBase>();
        private Button boutonActif;

        /// <summary>Vrai si l'utilisateur a cliqué sur « Déconnexion » (retour à l'écran de connexion).</summary>
        public bool Deconnexion { get; private set; }

        public FormPrincipal(Utilisateur u)
        {
            utilisateur = u;
            Text = "Gestion des Ressources Humaines";
            Font = Theme.Police;
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(1150, 720);
            Size = new Size(1300, 800);
            WindowState = FormWindowState.Maximized;

            contenu = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Fond };

            Panel menu = new Panel { Dock = DockStyle.Left, Width = 230, BackColor = Theme.Menu };
            Label logo = new Label
            {
                Text = "GRH", Dock = DockStyle.Top, Height = 80, ForeColor = Color.White,
                Font = new Font("Segoe UI", 24F, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter
            };
            Label session = new Label
            {
                Text = u.NomAffiche + Environment.NewLine + u.Role.Libelle(), Dock = DockStyle.Bottom, Height = 60,
                ForeColor = Color.Gainsboro, TextAlign = ContentAlignment.MiddleCenter
            };
            boutonsMenu = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false };
            menu.Controls.Add(boutonsMenu);
            menu.Controls.Add(session);
            menu.Controls.Add(logo);

            Controls.Add(contenu);
            Controls.Add(menu);

            Func<ModuleBase> fabriqueAccueil = () => new TableauDeBordControl();
            Button accueil = AjouterEntree("Tableau de bord", fabriqueAccueil);
            AjouterEntree("Employés", () => new EmployesControl());
            AjouterEntree("Départements", () => new DepartementsControl());
            AjouterEntree("Congés", () => new CongesControl());
            AjouterEntree("Paie", () => new PaieControl());
            AjouterEntree("Évaluations", () => new EvaluationsControl(utilisateur.NomAffiche));
            AjouterEntree("Recrutement", () => new RecrutementControl());
            if (utilisateur.EstAdministrateur)
                AjouterEntree("Utilisateurs", () => new UtilisateursControl(utilisateur.Id));

            Button sortie = CreerBoutonMenu("Déconnexion");
            sortie.Margin = new Padding(0, 30, 0, 0);
            sortie.Click += (s, e) => { Deconnexion = true; Close(); };
            boutonsMenu.Controls.Add(sortie);

            Load += (s, e) => Afficher("Tableau de bord", fabriqueAccueil, accueil);
        }

        private Button CreerBoutonMenu(string texte)
        {
            Button b = new Button
            {
                Text = "   " + texte, Width = 230, Height = 46, FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White, BackColor = Theme.Menu, TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 11F), Margin = new Padding(0), Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderSize = 0;
            b.FlatAppearance.MouseOverBackColor = Theme.MenuSurvol;
            return b;
        }

        private Button AjouterEntree(string texte, Func<ModuleBase> fabrique)
        {
            Button b = CreerBoutonMenu(texte);
            b.Click += (s, e) => Afficher(texte, fabrique, b);
            boutonsMenu.Controls.Add(b);
            return b;
        }

        /// <summary>Affiche un module (créé à la première ouverture) et recharge ses données.</summary>
        private void Afficher(string cle, Func<ModuleBase> fabrique, Button bouton)
        {
            ModuleBase module;
            if (!modules.TryGetValue(cle, out module))
            {
                module = fabrique();
                modules[cle] = module;
            }

            if (boutonActif != null) boutonActif.BackColor = Theme.Menu;
            bouton.BackColor = Theme.Principal;
            boutonActif = bouton;

            contenu.Controls.Clear();
            contenu.Controls.Add(module);
            try
            {
                module.Rafraichir();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Impossible de charger les données : " + ex.Message, "Erreur",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
