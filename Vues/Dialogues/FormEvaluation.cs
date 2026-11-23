using System;
using System.Linq;
using System.Windows.Forms;
using GestionRH.Modeles;
using GestionRH.Services;

namespace GestionRH.Vues
{
    public class FormEvaluation : FormEditionBase
    {
        private readonly Evaluation evaluation;
        private readonly ComboBox employe;
        private readonly TextBox periode, evaluateur, objectifs, commentaire;
        private readonly DateTimePicker date;
        private readonly NumericUpDown competences, resultats, comportement;
        private readonly Label noteGlobale;

        public FormEvaluation(Evaluation v, string evaluateurParDefaut)
            : base(v == null ? "Nouvelle évaluation" : "Modifier l'évaluation")
        {
            evaluation = v ?? new Evaluation { Periode = DateTime.Today.Year.ToString(), Evaluateur = evaluateurParDefaut };

            employe = AjouterChamp("Employé *", Liste());
            ElementListe.Remplir(employe, new ServiceEmployes().Lister()
                .Select(e => new ElementListe(e.Id, e.Matricule + " - " + e.NomComplet)));
            if (evaluation.EmployeId > 0) ElementListe.Selectionner(employe, evaluation.EmployeId);

            periode = AjouterChamp("Période * (ex. 2026-S1)", Texte(evaluation.Periode));
            date = AjouterChamp("Date de l'entretien", Date(evaluation.DateEvaluation));
            evaluateur = AjouterChamp("Évaluateur", Texte(evaluation.Evaluateur));
            competences = AjouterChamp("Compétences (1-5)", Nombre(evaluation.NoteCompetences, 1, 5));
            resultats = AjouterChamp("Atteinte objectifs (1-5)", Nombre(evaluation.NoteObjectifs, 1, 5));
            comportement = AjouterChamp("Comportement (1-5)", Nombre(evaluation.NoteComportement, 1, 5));
            noteGlobale = AjouterChamp("Note globale", new Label { AutoSize = true, Font = Theme.PoliceGras });
            objectifs = AjouterChamp("Objectifs suivants", Texte(evaluation.ObjectifsSuivants, true));
            commentaire = AjouterChamp("Commentaire", Texte(evaluation.Commentaire, true));

            competences.ValueChanged += (s, e) => MettreAJourNote();
            resultats.ValueChanged += (s, e) => MettreAJourNote();
            comportement.ValueChanged += (s, e) => MettreAJourNote();
            MettreAJourNote();
            TerminerMiseEnPage();
        }

        private void MettreAJourNote()
        {
            Evaluation apercu = new Evaluation
            {
                NoteCompetences = (int)competences.Value,
                NoteObjectifs = (int)resultats.Value,
                NoteComportement = (int)comportement.Value
            };
            noteGlobale.Text = apercu.NoteGlobale.ToString("0.00") + " / 5  —  " + apercu.Appreciation;
        }

        protected override void Enregistrer()
        {
            object id = ElementListe.ValeurDe(employe);
            Exiger(id != null, "Choisissez un employé.");
            evaluation.EmployeId = (int)id;
            evaluation.Employe = null;
            evaluation.Periode = periode.Text.Trim();
            evaluation.DateEvaluation = date.Value.Date;
            evaluation.Evaluateur = evaluateur.Text.Trim();
            evaluation.NoteCompetences = (int)competences.Value;
            evaluation.NoteObjectifs = (int)resultats.Value;
            evaluation.NoteComportement = (int)comportement.Value;
            evaluation.ObjectifsSuivants = objectifs.Text.Trim();
            evaluation.Commentaire = commentaire.Text.Trim();
            new ServiceEvaluations().Enregistrer(evaluation);
        }
    }
}
