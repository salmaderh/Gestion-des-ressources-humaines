using System.Linq;
using System.Windows.Forms;
using GestionRH.Services;

namespace GestionRH.Vues
{
    public class FormGenererPaie : FormEditionBase
    {
        private readonly ComboBox employe;
        private readonly NumericUpDown mois, annee, primes, heuresSup;

        public FormGenererPaie(int moisInitial, int anneeInitiale) : base("Générer une fiche de paie")
        {
            employe = AjouterChamp("Employé *", Liste());
            ElementListe.Remplir(employe, new ServiceEmployes().ListerActifs()
                .Select(e => new ElementListe(e.Id, e.Matricule + " - " + e.NomComplet)));
            mois = AjouterChamp("Mois", Nombre(moisInitial, 1, 12));
            annee = AjouterChamp("Année", Nombre(anneeInitiale, 2000, 2100));
            annee.ThousandsSeparator = false;
            primes = AjouterChamp("Primes (TND)", Nombre(0, 0, 1000000, 3));
            heuresSup = AjouterChamp("Heures sup. (montant TND)", Nombre(0, 0, 1000000, 3));
            AjouterChamp("", new Label
            {
                AutoSize = true, ForeColor = Theme.Gris,
                Text = "Les jours de congé sans solde approuvés sont retenus automatiquement."
            });
            BoutonValider.Text = "Générer";
            TerminerMiseEnPage();
        }

        protected override void Enregistrer()
        {
            object id = ElementListe.ValeurDe(employe);
            Exiger(id != null, "Choisissez un employé.");
            new ServicePaie().Generer((int)id, (int)mois.Value, (int)annee.Value, primes.Value, heuresSup.Value);
        }
    }
}
