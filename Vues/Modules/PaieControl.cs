using System;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using GestionRH.Modeles;
using GestionRH.Services;

namespace GestionRH.Vues
{
    public class PaieControl : ModuleBase
    {
        private readonly ServicePaie service = new ServicePaie();
        private readonly NumericUpDown mois, annee;
        private readonly Label resume;

        public PaieControl() : base("Paie")
        {
            AjouterBouton("Générer une fiche", Theme.Principal, GenererUne);
            AjouterBouton("Générer le mois (tous)", Theme.Succes, GenererTous);
            AjouterBouton("Voir le bulletin", Theme.Avertissement, VoirBulletin);
            AjouterBouton("Supprimer", Theme.Danger, Supprimer);

            DateTime precedent = DateTime.Today.AddMonths(-1);
            mois = new NumericUpDown { Minimum = 1, Maximum = 12, Value = precedent.Month, Width = 60 };
            annee = new NumericUpDown { Minimum = 2000, Maximum = 2100, Value = precedent.Year, Width = 80 };
            mois.ValueChanged += (s, e) => Rafraichir();
            annee.ValueChanged += (s, e) => Rafraichir();
            AjouterFiltre("Mois :", mois);
            AjouterFiltre("Année :", annee);

            resume = new Label { Height = 34, Font = Theme.PoliceGras, ForeColor = Theme.TexteFonce, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
            AjouterSousBarre(resume);
        }

        public override void Rafraichir()
        {
            CultureInfo fr = CultureInfo.GetCultureInfo("fr-FR");
            var fiches = service.Lister((int)mois.Value, (int)annee.Value);
            Afficher(fiches.Select(f => new
            {
                f.Id,
                Employé = f.Employe.NomComplet,
                Période = f.Periode,
                Base = f.SalaireBase.ToString("N3", fr),
                Primes = f.Primes.ToString("N3", fr),
                Heures_sup = f.HeuresSupplementaires.ToString("N3", fr),
                Retenue = f.RetenueAbsences.ToString("N3", fr),
                Brut = f.SalaireBrut.ToString("N3", fr),
                Cotisations = f.CotisationsSociales.ToString("N3", fr),
                Impôt = f.ImpotRevenu.ToString("N3", fr),
                Net = f.SalaireNet.ToString("N3", fr)
            }));
            decimal masse = fiches.Sum(f => f.SalaireNet);
            resume.Text = fiches.Count + " fiche(s) pour " + ((int)mois.Value).ToString("00") + "/" + annee.Value +
                          "   —   Masse salariale nette : " + masse.ToString("N3", fr) + " TND";
        }

        protected override void DoubleClicLigne() => VoirBulletin();

        private void GenererUne()
        {
            OuvrirDialogue(new FormGenererPaie((int)mois.Value, (int)annee.Value));
        }

        private void GenererTous()
        {
            int m = (int)mois.Value, a = (int)annee.Value;
            if (!Confirmer("Générer les fiches de " + m.ToString("00") + "/" + a + " pour tous les employés actifs ?")) return;
            Executer(() =>
            {
                int n = service.GenererPourTous(m, a);
                MessageBox.Show(this, n + " fiche(s) générée(s).", "Paie", MessageBoxButtons.OK, MessageBoxIcon.Information);
            });
        }

        private void VoirBulletin()
        {
            int? id = ExigerSelection();
            if (id == null) return;
            FichePaie f = service.ObtenirAvecEmploye(id.Value);
            if (f == null) return;
            using (FormBulletin b = new FormBulletin(f)) b.ShowDialog(this);
        }

        private void Supprimer()
        {
            int? id = ExigerSelection();
            if (id != null && Confirmer("Supprimer cette fiche de paie ?"))
                Executer(() => service.Supprimer(id.Value));
        }
    }
}
