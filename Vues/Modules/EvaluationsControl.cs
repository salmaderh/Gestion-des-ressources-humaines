using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using GestionRH.Modeles;
using GestionRH.Services;

namespace GestionRH.Vues
{
    public class EvaluationsControl : ModuleBase
    {
        private readonly ServiceEvaluations service = new ServiceEvaluations();
        private readonly ComboBox filtreEmploye;
        private readonly string evaluateur;

        public EvaluationsControl(string evaluateurParDefaut) : base("Évaluations")
        {
            evaluateur = evaluateurParDefaut;
            AjouterBouton("Nouvelle évaluation", Theme.Principal, () => OuvrirDialogue(new FormEvaluation(null, evaluateur)));
            AjouterBouton("Modifier", Theme.Avertissement, Modifier);
            AjouterBouton("Supprimer", Theme.Danger, Supprimer);
            filtreEmploye = AjouterFiltreListe("Employé :", 240);
        }

        public override void Rafraichir()
        {
            List<ElementListe> employes = new List<ElementListe> { new ElementListe(null, "(Tous)") };
            employes.AddRange(new ServiceEmployes().Lister().Select(e => new ElementListe(e.Id, e.NomComplet)));
            RechargerListe(filtreEmploye, employes);

            int? employeId = (int?)ElementListe.ValeurDe(filtreEmploye);
            Afficher(service.Lister(employeId).Select(v => new
            {
                v.Id,
                Employé = v.Employe.NomComplet,
                Période = v.Periode,
                Date = v.DateEvaluation.ToString("dd/MM/yyyy"),
                Évaluateur = v.Evaluateur,
                Compétences = v.NoteCompetences,
                Objectifs = v.NoteObjectifs,
                Comportement = v.NoteComportement,
                Note_globale = v.NoteGlobale.ToString("0.00"),
                Appréciation = v.Appreciation
            }));
        }

        protected override void DoubleClicLigne() => Modifier();

        private void Modifier()
        {
            int? id = ExigerSelection();
            if (id != null) OuvrirDialogue(new FormEvaluation(service.Obtenir(id.Value), evaluateur));
        }

        private void Supprimer()
        {
            int? id = ExigerSelection();
            if (id != null && Confirmer("Supprimer cette évaluation ?"))
                Executer(() => service.Supprimer(id.Value));
        }
    }
}
