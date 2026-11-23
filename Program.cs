/*
 * =====================================================================================
 *  Mini-projet Programmation avancée — Gestion des Ressources Humaines (GRH / HRM)
 * =====================================================================================
 *  Auteur : Salma Derhmi
 *
 *  Application de bureau C#.NET (Windows Forms, .NET 8) avec Entity Framework Core et
 *  une base SQLite, approche Code First : la base gestion_rh.db est générée à partir des
 *  classes du modèle par les migrations (dotnet ef migrations add / database update).
 *
 *  Modules : employés, départements, congés, paie, évaluations, recrutement,
 *            tableau de bord, comptes utilisateurs (administrateur).
 *
 *  Architecture (voir docs/CONCEPTION.md) :
 *    Modeles/   entités persistées par EF Core
 *    Donnees/   HrmContext (DbContext) + application des migrations + données de démonstration
 *    Migrations/ migrations EF Core générées (voir docs/MIGRATIONS.md)
 *    Services/  règles de gestion ; ServiceCrud<T> fournit le CRUD générique
 *    Vues/      fenêtres WinForms ; ModuleBase et FormEditionBase factorisent l'interface
 * =====================================================================================
 */

using System;
using System.Windows.Forms;
using GestionRH.Donnees;
using GestionRH.Vues;

namespace GestionRH
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                InitialiseurBase.Initialiser();   // applique les migrations + données de démonstration
            }
            catch (Exception ex)
            {
                MessageBox.Show("Impossible d'initialiser la base de données :\n" + ex.Message,
                                "Gestion RH", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            while (true)
            {
                using (FormConnexion connexion = new FormConnexion())
                {
                    if (connexion.ShowDialog() != DialogResult.OK) return;
                    using (FormPrincipal principal = new FormPrincipal(connexion.UtilisateurConnecte))
                    {
                        principal.ShowDialog();
                        if (!principal.Deconnexion) return;
                    }
                }
            }
        }
    }
}
