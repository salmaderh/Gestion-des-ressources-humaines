using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GestionRH.Modeles;
using GestionRH.Services;

namespace GestionRH.Vues
{
    /// <summary>Offres d'emploi (grille du haut) et candidatures de l'offre sélectionnée (grille du bas).</summary>
    public class RecrutementControl : ModuleBase
    {
        private readonly ServiceOffres serviceOffres = new ServiceOffres();
        private readonly ServiceCandidatures serviceCandidatures = new ServiceCandidatures();
        private readonly DataGridView grilleCandidats;
        private readonly Label titreCandidats;

        public RecrutementControl() : base("Recrutement")
        {
            AjouterBouton("Nouvelle offre", Theme.Principal, () => OuvrirDialogue(new FormOffre(null)));
            AjouterBouton("Modifier l'offre", Theme.Avertissement, ModifierOffre);
            AjouterBouton("Supprimer l'offre", Theme.Danger, SupprimerOffre);

            // ----- Panneau des candidatures (en bas) -----
            Panel bas = new Panel { Dock = DockStyle.Bottom, Height = 330, Padding = new Padding(0, 12, 0, 0) };
            grilleCandidats = new DataGridView { Dock = DockStyle.Fill };
            Theme.StyliserGrille(grilleCandidats);
            grilleCandidats.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0) ModifierCandidature(); };

            FlowLayoutPanel barre = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 46, WrapContents = false, Padding = new Padding(0, 4, 0, 6) };
            barre.Controls.Add(BoutonCandidat("Ajouter", Theme.Principal, AjouterCandidature));
            barre.Controls.Add(BoutonCandidat("Modifier", Theme.Avertissement, ModifierCandidature));
            barre.Controls.Add(BoutonCandidat("Entretien", Theme.Gris, () => ChangerStatut(StatutCandidature.Entretien)));
            barre.Controls.Add(BoutonCandidat("Refuser", Theme.Danger, () => ChangerStatut(StatutCandidature.Refusee)));
            barre.Controls.Add(BoutonCandidat("Embaucher", Theme.Succes, Embaucher));
            barre.Controls.Add(BoutonCandidat("Supprimer", Theme.Danger, SupprimerCandidature));

            titreCandidats = new Label { Dock = DockStyle.Top, Height = 30, Font = new Font("Segoe UI", 13F, FontStyle.Bold), ForeColor = Theme.TexteFonce };

            bas.Controls.Add(grilleCandidats);
            bas.Controls.Add(barre);
            bas.Controls.Add(titreCandidats);
            AjouterSousBarre(bas);

            Grille.SelectionChanged += (s, e) => RafraichirCandidats();
        }

        private static Button BoutonCandidat(string texte, Color couleur, System.Action action)
        {
            Button b = Theme.CreerBouton(texte, couleur);
            b.Click += (s, e) => action();
            return b;
        }

        public override void Rafraichir()
        {
            int? selection = IdSelectionne();
            Afficher(serviceOffres.Lister().Select(o => new
            {
                o.Id,
                Intitulé = o.Titre,
                Département = o.Departement != null ? o.Departement.Nom : "-",
                Contrat = o.TypeContrat,
                Publiée_le = o.DatePublication.ToString("dd/MM/yyyy"),
                Date_limite = o.DateLimite.ToString("dd/MM/yyyy"),
                Statut = o.Statut.Libelle(),
                Candidatures = o.Candidatures.Count
            }));
            SelectionnerLigne(Grille, selection);
            RafraichirCandidats();
        }

        private void RafraichirCandidats()
        {
            int? offreId = IdSelectionne();
            if (offreId == null)
            {
                titreCandidats.Text = "Candidatures (sélectionnez une offre)";
                grilleCandidats.DataSource = null;
                return;
            }
            string titre = Grille.CurrentRow.Cells["Intitulé"].Value as string;
            titreCandidats.Text = "Candidatures — " + titre;
            Afficher(grilleCandidats, serviceCandidatures.ListerParOffre(offreId.Value).Select(c => new
            {
                c.Id,
                Candidat = c.NomComplet,
                c.Email,
                Téléphone = c.Telephone,
                Reçue_le = c.DateCandidature.ToString("dd/MM/yyyy"),
                Statut = c.Statut.Libelle(),
                Note_entretien = c.NoteEntretien.HasValue ? c.NoteEntretien.Value + " / 20" : "-",
                c.Commentaire
            }));
        }

        protected override void DoubleClicLigne() => ModifierOffre();

        private void ModifierOffre()
        {
            int? id = ExigerSelection();
            if (id != null) OuvrirDialogue(new FormOffre(serviceOffres.Obtenir(id.Value)));
        }

        private void SupprimerOffre()
        {
            int? id = ExigerSelection();
            if (id != null && Confirmer("Supprimer cette offre et toutes ses candidatures ?"))
                Executer(() => serviceOffres.Supprimer(id.Value));
        }

        private void AjouterCandidature()
        {
            int? offreId = ExigerSelection();
            if (offreId != null) OuvrirDialogue(new FormCandidature(null, offreId.Value));
        }

        private void ModifierCandidature()
        {
            int? id = ExigerSelection(grilleCandidats);
            if (id != null) OuvrirDialogue(new FormCandidature(serviceCandidatures.Obtenir(id.Value), 0));
        }

        private void ChangerStatut(StatutCandidature statut)
        {
            int? id = ExigerSelection(grilleCandidats);
            if (id != null) Executer(() => serviceCandidatures.ChangerStatut(id.Value, statut));
        }

        private void Embaucher()
        {
            int? id = ExigerSelection(grilleCandidats);
            if (id == null) return;
            Candidature c = serviceCandidatures.Obtenir(id.Value);
            string titreOffre = Grille.CurrentRow != null ? Grille.CurrentRow.Cells["Intitulé"].Value as string : "";
            using (FormEmbauche f = new FormEmbauche(c, titreOffre))
            {
                if (f.ShowDialog(this) != DialogResult.OK) return;
                MessageBox.Show(this, f.EmployeCree.NomComplet + " a été ajouté(e) aux employés (matricule " +
                                f.EmployeCree.Matricule + ").", "Embauche", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Rafraichir();
            }
        }

        private void SupprimerCandidature()
        {
            int? id = ExigerSelection(grilleCandidats);
            if (id != null && Confirmer("Supprimer cette candidature ?"))
                Executer(() => serviceCandidatures.Supprimer(id.Value));
        }
    }
}
