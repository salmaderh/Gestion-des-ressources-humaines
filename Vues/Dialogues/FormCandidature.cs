using System.Windows.Forms;
using GestionRH.Modeles;
using GestionRH.Services;

namespace GestionRH.Vues
{
    public class FormCandidature : FormEditionBase
    {
        private readonly Candidature candidature;
        private readonly TextBox nom, prenom, email, telephone, commentaire;
        private readonly DateTimePicker date;
        private readonly ComboBox statut;
        private readonly CheckBox noteSaisie;
        private readonly NumericUpDown note;

        public FormCandidature(Candidature c, int offreId)
            : base(c == null ? "Nouvelle candidature" : "Modifier la candidature")
        {
            candidature = c ?? new Candidature { OffreEmploiId = offreId };

            nom = AjouterChamp("Nom *", Texte(candidature.Nom));
            prenom = AjouterChamp("Prénom *", Texte(candidature.Prenom));
            email = AjouterChamp("E-mail", Texte(candidature.Email));
            telephone = AjouterChamp("Téléphone", Texte(candidature.Telephone));
            date = AjouterChamp("Date de candidature", Date(candidature.DateCandidature));

            statut = AjouterChamp("Statut", Liste());
            ElementListe.Remplir(statut, ElementListe.DepuisEnum<StatutCandidature>());
            ElementListe.Selectionner(statut, candidature.Statut);

            noteSaisie = AjouterChamp("Entretien noté", new CheckBox { Checked = candidature.NoteEntretien.HasValue });
            note = AjouterChamp("Note d'entretien (/20)", Nombre(candidature.NoteEntretien ?? 10, 0, 20));
            note.Enabled = noteSaisie.Checked;
            noteSaisie.CheckedChanged += (s, e) => note.Enabled = noteSaisie.Checked;

            commentaire = AjouterChamp("Commentaire", Texte(candidature.Commentaire, true));
            TerminerMiseEnPage();
        }

        protected override void Enregistrer()
        {
            candidature.Nom = nom.Text.Trim();
            candidature.Prenom = prenom.Text.Trim();
            candidature.Email = email.Text.Trim();
            candidature.Telephone = telephone.Text.Trim();
            candidature.DateCandidature = date.Value.Date;
            candidature.Statut = (StatutCandidature)ElementListe.ValeurDe(statut);
            candidature.NoteEntretien = noteSaisie.Checked ? (int?)note.Value : null;
            candidature.Commentaire = commentaire.Text.Trim();
            candidature.OffreEmploi = null;
            new ServiceCandidatures().Enregistrer(candidature);
        }
    }
}
