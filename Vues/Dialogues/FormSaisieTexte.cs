using System.Windows.Forms;

namespace GestionRH.Vues
{
    /// <summary>Petite fenêtre de saisie d'un texte (commentaire d'approbation / de refus...).</summary>
    public class FormSaisieTexte : FormEditionBase
    {
        private readonly TextBox saisie;

        public string Valeur { get; private set; }

        public FormSaisieTexte(string titre, string libelle, string texteBouton) : base(titre)
        {
            saisie = AjouterChamp(libelle, Texte("", true));
            BoutonValider.Text = texteBouton;
            TerminerMiseEnPage();
        }

        protected override void Enregistrer()
        {
            Valeur = saisie.Text.Trim();
        }
    }
}
