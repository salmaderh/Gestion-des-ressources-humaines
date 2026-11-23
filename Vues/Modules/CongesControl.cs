using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using GestionRH.Modeles;
using GestionRH.Services;

namespace GestionRH.Vues
{
    public class CongesControl : ModuleBase
    {
        private readonly ServiceConges service = new ServiceConges();
        private readonly ComboBox filtreStatut;

        public CongesControl() : base("Congés")
        {
            AjouterBouton("Nouvelle demande", Theme.Principal, () => OuvrirDialogue(new FormDemandeConge(null)));
            AjouterBouton("Modifier", Theme.Avertissement, Modifier);
            AjouterBouton("Approuver", Theme.Succes, () => Decider(true));
            AjouterBouton("Refuser", Theme.Danger, () => Decider(false));
            AjouterBouton("Supprimer", Theme.Gris, Supprimer);

            filtreStatut = AjouterFiltreListe("Statut :", 150);
            List<ElementListe> statuts = new List<ElementListe> { new ElementListe(null, "(Tous)") };
            statuts.AddRange(ElementListe.DepuisEnum<StatutDemande>());
            RechargerListe(filtreStatut, statuts);
        }

        public override void Rafraichir()
        {
            StatutDemande? statut = (StatutDemande?)ElementListe.ValeurDe(filtreStatut);
            Afficher(service.Lister(statut).Select(c => new
            {
                c.Id,
                Employé = c.Employe.NomComplet,
                Type = c.Type.Libelle(),
                Du = c.DateDebut.ToString("dd/MM/yyyy"),
                Au = c.DateFin.ToString("dd/MM/yyyy"),
                Jours = c.NombreJours,
                Statut = c.Statut.Libelle(),
                Demandée_le = c.DateDemande.ToString("dd/MM/yyyy"),
                c.Motif,
                Décision = c.CommentaireDecision
            }));
        }

        protected override void DoubleClicLigne() => Modifier();

        private void Modifier()
        {
            int? id = ExigerSelection();
            if (id != null) OuvrirDialogue(new FormDemandeConge(service.Obtenir(id.Value)));
        }

        private void Decider(bool approuver)
        {
            int? id = ExigerSelection();
            if (id == null) return;
            using (FormSaisieTexte f = new FormSaisieTexte(approuver ? "Approuver la demande" : "Refuser la demande",
                                                           "Commentaire", approuver ? "Approuver" : "Refuser"))
            {
                if (f.ShowDialog(this) != DialogResult.OK) return;
                Executer(() =>
                {
                    if (approuver) service.Approuver(id.Value, f.Valeur);
                    else service.Refuser(id.Value, f.Valeur);
                });
            }
        }

        private void Supprimer()
        {
            int? id = ExigerSelection();
            if (id != null && Confirmer("Supprimer cette demande ? (un congé annuel approuvé sera recrédité)"))
                Executer(() => service.Supprimer(id.Value));
        }
    }
}
