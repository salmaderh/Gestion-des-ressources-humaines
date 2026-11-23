# Gestion des Ressources Humaines (GRH) — C#.NET · Entity Framework Core · SQLite

Application de bureau **Windows Forms (.NET 8)** pour la gestion des ressources humaines :
**congés, paie, évaluations, recrutement**, ainsi que les employés et les départements.
Toutes les données sont stockées dans une base **SQLite** manipulée exclusivement avec
**Entity Framework Core**, en approche **Code First** : les classes C# du modèle génèrent la base
grâce aux **migrations** (aucune requête SQL écrite à la main).

## Objectifs du mini-projet

| Objectif | Contenu | Où le trouver |
|---|---|---|
| 1. Compte et dépôt GitHub | Dépôt public contenant tout le projet | ce dépôt |
| 2. Conception orientée objet | Diagramme de cas d'utilisation, diagrammes de classes, classes C# conformes | [`docs/CONCEPTION.md`](docs/CONCEPTION.md), `Modeles/`, `Services/`, `Vues/` |
| 3. Base de données | Entity Framework Core Code First, migrations, base SQLite, interfaces CRUD | [`docs/MIGRATIONS.md`](docs/MIGRATIONS.md), `Donnees/`, `Migrations/`, `gestion_rh.db` |

**Réalisé par :** Salma Derhmi

## Fonctionnalités

| Module | Ce que l'on peut faire |
|---|---|
| Connexion | Authentification (mot de passe haché PBKDF2), rôles Administrateur / Gestionnaire RH |
| Tableau de bord | Indicateurs (effectif, congés en attente, masse salariale, offres...) et validation rapide des congés |
| Employés | Ajouter, modifier, supprimer, rechercher, filtrer par département ; matricule automatique |
| Départements | CRUD avec effectif de chaque département |
| Congés | Demandes (annuel, maladie, maternité, sans solde...), contrôle du solde et des chevauchements, approbation / refus |
| Paie | Génération d'une fiche ou de tout le mois, retenue automatique des congés sans solde, bulletin exportable |
| Évaluations | Trois critères notés de 1 à 5, note globale et appréciation calculées |
| Recrutement | Offres, candidatures, entretiens, **embauche** (crée l'employé automatiquement) |
| Utilisateurs | Gestion des comptes (administrateur uniquement) |

## Technologies

* C# / .NET 8 — Windows Forms (`net8.0-windows`)
* Entity Framework Core 8 — `Microsoft.EntityFrameworkCore.Sqlite`
* Approche **Code First** : schéma créé et mis à jour par les migrations (`Migrations/`)
* Base SQLite : fichier `gestion_rh.db` dans le dossier du projet (versionné sur GitHub)

## Lancer le projet

Prérequis : Windows + **Visual Studio 2022** (charge de travail « Développement .NET Desktop »)
ou le **SDK .NET 8**.

Si le dossier `Migrations/` et `gestion_rh.db` sont déjà présents, il suffit d'ouvrir `GestionRH.sln`
et d'appuyer sur **F5**. Sinon, créer d'abord la base dans le dossier du projet :

```bash
dotnet tool install --global dotnet-ef --version 8.*   # une fois par machine
dotnet ef migrations add InitialCreate                   # génère la migration depuis le modèle
dotnet ef database update                                # crée gestion_rh.db
dotnet run                                               # ou F5 dans Visual Studio
```

Détails, évolution du schéma et dépannage : **[docs/MIGRATIONS.md](docs/MIGRATIONS.md)**.

Comptes de démonstration :

| Utilisateur | Mot de passe | Rôle |
|---|---|---|
| `admin` | `admin123` | Administrateur |
| `rh` | `rh123` | Gestionnaire RH |

Pour repartir d'une base neuve : `dotnet ef database drop` puis `dotnet ef database update`.

## Structure

```
Program.cs            Point d'entrée : initialise la base, affiche la connexion puis la fenêtre principale
Modeles/              Entités EF Core (Employe, DemandeConge, FichePaie, Evaluation, OffreEmploi...)
Donnees/              HrmContext (DbContext SQLite) + InitialiseurBase (migrations + données de démo)
Migrations/           Migrations EF Core générées par « dotnet ef migrations add »
gestion_rh.db         Base SQLite générée par les migrations
Services/             Règles de gestion ; ServiceCrud<T> = CRUD générique Entity Framework
Vues/                 Interfaces Windows Forms
  Commun/             ModuleBase, FormEditionBase, Theme, ElementListe
  Modules/            Écrans : tableau de bord, employés, congés, paie, évaluations, recrutement...
  Dialogues/          Fenêtres de saisie (employé, demande de congé, fiche de paie, offre...)
docs/                 Diagrammes UML et document de conception
```

## Conception

Diagramme de cas d'utilisation, diagrammes de classes et principes orientés objet :
**[docs/CONCEPTION.md](docs/CONCEPTION.md)**.

## Calcul de la paie (simplifié)

```
brut        = salaire de base + primes + heures sup. − retenue congé sans solde
cotisations = brut × 9,18 %
impôt       = (brut − cotisations) × 15 %     (taux simplifié, modifiable dans ServicePaie)
net         = brut − cotisations − impôt
```
