using System.Drawing;
using System.Windows.Forms;

namespace GestionRH.Vues
{
    /// <summary>Couleurs, polices et styles communs à toutes les fenêtres.</summary>
    public static class Theme
    {
        public static readonly Color Principal = Color.FromArgb(37, 99, 235);
        public static readonly Color Succes = Color.FromArgb(22, 163, 74);
        public static readonly Color Danger = Color.FromArgb(220, 38, 38);
        public static readonly Color Avertissement = Color.FromArgb(217, 119, 6);
        public static readonly Color Gris = Color.FromArgb(100, 116, 139);
        public static readonly Color Menu = Color.FromArgb(30, 41, 59);
        public static readonly Color MenuSurvol = Color.FromArgb(51, 65, 85);
        public static readonly Color Fond = Color.FromArgb(241, 245, 249);
        public static readonly Color TexteFonce = Color.FromArgb(15, 23, 42);

        public static readonly Font Police = new Font("Segoe UI", 10F);
        public static readonly Font PoliceGras = new Font("Segoe UI", 10F, FontStyle.Bold);
        public static readonly Font PoliceTitre = new Font("Segoe UI", 18F, FontStyle.Bold);
        public static readonly Font PoliceChiffre = new Font("Segoe UI", 22F, FontStyle.Bold);

        public static Button CreerBouton(string texte, Color couleur)
        {
            Button b = new Button
            {
                Text = texte,
                AutoSize = true,
                MinimumSize = new Size(110, 34),
                Padding = new Padding(10, 2, 10, 2),
                Margin = new Padding(0, 0, 8, 0),
                FlatStyle = FlatStyle.Flat,
                BackColor = couleur,
                ForeColor = Color.White,
                Font = PoliceGras,
                Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }

        public static void StyliserGrille(DataGridView g)
        {
            g.ReadOnly = true;
            g.AllowUserToAddRows = false;
            g.AllowUserToDeleteRows = false;
            g.AllowUserToResizeRows = false;
            g.MultiSelect = false;
            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            g.RowHeadersVisible = false;
            g.BackgroundColor = Color.White;
            g.BorderStyle = BorderStyle.None;
            g.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            g.GridColor = Color.FromArgb(226, 232, 240);
            g.EnableHeadersVisualStyles = false;
            g.ColumnHeadersHeight = 38;
            g.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            g.ColumnHeadersDefaultCellStyle.BackColor = Menu;
            g.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            g.ColumnHeadersDefaultCellStyle.Font = PoliceGras;
            g.DefaultCellStyle.Font = Police;
            g.DefaultCellStyle.SelectionBackColor = Color.FromArgb(191, 219, 254);
            g.DefaultCellStyle.SelectionForeColor = TexteFonce;
            g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            g.RowTemplate.Height = 32;
        }
    }
}
