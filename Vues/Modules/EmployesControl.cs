using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using GestionRH.Modeles;
using GestionRH.Services;

namespace GestionRH.Vues
{
    public class EmployesControl : ModuleBase
    {
        private readonly ServiceEmployes service = new ServiceEmployes();
        private readonly TextBox recherche;
        private readonly ComboBox filtreDepartement;

        public EmployesControl() : base("Employés")
        {
            AjouterBouton("Ajouter", Theme.Principal, Ajouter);
            AjouterBouton("Modifier", Theme.Avertissement, Modifier);
            AjouterBouton("Supprimer", Theme.Danger, Supprimer);

            recherche = new TextBox { Width = 200 };
            recherche.TextChanged += (s, e) => Rafraichir();
            AjouterFiltre("Rechercher :", recherche);
            filtreDepartement = AjouterFiltreListe("Département :");
        }

        public override void Rafraichir()
        {
            List<ElementListe> deps = new List<ElementListe> { new ElementListe(null, "(Tous)") };
            deps.AddRange(new ServiceDepartements().Lister().Select(d => new ElementListe(d.Id, d.Nom)));
            RechargerListe(filtreDepartement, deps);

            int? departementId = (int?)ElementListe.ValeurDe(filtreDepartement);
            Afficher(service.Rechercher(recherche.Text, departementId).Select(e => new
            {
                e.Id,
                e.Matricule,
                Nom = e.NomComplet,
                e.Poste,
                Département = e.Departement != null ? e.Departement.Nom : "-",
                Embauche = e.DateEmbauche.ToString("dd/MM/yyyy"),
                Salaire = e.SalaireBase.ToString("N3"),
                Solde_congés = e.SoldeConges,
                Statut = e.Statut.Libelle()
            }));
        }

        protected override void DoubleClicLigne() => Modifier();

        private void Ajouter()
        {
            OuvrirDialogue(new FormEmploye(null));
        }

        private void Modifier()
        {
            int? id = ExigerSelection();
            if (id != null) OuvrirDialogue(new FormEmploye(service.Obtenir(id.Value)));
        }

        private void Supprimer()
        {
            int? id = ExigerSelection();
            if (id != null && Confirmer("Supprimer cet employé ainsi que ses congés, fiches de paie et évaluations ?"))
                Executer(() => service.Supprimer(id.Value));
        }
    }
}
