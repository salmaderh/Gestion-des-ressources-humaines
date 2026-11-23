using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GestionRH.Services;
using Microsoft.EntityFrameworkCore;

namespace GestionRH.Vues
{
    /// <summary>
    /// Écran générique d'un module : un titre, une barre d'outils (boutons, filtres) et une grille.
    /// Les modules concrets (Employés, Congés, Paie...) héritent de cette classe et
    /// implémentent Rafraichir().
    /// </summary>
    public abstract class ModuleBase : UserControl
    {
        protected readonly DataGridView Grille;
        protected readonly FlowLayoutPanel BarreOutils;
        protected readonly Label Titre;

        protected ModuleBase(string titre)
        {
            Dock = DockStyle.Fill;
            BackColor = Theme.Fond;
            Padding = new Padding(20, 12, 20, 20);
            Font = Theme.Police;

            Grille = new DataGridView { Dock = DockStyle.Fill };
            Theme.StyliserGrille(Grille);
            Grille.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0) DoubleClicLigne(); };

            BarreOutils = new FlowLayoutPanel
            {
                Dock = DockStyle.Top, Height = 50, WrapContents = false,
                Padding = new Padding(0, 6, 0, 8), AutoScroll = true
            };
            Titre = new Label
            {
                Text = titre, Dock = DockStyle.Top, Height = 48,
                Font = Theme.PoliceTitre, ForeColor = Theme.TexteFonce
            };

            // Ordre d'ajout : le contrôle « Fill » d'abord, puis les contrôles « Top »
            Controls.Add(Grille);
            Controls.Add(BarreOutils);
            Controls.Add(Titre);
        }

        /// <summary>Vrai pendant le rechargement d'une liste de filtre (évite les rafraîchissements en boucle).</summary>
        protected bool EnChargement { get; private set; }

        /// <summary>Recharge les données depuis la base.</summary>
        public abstract void Rafraichir();

        /// <summary>Action du double-clic sur une ligne (par défaut : rien).</summary>
        protected virtual void DoubleClicLigne() { }

        protected Button AjouterBouton(string texte, Color couleur, Action action)
        {
            Button b = Theme.CreerBouton(texte, couleur);
            b.Click += (s, e) => action();
            BarreOutils.Controls.Add(b);
            return b;
        }

        protected void AjouterFiltre(string libelle, Control controle)
        {
            Label l = new Label { Text = libelle, AutoSize = true, Margin = new Padding(16, 9, 4, 0) };
            controle.Margin = new Padding(0, 4, 0, 0);
            BarreOutils.Controls.Add(l);
            BarreOutils.Controls.Add(controle);
        }

        /// <summary>Crée une liste déroulante de filtre qui rafraîchit l'écran quand on change la sélection.</summary>
        protected ComboBox AjouterFiltreListe(string libelle, int largeur = 200)
        {
            ComboBox c = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = largeur };
            c.SelectedIndexChanged += (s, e) => { if (!EnChargement) Rafraichir(); };
            AjouterFiltre(libelle, c);
            return c;
        }

        /// <summary>Remplit une liste en conservant la sélection courante.</summary>
        protected void RechargerListe(ComboBox combo, IEnumerable<ElementListe> elements)
        {
            object ancienne = ElementListe.ValeurDe(combo);
            EnChargement = true;
            try
            {
                ElementListe.Remplir(combo, elements);
                ElementListe.Selectionner(combo, ancienne);
            }
            finally
            {
                EnChargement = false;
            }
        }

        /// <summary>Insère un contrôle (cartes, résumé...) entre la barre d'outils et la grille.</summary>
        protected void AjouterSousBarre(Control controle)
        {
            if (controle.Dock == DockStyle.None) controle.Dock = DockStyle.Top;
            Controls.Add(controle);
            Controls.SetChildIndex(controle, 1);
            Controls.SetChildIndex(Grille, 0);
        }

        /// <summary>Affiche des lignes dans la grille ; une colonne « Id » est masquée automatiquement.</summary>
        protected void Afficher<T>(IEnumerable<T> lignes)
        {
            Afficher(Grille, lignes);
        }

        protected static void Afficher<T>(DataGridView grille, IEnumerable<T> lignes)
        {
            grille.DataSource = lignes.ToList();
            foreach (DataGridViewColumn c in grille.Columns)
                c.HeaderText = c.Name.Replace('_', ' ');   // « Demandée_le » -> « Demandée le »
            if (grille.Columns.Contains("Id")) grille.Columns["Id"].Visible = false;
        }

        /// <summary>Re-sélectionne la ligne d'identifiant donné (après un rafraîchissement).</summary>
        protected static void SelectionnerLigne(DataGridView grille, int? id)
        {
            if (id == null || !grille.Columns.Contains("Id")) return;
            foreach (DataGridViewRow r in grille.Rows)
            {
                object v = r.Cells["Id"].Value;
                if (v != null && Convert.ToInt32(v) == id.Value)
                {
                    DataGridViewCell visible = r.Cells.Cast<DataGridViewCell>().FirstOrDefault(c => c.Visible);
                    if (visible != null) grille.CurrentCell = visible;
                    return;
                }
            }
        }

        protected int? IdSelectionne()
        {
            return IdSelectionne(Grille);
        }

        protected static int? IdSelectionne(DataGridView grille)
        {
            if (grille.CurrentRow == null || !grille.Columns.Contains("Id")) return null;
            object v = grille.CurrentRow.Cells["Id"].Value;
            return v == null ? (int?)null : Convert.ToInt32(v);
        }

        /// <summary>Id de la ligne sélectionnée, ou message si aucune ligne n'est sélectionnée.</summary>
        protected int? ExigerSelection(DataGridView grille = null)
        {
            int? id = IdSelectionne(grille ?? Grille);
            if (id == null)
                MessageBox.Show(this, "Veuillez sélectionner une ligne.", "Information",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return id;
        }

        protected bool Confirmer(string message)
        {
            return MessageBox.Show(this, message, "Confirmation", MessageBoxButtons.YesNo,
                                   MessageBoxIcon.Question) == DialogResult.Yes;
        }

        /// <summary>Exécute une action, affiche les erreurs de façon lisible puis rafraîchit l'écran.</summary>
        protected void Executer(Action action)
        {
            try
            {
                action();
            }
            catch (RegleMetierException ex)
            {
                MessageBox.Show(this, ex.Message, "Opération impossible", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show(this, "Erreur base de données : " + (ex.InnerException?.Message ?? ex.Message),
                                "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Rafraichir();
        }

        /// <summary>Ouvre une fenêtre d'édition ; si elle est validée, rafraîchit la grille.</summary>
        protected void OuvrirDialogue(Form dialogue)
        {
            using (dialogue)
            {
                if (dialogue.ShowDialog(this) == DialogResult.OK) Rafraichir();
            }
        }
    }
}
