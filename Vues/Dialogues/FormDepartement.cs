using System.Windows.Forms;
using GestionRH.Modeles;
using GestionRH.Services;

namespace GestionRH.Vues
{
    public class FormDepartement : FormEditionBase
    {
        private readonly Departement departement;
        private readonly TextBox nom;
        private readonly TextBox description;

        public FormDepartement(Departement d)
            : base(d == null ? "Nouveau département" : "Modifier le département")
        {
            departement = d ?? new Departement();
            nom = AjouterChamp("Nom *", Texte(departement.Nom));
            description = AjouterChamp("Description", Texte(departement.Description, true));
            TerminerMiseEnPage();
        }

        protected override void Enregistrer()
        {
            departement.Nom = nom.Text.Trim();
            departement.Description = description.Text.Trim();
            new ServiceDepartements().Enregistrer(departement);
        }
    }
}
