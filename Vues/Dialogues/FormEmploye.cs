using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using GestionRH.Modeles;
using GestionRH.Services;

namespace GestionRH.Vues
{
    public class FormEmploye : FormEditionBase
    {
        private readonly Employe employe;
        private readonly TextBox matricule, nom, prenom, email, telephone, poste;
        private readonly ComboBox departement, statut;
        private readonly DateTimePicker dateEmbauche;
        private readonly NumericUpDown salaire, solde;

        public FormEmploye(Employe e)
            : base(e == null ? "Nouvel employé" : "Modifier l'employé")
        {
            employe = e ?? new Employe { Matricule = new ServiceEmployes().ProchainMatricule() };

            matricule = AjouterChamp("Matricule *", Texte(employe.Matricule));
            nom = AjouterChamp("Nom *", Texte(employe.Nom));
            prenom = AjouterChamp("Prénom *", Texte(employe.Prenom));
            email = AjouterChamp("E-mail", Texte(employe.Email));
            telephone = AjouterChamp("Téléphone", Texte(employe.Telephone));
            poste = AjouterChamp("Poste", Texte(employe.Poste));

            departement = AjouterChamp("Département", Liste());
            List<ElementListe> deps = new List<ElementListe> { new ElementListe(null, "(Aucun)") };
            deps.AddRange(new ServiceDepartements().Lister().Select(d => new ElementListe(d.Id, d.Nom)));
            ElementListe.Remplir(departement, deps);
            ElementListe.Selectionner(departement, employe.DepartementId);

            dateEmbauche = AjouterChamp("Date d'embauche", Date(employe.DateEmbauche));
            salaire = AjouterChamp("Salaire de base (TND)", Nombre(employe.SalaireBase, 0, 1000000, 3));
            solde = AjouterChamp("Solde congés (jours)", Nombre((decimal)employe.SoldeConges, 0, 365, 1));

            statut = AjouterChamp("Statut", Liste());
            ElementListe.Remplir(statut, ElementListe.DepuisEnum<StatutEmploye>());
            ElementListe.Selectionner(statut, employe.Statut);

            TerminerMiseEnPage();
        }

        protected override void Enregistrer()
        {
            employe.Matricule = matricule.Text.Trim();
            employe.Nom = nom.Text.Trim();
            employe.Prenom = prenom.Text.Trim();
            employe.Email = email.Text.Trim();
            employe.Telephone = telephone.Text.Trim();
            employe.Poste = poste.Text.Trim();
            employe.DepartementId = (int?)ElementListe.ValeurDe(departement);
            employe.Departement = null;
            employe.DateEmbauche = dateEmbauche.Value.Date;
            employe.SalaireBase = salaire.Value;
            employe.SoldeConges = (double)solde.Value;
            employe.Statut = (StatutEmploye)ElementListe.ValeurDe(statut);
            new ServiceEmployes().Enregistrer(employe);
        }
    }
}
