using System;
using System.IO;
using GestionRH.Modeles;
using Microsoft.EntityFrameworkCore;

namespace GestionRH.Donnees
{
    /// <summary>
    /// Contexte Entity Framework Core (approche Code First) : une table SQLite par DbSet.
    /// Le schéma est créé et mis à jour par les migrations (dossier Migrations/).
    /// La base « gestion_rh.db » est placée dans le dossier du projet (à côté de GestionRH.csproj)
    /// pour être versionnée sur GitHub ; « dotnet ef » et l'application utilisent donc le même fichier.
    /// </summary>
    public class HrmContext : DbContext
    {
        public const string NomFichierBase = "gestion_rh.db";

        public static string CheminBase { get; set; } = TrouverCheminBase();

        /// <summary>Remonte depuis bin/Debug/... jusqu'au dossier contenant GestionRH.csproj.</summary>
        private static string TrouverCheminBase()
        {
            DirectoryInfo dossier = new DirectoryInfo(AppContext.BaseDirectory);
            while (dossier != null)
            {
                if (File.Exists(Path.Combine(dossier.FullName, "GestionRH.csproj")))
                    return Path.Combine(dossier.FullName, NomFichierBase);
                dossier = dossier.Parent;
            }
            return Path.Combine(AppContext.BaseDirectory, NomFichierBase);   // application déployée
        }

        public DbSet<Departement> Departements { get; set; }
        public DbSet<Employe> Employes { get; set; }
        public DbSet<DemandeConge> DemandesConge { get; set; }
        public DbSet<FichePaie> FichesPaie { get; set; }
        public DbSet<Evaluation> Evaluations { get; set; }
        public DbSet<OffreEmploi> OffresEmploi { get; set; }
        public DbSet<Candidature> Candidatures { get; set; }
        public DbSet<Utilisateur> Utilisateurs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
                // Pooling=False : chaque connexion est réellement fermée, SQLite recopie alors son
                // journal (-wal) dans gestion_rh.db, qui contient donc toujours toutes les données.
                options.UseSqlite("Data Source=" + CheminBase + ";Pooling=False");
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configuration)
        {
            // SQLite n'a pas de type décimal natif : on stocke les montants en REAL
            // pour pouvoir trier / additionner directement en SQL.
            configuration.Properties<decimal>().HaveConversion<double>();
        }

        protected override void OnModelCreating(ModelBuilder modele)
        {
            modele.Entity<Departement>().HasIndex(d => d.Nom).IsUnique();

            modele.Entity<Employe>().HasIndex(e => e.Matricule).IsUnique();
            modele.Entity<Employe>()
                  .HasOne(e => e.Departement).WithMany(d => d.Employes)
                  .HasForeignKey(e => e.DepartementId).OnDelete(DeleteBehavior.SetNull);

            modele.Entity<DemandeConge>()
                  .HasOne(c => c.Employe).WithMany(e => e.Conges)
                  .HasForeignKey(c => c.EmployeId).OnDelete(DeleteBehavior.Cascade);

            modele.Entity<FichePaie>()
                  .HasOne(f => f.Employe).WithMany(e => e.FichesPaie)
                  .HasForeignKey(f => f.EmployeId).OnDelete(DeleteBehavior.Cascade);
            modele.Entity<FichePaie>()
                  .HasIndex(f => new { f.EmployeId, f.Annee, f.Mois }).IsUnique();   // une fiche par mois

            modele.Entity<Evaluation>()
                  .HasOne(v => v.Employe).WithMany(e => e.Evaluations)
                  .HasForeignKey(v => v.EmployeId).OnDelete(DeleteBehavior.Cascade);

            modele.Entity<OffreEmploi>()
                  .HasOne(o => o.Departement).WithMany(d => d.Offres)
                  .HasForeignKey(o => o.DepartementId).OnDelete(DeleteBehavior.SetNull);

            modele.Entity<Candidature>()
                  .HasOne(c => c.OffreEmploi).WithMany(o => o.Candidatures)
                  .HasForeignKey(c => c.OffreEmploiId).OnDelete(DeleteBehavior.Cascade);

            modele.Entity<Utilisateur>().HasIndex(u => u.NomUtilisateur).IsUnique();
        }
    }
}
