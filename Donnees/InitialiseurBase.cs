using System;
using System.Collections.Generic;
using System.Linq;
using GestionRH.Modeles;
using GestionRH.Services;
using Microsoft.EntityFrameworkCore;

namespace GestionRH.Donnees
{
    /// <summary>
    /// Applique les migrations Entity Framework (Code First) puis insère des données de démonstration
    /// si la base est vide.
    /// </summary>
    public static class InitialiseurBase
    {
        public static void Initialiser()
        {
            using (HrmContext db = new HrmContext())
            {
                if (!db.Database.GetMigrations().Any())
                    throw new InvalidOperationException(
                        "Aucune migration n'a été trouvée dans le projet.\n\n" +
                        "Créez la migration initiale puis la base, dans le dossier du projet :\n" +
                        "   dotnet ef migrations add InitialCreate\n" +
                        "   dotnet ef database update\n\n" +
                        "(ou, dans Visual Studio : Add-Migration InitialCreate puis Update-Database).");

                // Crée la base si besoin et applique les migrations qui ne le sont pas encore.
                db.Database.Migrate();

                if (!db.Utilisateurs.Any())
                {
                    db.Utilisateurs.Add(ServiceAuthentification.CreerCompte(
                        "admin", "admin123", "Administrateur", RoleUtilisateur.Administrateur));
                    db.Utilisateurs.Add(ServiceAuthentification.CreerCompte(
                        "rh", "rh1234", "Gestionnaire RH", RoleUtilisateur.GestionnaireRH));
                    db.SaveChanges();
                }

                if (!db.Departements.Any())
                    InsererDonneesDemo(db);
            }
        }

        private static void InsererDonneesDemo(HrmContext db)
        {
            Departement rh = new Departement { Nom = "Ressources humaines", Description = "Gestion du personnel" };
            Departement info = new Departement { Nom = "Informatique", Description = "Développement et support" };
            Departement fin = new Departement { Nom = "Finance", Description = "Comptabilité et contrôle de gestion" };
            Departement com = new Departement { Nom = "Commercial", Description = "Ventes et relation client" };
            db.Departements.AddRange(rh, info, fin, com);

            DateTime aujourdhui = DateTime.Today;
            List<Employe> employes = new List<Employe>
            {
                NouvelEmploye("EMP001", "Trabelsi", "Amira", "Responsable RH", rh, 3200, new DateTime(2018, 3, 1)),
                NouvelEmploye("EMP002", "Gharbi", "Youssef", "Développeur .NET", info, 2800, new DateTime(2020, 9, 15)),
                NouvelEmploye("EMP003", "Hammami", "Ines", "Cheffe de projet", info, 3500, new DateTime(2017, 1, 10)),
                NouvelEmploye("EMP004", "Mansouri", "Karim", "Comptable", fin, 2400, new DateTime(2021, 5, 3)),
                NouvelEmploye("EMP005", "Chaabane", "Rim", "Commerciale", com, 2100, new DateTime(2022, 2, 1)),
                NouvelEmploye("EMP006", "Saidi", "Walid", "Technicien support", info, 1900, new DateTime(2023, 6, 12)),
                NouvelEmploye("EMP007", "Khelifi", "Nour", "Chargée de recrutement", rh, 2300, new DateTime(2019, 11, 4)),
                NouvelEmploye("EMP008", "Jaziri", "Mehdi", "Contrôleur de gestion", fin, 2900, new DateTime(2016, 4, 18))
            };
            db.Employes.AddRange(employes);

            db.DemandesConge.AddRange(
                new DemandeConge { Employe = employes[1], Type = TypeConge.Annuel, DateDebut = aujourdhui.AddDays(10),
                                   DateFin = aujourdhui.AddDays(14), Motif = "Vacances" },
                new DemandeConge { Employe = employes[4], Type = TypeConge.Maladie, DateDebut = aujourdhui.AddDays(-3),
                                   DateFin = aujourdhui.AddDays(-1), Motif = "Certificat médical",
                                   Statut = StatutDemande.Approuvee, CommentaireDecision = "OK" },
                new DemandeConge { Employe = employes[5], Type = TypeConge.Annuel, DateDebut = aujourdhui.AddDays(20),
                                   DateFin = aujourdhui.AddDays(24), Motif = "Voyage" });

            db.Evaluations.AddRange(
                new Evaluation { Employe = employes[1], Periode = (aujourdhui.Year - 1).ToString(), Evaluateur = "Ines Hammami",
                                 NoteCompetences = 4, NoteObjectifs = 4, NoteComportement = 5, Commentaire = "Très bon travail." },
                new Evaluation { Employe = employes[3], Periode = (aujourdhui.Year - 1).ToString(), Evaluateur = "Mehdi Jaziri",
                                 NoteCompetences = 3, NoteObjectifs = 3, NoteComportement = 4, Commentaire = "Bonne progression." },
                new Evaluation { Employe = employes[5], Periode = (aujourdhui.Year - 1).ToString(), Evaluateur = "Ines Hammami",
                                 NoteCompetences = 3, NoteObjectifs = 2, NoteComportement = 3, Commentaire = "Objectifs à clarifier." });

            OffreEmploi offre1 = new OffreEmploi { Titre = "Développeur full-stack", Departement = info, TypeContrat = "CDI",
                Description = "C#, ASP.NET Core, SQL", DatePublication = aujourdhui.AddDays(-10), DateLimite = aujourdhui.AddDays(20) };
            OffreEmploi offre2 = new OffreEmploi { Titre = "Assistant(e) comptable", Departement = fin, TypeContrat = "CDD",
                Description = "Saisie comptable, rapprochements", DatePublication = aujourdhui.AddDays(-5), DateLimite = aujourdhui.AddDays(25) };
            db.OffresEmploi.AddRange(offre1, offre2);

            db.Candidatures.AddRange(
                new Candidature { OffreEmploi = offre1, Nom = "Ferchichi", Prenom = "Sarra", Email = "sarra.f@mail.tn", Statut = StatutCandidature.Entretien, NoteEntretien = 15 },
                new Candidature { OffreEmploi = offre1, Nom = "Belhadj", Prenom = "Omar", Email = "omar.b@mail.tn" },
                new Candidature { OffreEmploi = offre2, Nom = "Mejri", Prenom = "Hela", Email = "hela.m@mail.tn" });

            // Fiches de paie du mois précédent
            DateTime moisPrecedent = aujourdhui.AddMonths(-1);
            foreach (Employe e in employes)
                db.FichesPaie.Add(ServicePaie.Calculer(e, moisPrecedent.Month, moisPrecedent.Year, 0m, 0m, 0));

            db.SaveChanges();
        }

        private static Employe NouvelEmploye(string matricule, string nom, string prenom, string poste,
                                             Departement dep, decimal salaire, DateTime embauche)
        {
            return new Employe
            {
                Matricule = matricule, Nom = nom, Prenom = prenom, Poste = poste, Departement = dep,
                SalaireBase = salaire, DateEmbauche = embauche,
                Email = (prenom + "." + nom).ToLowerInvariant() + "@entreprise.tn"
            };
        }
    }
}
