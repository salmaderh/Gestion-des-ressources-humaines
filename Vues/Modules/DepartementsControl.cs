using System.Linq;
using GestionRH.Services;

namespace GestionRH.Vues
{
    public class DepartementsControl : ModuleBase
    {
        private readonly ServiceDepartements service = new ServiceDepartements();

        public DepartementsControl() : base("Départements")
        {
            AjouterBouton("Ajouter", Theme.Principal, () => OuvrirDialogue(new FormDepartement(null)));
            AjouterBouton("Modifier", Theme.Avertissement, Modifier);
            AjouterBouton("Supprimer", Theme.Danger, Supprimer);
        }

        public override void Rafraichir()
        {
            Afficher(service.Lister().Select(d => new
            {
                d.Id,
                d.Nom,
                d.Description,
                Effectif = d.Employes.Count
            }));
        }

        protected override void DoubleClicLigne() => Modifier();

        private void Modifier()
        {
            int? id = ExigerSelection();
            if (id != null) OuvrirDialogue(new FormDepartement(service.Obtenir(id.Value)));
        }

        private void Supprimer()
        {
            int? id = ExigerSelection();
            if (id != null && Confirmer("Supprimer ce département ? Ses employés resteront sans département."))
                Executer(() => service.Supprimer(id.Value));
        }
    }
}
