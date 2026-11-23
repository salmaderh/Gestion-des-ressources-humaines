using System;
using System.Linq;
using System.Windows.Forms;
using GestionRH.Modeles;
using GestionRH.Services;

namespace GestionRH.Vues
{
    public class FormDemandeConge : FormEditionBase
    {
        private readonly DemandeConge demande;
        private readonly ComboBox employe, type;
        private readonly DateTimePicker debut, fin;
        private readonly Label jours;
        private readonly TextBox motif;

        public FormDemandeConge(DemandeConge d)
            : base(d == null ? "Nouvelle demande de congé" : "Modifier la demande de congé")
        {
            demande = d ?? new DemandeConge { DateDebut = DateTime.Today.AddDays(1), DateFin = DateTime.Today.AddDays(1) };

            employe = AjouterChamp("Employé *", Liste());
            ElementListe.Remplir(employe, new ServiceEmployes().ListerActifs()
                .Select(e => new ElementListe(e.Id, e.NomComplet + "  (solde : " + e.SoldeConges + " j)")));
            if (demande.EmployeId > 0) ElementListe.Selectionner(employe, demande.EmployeId);

            type = AjouterChamp("Type de congé", Liste());
            ElementListe.Remplir(type, ElementListe.DepuisEnum<TypeConge>());
            ElementListe.Selectionner(type, demande.Type);

            debut = AjouterChamp("Du", Date(demande.DateDebut));
            fin = AjouterChamp("Au (inclus)", Date(demande.DateFin));
            jours = AjouterChamp("Jours ouvrés", new Label { AutoSize = true, Font = Theme.PoliceGras });
            motif = AjouterChamp("Motif", Texte(demande.Motif, true));

            debut.ValueChanged += (s, e) => { if (fin.Value < debut.Value) fin.Value = debut.Value; MettreAJourJours(); };
            fin.ValueChanged += (s, e) => MettreAJourJours();
            MettreAJourJours();
            TerminerMiseEnPage();
        }

        private void MettreAJourJours()
        {
            int n = DemandeConge.CompterJoursOuvres(debut.Value, fin.Value);
            jours.Text = n + " jour(s)";
        }

        protected override void Enregistrer()
        {
            object id = ElementListe.ValeurDe(employe);
            Exiger(id != null, "Choisissez un employé.");
            demande.EmployeId = (int)id;
            demande.Employe = null;
            demande.Type = (TypeConge)ElementListe.ValeurDe(type);
            demande.DateDebut = debut.Value.Date;
            demande.DateFin = fin.Value.Date;
            demande.Motif = motif.Text.Trim();
            new ServiceConges().Enregistrer(demande);
        }
    }
}
