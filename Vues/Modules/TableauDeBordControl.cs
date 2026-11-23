using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using GestionRH.Modeles;
using GestionRH.Services;

namespace GestionRH.Vues
{
    /// <summary>Indicateurs clés + traitement rapide des demandes de congé en attente.</summary>
    public class TableauDeBordControl : ModuleBase
    {
        private readonly FlowLayoutPanel cartes;
        private readonly ServiceConges serviceConges = new ServiceConges();

        public TableauDeBordControl() : base("Tableau de bord")
        {
            cartes = new FlowLayoutPanel { Height = 130, WrapContents = false, AutoScroll = true, Padding = new Padding(0, 4, 0, 8) };
            AjouterSousBarre(cartes);

            BarreOutils.Controls.Add(new Label
            {
                Text = "Demandes de congé en attente :", AutoSize = true,
                Font = Theme.PoliceGras, Margin = new Padding(0, 9, 12, 0)
            });
            AjouterBouton("Approuver", Theme.Succes, () => Decider(true));
            AjouterBouton("Refuser", Theme.Danger, () => Decider(false));
            AjouterBouton("Actualiser", Theme.Gris, Rafraichir);
        }

        public override void Rafraichir()
        {
            Indicateurs i = new ServiceTableauDeBord().Calculer();
            CultureInfo fr = CultureInfo.GetCultureInfo("fr-FR");

            cartes.Controls.Clear();
            cartes.Controls.Add(Carte("Employés actifs", i.EmployesActifs.ToString(), Theme.Principal));
            cartes.Controls.Add(Carte("Départements", i.Departements.ToString(), Theme.Gris));
            cartes.Controls.Add(Carte("Congés en attente", i.CongesEnAttente.ToString(), Theme.Avertissement));
            cartes.Controls.Add(Carte("En congé aujourd'hui", i.EnCongeAujourdhui.ToString(), Theme.Succes));
            cartes.Controls.Add(Carte("Offres ouvertes", i.OffresOuvertes + " (" + i.CandidaturesEnCours + " cand.)", Theme.Principal));
            cartes.Controls.Add(Carte("Masse salariale nette (M-1)", i.MasseSalarialeMoisPrecedent.ToString("N0", fr) + " TND", Theme.Danger));
            cartes.Controls.Add(Carte("Note moyenne évaluations", i.NoteMoyenne.ToString("0.00", fr) + " / 5", Theme.Succes));

            Afficher(serviceConges.Lister(StatutDemande.EnAttente).Select(c => new
            {
                c.Id,
                Employé = c.Employe.NomComplet,
                Type = c.Type.Libelle(),
                Du = c.DateDebut.ToString("dd/MM/yyyy"),
                Au = c.DateFin.ToString("dd/MM/yyyy"),
                Jours = c.NombreJours,
                Solde_actuel = c.Employe.SoldeConges,
                c.Motif
            }));
        }

        private static Control Carte(string titre, string valeur, Color couleur)
        {
            Panel carte = new Panel { Width = 210, Height = 105, BackColor = Color.White, Margin = new Padding(0, 0, 12, 0) };
            Panel bande = new Panel { Dock = DockStyle.Left, Width = 6, BackColor = couleur };
            Label v = new Label
            {
                Text = valeur, Font = new Font("Segoe UI", 16F, FontStyle.Bold), ForeColor = couleur,
                Location = new Point(18, 18), AutoSize = true
            };
            Label t = new Label { Text = titre, ForeColor = Theme.Gris, Location = new Point(18, 66), AutoSize = true };
            carte.Controls.Add(v);
            carte.Controls.Add(t);
            carte.Controls.Add(bande);
            return carte;
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
                    if (approuver) serviceConges.Approuver(id.Value, f.Valeur);
                    else serviceConges.Refuser(id.Value, f.Valeur);
                });
            }
        }
    }
}
