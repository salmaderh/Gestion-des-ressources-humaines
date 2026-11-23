using System;
using System.Windows.Forms;
using GestionRH.Modeles;
using GestionRH.Services;

namespace GestionRH.Vues
{
    /// <summary>Embauche d'un candidat : crée automatiquement la fiche employé.</summary>
    public class FormEmbauche : FormEditionBase
    {
        private readonly Candidature candidature;
        private readonly TextBox poste;
        private readonly NumericUpDown salaire;
        private readonly DateTimePicker date;

        public Employe EmployeCree { get; private set; }

        public FormEmbauche(Candidature c, string titreOffre) : base("Embaucher " + c.NomComplet)
        {
            candidature = c;
            poste = AjouterChamp("Poste", Texte(titreOffre));
            salaire = AjouterChamp("Salaire de base (TND) *", Nombre(1500, 0, 1000000, 3));
            date = AjouterChamp("Date d'embauche", Date(DateTime.Today));
            BoutonValider.Text = "Embaucher";
            TerminerMiseEnPage();
        }

        protected override void Enregistrer()
        {
            EmployeCree = new ServiceCandidatures().Embaucher(candidature.Id, poste.Text.Trim(), salaire.Value, date.Value.Date);
        }
    }
}
