using System;
using System.Drawing;
using System.Windows.Forms;
using GestionRH.Services;
using Microsoft.EntityFrameworkCore;

namespace GestionRH.Vues
{
    /// <summary>
    /// Fenêtre de saisie générique : une grille « libellé / champ » et les boutons
    /// Enregistrer / Annuler. Les fenêtres concrètes ajoutent leurs champs avec AjouterChamp()
    /// et implémentent Enregistrer() (lecture des champs + appel du service).
    /// </summary>
    public abstract class FormEditionBase : Form
    {
        private readonly TableLayoutPanel champs;
        private int hauteurChamps;
        protected readonly Button BoutonValider;
        protected readonly Button BoutonAnnuler;

        protected FormEditionBase(string titre)
        {
            Text = titre;
            Font = Theme.Police;
            BackColor = Color.White;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            ClientSize = new Size(540, 400);

            champs = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(18, 16, 18, 8), AutoScroll = true
            };
            champs.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170F));
            champs.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            FlowLayoutPanel bas = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom, Height = 56, FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(12, 10, 12, 10), BackColor = Theme.Fond
            };
            BoutonAnnuler = Theme.CreerBouton("Annuler", Theme.Gris);
            BoutonAnnuler.DialogResult = DialogResult.Cancel;
            BoutonValider = Theme.CreerBouton("Enregistrer", Theme.Principal);
            BoutonValider.Click += (s, e) => TenterEnregistrement();
            bas.Controls.Add(BoutonAnnuler);
            bas.Controls.Add(BoutonValider);

            Controls.Add(champs);
            Controls.Add(bas);
            CancelButton = BoutonAnnuler;
        }

        /// <summary>Ajoute une ligne « libellé : contrôle » et renvoie le contrôle.</summary>
        protected T AjouterChamp<T>(string libelle, T controle) where T : Control
        {
            int ligne = champs.RowCount;
            champs.RowCount = ligne + 1;
            champs.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            Label l = new Label { Text = libelle, AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 8, 8, 4) };
            controle.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            controle.Margin = new Padding(0, 4, 0, 4);
            champs.Controls.Add(l, 0, ligne);
            champs.Controls.Add(controle, 1, ligne);
            hauteurChamps += Math.Max(controle.Height, 24) + 10;
            return controle;
        }

        /// <summary>Ajuste la hauteur de la fenêtre au nombre de champs (à appeler en fin de constructeur).</summary>
        protected void TerminerMiseEnPage()
        {
            int hauteur = hauteurChamps + 24 + 56 + 10;   // champs + marges + barre de boutons
            ClientSize = new Size(ClientSize.Width, Math.Max(200, Math.Min(720, hauteur)));
        }

        // ---------- Fabriques de champs ----------

        protected static TextBox Texte(string valeur = "", bool multiligne = false)
        {
            TextBox t = new TextBox { Text = valeur ?? "" };
            if (multiligne) { t.Multiline = true; t.Height = 80; t.ScrollBars = ScrollBars.Vertical; }
            return t;
        }

        protected static NumericUpDown Nombre(decimal valeur, decimal min, decimal max, int decimales = 0)
        {
            NumericUpDown n = new NumericUpDown
            {
                Minimum = min, Maximum = max, DecimalPlaces = decimales, ThousandsSeparator = true,
                Increment = 1
            };
            n.Value = Math.Min(max, Math.Max(min, valeur));
            return n;
        }

        protected static DateTimePicker Date(DateTime valeur)
        {
            return new DateTimePicker { Format = DateTimePickerFormat.Short, Value = valeur };
        }

        protected static ComboBox Liste()
        {
            return new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        }

        protected static void Exiger(bool condition, string message)
        {
            if (!condition) throw new RegleMetierException(message);
        }

        /// <summary>Lit les champs et enregistre via le service. Lever RegleMetierException en cas d'erreur.</summary>
        protected abstract void Enregistrer();

        private void TenterEnregistrement()
        {
            try
            {
                Enregistrer();
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (RegleMetierException ex)
            {
                MessageBox.Show(this, ex.Message, "Vérification", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
        }
    }
}
