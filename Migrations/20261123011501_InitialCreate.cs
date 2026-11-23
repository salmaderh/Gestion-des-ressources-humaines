using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionRH.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Departements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Utilisateurs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NomUtilisateur = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    NomAffiche = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    MotDePasseHash = table.Column<string>(type: "TEXT", nullable: false),
                    Sel = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilisateurs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Matricule = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Nom = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    Prenom = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    Telephone = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    Poste = table.Column<string>(type: "TEXT", maxLength: 80, nullable: true),
                    DateEmbauche = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SalaireBase = table.Column<double>(type: "REAL", nullable: false),
                    SoldeConges = table.Column<double>(type: "REAL", nullable: false),
                    Statut = table.Column<int>(type: "INTEGER", nullable: false),
                    DepartementId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employes_Departements_DepartementId",
                        column: x => x.DepartementId,
                        principalTable: "Departements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "OffresEmploi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Titre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    TypeContrat = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    DepartementId = table.Column<int>(type: "INTEGER", nullable: true),
                    DatePublication = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateLimite = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Statut = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OffresEmploi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OffresEmploi_Departements_DepartementId",
                        column: x => x.DepartementId,
                        principalTable: "Departements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "DemandesConge",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    DateDebut = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateFin = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Motif = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true),
                    Statut = table.Column<int>(type: "INTEGER", nullable: false),
                    DateDemande = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CommentaireDecision = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DemandesConge", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DemandesConge_Employes_EmployeId",
                        column: x => x.EmployeId,
                        principalTable: "Employes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Evaluations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Periode = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    DateEvaluation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Evaluateur = table.Column<string>(type: "TEXT", maxLength: 80, nullable: true),
                    NoteCompetences = table.Column<int>(type: "INTEGER", nullable: false),
                    NoteObjectifs = table.Column<int>(type: "INTEGER", nullable: false),
                    NoteComportement = table.Column<int>(type: "INTEGER", nullable: false),
                    ObjectifsSuivants = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    Commentaire = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evaluations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Evaluations_Employes_EmployeId",
                        column: x => x.EmployeId,
                        principalTable: "Employes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FichesPaie",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmployeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Mois = table.Column<int>(type: "INTEGER", nullable: false),
                    Annee = table.Column<int>(type: "INTEGER", nullable: false),
                    SalaireBase = table.Column<double>(type: "REAL", nullable: false),
                    Primes = table.Column<double>(type: "REAL", nullable: false),
                    HeuresSupplementaires = table.Column<double>(type: "REAL", nullable: false),
                    JoursSansSolde = table.Column<int>(type: "INTEGER", nullable: false),
                    RetenueAbsences = table.Column<double>(type: "REAL", nullable: false),
                    SalaireBrut = table.Column<double>(type: "REAL", nullable: false),
                    CotisationsSociales = table.Column<double>(type: "REAL", nullable: false),
                    ImpotRevenu = table.Column<double>(type: "REAL", nullable: false),
                    SalaireNet = table.Column<double>(type: "REAL", nullable: false),
                    DateGeneration = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FichesPaie", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FichesPaie_Employes_EmployeId",
                        column: x => x.EmployeId,
                        principalTable: "Employes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Candidatures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OffreEmploiId = table.Column<int>(type: "INTEGER", nullable: false),
                    Nom = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    Prenom = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    Telephone = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    DateCandidature = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Statut = table.Column<int>(type: "INTEGER", nullable: false),
                    NoteEntretien = table.Column<int>(type: "INTEGER", nullable: true),
                    Commentaire = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Candidatures_OffresEmploi_OffreEmploiId",
                        column: x => x.OffreEmploiId,
                        principalTable: "OffresEmploi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Candidatures_OffreEmploiId",
                table: "Candidatures",
                column: "OffreEmploiId");

            migrationBuilder.CreateIndex(
                name: "IX_DemandesConge_EmployeId",
                table: "DemandesConge",
                column: "EmployeId");

            migrationBuilder.CreateIndex(
                name: "IX_Departements_Nom",
                table: "Departements",
                column: "Nom",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employes_DepartementId",
                table: "Employes",
                column: "DepartementId");

            migrationBuilder.CreateIndex(
                name: "IX_Employes_Matricule",
                table: "Employes",
                column: "Matricule",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Evaluations_EmployeId",
                table: "Evaluations",
                column: "EmployeId");

            migrationBuilder.CreateIndex(
                name: "IX_FichesPaie_EmployeId_Annee_Mois",
                table: "FichesPaie",
                columns: new[] { "EmployeId", "Annee", "Mois" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OffresEmploi_DepartementId",
                table: "OffresEmploi",
                column: "DepartementId");

            migrationBuilder.CreateIndex(
                name: "IX_Utilisateurs_NomUtilisateur",
                table: "Utilisateurs",
                column: "NomUtilisateur",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Candidatures");

            migrationBuilder.DropTable(
                name: "DemandesConge");

            migrationBuilder.DropTable(
                name: "Evaluations");

            migrationBuilder.DropTable(
                name: "FichesPaie");

            migrationBuilder.DropTable(
                name: "Utilisateurs");

            migrationBuilder.DropTable(
                name: "OffresEmploi");

            migrationBuilder.DropTable(
                name: "Employes");

            migrationBuilder.DropTable(
                name: "Departements");
        }
    }
}
