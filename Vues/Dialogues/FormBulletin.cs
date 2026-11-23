using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using GestionRH.Modeles;
using GestionRH.Services;

namespace GestionRH.Vues
{
    /// <summary>Affiche un bulletin de paie et permet de l'exporter en fichier texte.</summary>
    public class FormBulletin : Form
    {
        private readonly FichePaie fiche;
        private readonly TextBox contenu;

        public FormBulletin(FichePaie f)
        {
            fiche = f;
            Text = "Bulletin de paie " + f.Periode;
            Font = Theme.Police;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(620, 600);
            MinimizeBox = false;
            ShowInTaskbar = false;

            contenu = new TextBox
            {
                Dock = DockStyle.Fill, Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical,
                Font = new Font("Consolas", 10.5F), BackColor = Color.White, Text = Construire(f)
            };
            FlowLayoutPanel bas = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom, Height = 56, FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(12, 10, 12, 10), BackColor = Theme.Fond
            };
            Button fermer = Theme.CreerBouton("Fermer", Theme.Gris);
            fermer.Click += (s, e) => Close();
            Button exporter = Theme.CreerBouton("Exporter (.txt)", Theme.Principal);
            exporter.Click += (s, e) => Exporter();
            bas.Controls.Add(fermer);
            bas.Controls.Add(exporter);

            Controls.Add(contenu);
            Controls.Add(bas);
            CancelButton = fermer;
        }

        public static string Construire(FichePaie f)
        {
            CultureInfo fr = CultureInfo.GetCultureInfo("fr-FR");
            string Ligne(string libelle, decimal montant) => libelle.PadRight(38) + montant.ToString("N3", fr).PadLeft(16) + " TND";
            string sep = new string('-', 60);
            Employe e = f.Employe;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("                    BULLETIN DE PAIE");
            sb.AppendLine("               Période : " + f.Periode);
            sb.AppendLine(sep);
            if (e != null)
            {
                sb.AppendLine("Matricule    : " + e.Matricule);
                sb.AppendLine("Employé      : " + e.NomComplet);
                sb.AppendLine("Poste        : " + e.Poste);
                sb.AppendLine("Département  : " + (e.Departement != null ? e.Departement.Nom : "-"));
                sb.AppendLine("Embauché le  : " + e.DateEmbauche.ToString("dd/MM/yyyy"));
            }
            sb.AppendLine(sep);
            sb.AppendLine(Ligne("Salaire de base", f.SalaireBase));
            sb.AppendLine(Ligne("Primes", f.Primes));
            sb.AppendLine(Ligne("Heures supplémentaires", f.HeuresSupplementaires));
            sb.AppendLine(Ligne("Retenue congé sans solde (" + f.JoursSansSolde + " j)", -f.RetenueAbsences));
            sb.AppendLine(sep);
            sb.AppendLine(Ligne("SALAIRE BRUT", f.SalaireBrut));
            sb.AppendLine(Ligne("Cotisations sociales (" + (ServicePaie.TauxCotisations * 100).ToString("0.##", fr) + " %)", -f.CotisationsSociales));
            sb.AppendLine(Ligne("Impôt sur le revenu (" + (ServicePaie.TauxImpot * 100).ToString("0.##", fr) + " %)", -f.ImpotRevenu));
            sb.AppendLine(sep);
            sb.AppendLine(Ligne("NET À PAYER", f.SalaireNet));
            sb.AppendLine(sep);
            sb.AppendLine("Généré le " + f.DateGeneration.ToString("dd/MM/yyyy HH:mm"));
            return sb.ToString();
        }

        private void Exporter()
        {
            using (SaveFileDialog d = new SaveFileDialog())
            {
                d.Filter = "Fichier texte (*.txt)|*.txt";
                d.FileName = "bulletin_" + (fiche.Employe != null ? fiche.Employe.Matricule : "employe") +
                             "_" + fiche.Annee + "_" + fiche.Mois.ToString("00") + ".txt";
                if (d.ShowDialog(this) != DialogResult.OK) return;
                try
                {
                    File.WriteAllText(d.FileName, contenu.Text, Encoding.UTF8);
                    MessageBox.Show(this, "Bulletin exporté.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (IOException ex)
                {
                    MessageBox.Show(this, ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
