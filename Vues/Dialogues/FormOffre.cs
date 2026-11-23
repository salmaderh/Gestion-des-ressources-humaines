using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using GestionRH.Modeles;
using GestionRH.Services;

namespace GestionRH.Vues
{
    public class FormOffre : FormEditionBase
    {
        private static readonly string[] TypesContrat = { "CDI", "CDD", "Stage", "Freelance" };

        private readonly OffreEmploi offre;
        private readonly TextBox titre, description;
        private readonly ComboBox departement, contrat, statut;
        private readonly DateTimePicker publication, limite;

        public FormOffre(OffreEmploi o) : base(o == null ? "Nouvelle offre d'emploi" : "Modifier l'offre")
        {
            offre = o ?? new OffreEmploi();

            titre = AjouterChamp("Intitulé du poste *", Texte(offre.Titre));

            departement = AjouterChamp("Département", Liste());
            List<ElementListe> deps = new List<ElementListe> { new ElementListe(null, "(Aucun)") };
            deps.AddRange(new ServiceDepartements().Lister().Select(d => new ElementListe(d.Id, d.Nom)));
            ElementListe.Remplir(departement, deps);
            ElementListe.Selectionner(departement, offre.DepartementId);

            contrat = AjouterChamp("Type de contrat", Liste());
            ElementListe.Remplir(contrat, TypesContrat.Select(t => new ElementListe(t, t)));
            ElementListe.Selectionner(contrat, offre.TypeContrat);

            publication = AjouterChamp("Date de publication", Date(offre.DatePublication));
            limite = AjouterChamp("Date limite", Date(offre.DateLimite));

            statut = AjouterChamp("Statut", Liste());
            ElementListe.Remplir(statut, ElementListe.DepuisEnum<StatutOffre>());
            ElementListe.Selectionner(statut, offre.Statut);

            description = AjouterChamp("Description / profil", Texte(offre.Description, true));
            TerminerMiseEnPage();
        }

        protected override void Enregistrer()
        {
            offre.Titre = titre.Text.Trim();
            offre.DepartementId = (int?)ElementListe.ValeurDe(departement);
            offre.Departement = null;
            offre.TypeContrat = (string)ElementListe.ValeurDe(contrat);
            offre.DatePublication = publication.Value.Date;
            offre.DateLimite = limite.Value.Date;
            offre.Statut = (StatutOffre)ElementListe.ValeurDe(statut);
            offre.Description = description.Text.Trim();
            offre.Candidatures = new List<Candidature>();   // on n'enregistre que l'offre elle-même
            new ServiceOffres().Enregistrer(offre);
        }
    }
}
