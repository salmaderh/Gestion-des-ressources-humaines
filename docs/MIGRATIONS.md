# Base de données : Entity Framework Core — approche Code First et migrations

## 1. Principe

Avec l'approche **Code First**, on n'écrit pas de SQL : on décrit le modèle en C#, et Entity Framework
génère la base.

| Élément | Où ? | Rôle |
|---|---|---|
| Classes du modèle | `Modeles/` (`Employe`, `DemandeConge`, `FichePaie`...) | Chaque classe devient une **table**, chaque propriété une **colonne** |
| Annotations | `[Required]`, `[MaxLength(60)]`, `[NotMapped]` | `NOT NULL`, longueur, propriété non stockée |
| Contexte | `Donnees/HrmContext.cs` | Un `DbSet<T>` par table ; `OnModelCreating` déclare les relations, index uniques, suppressions en cascade |
| Migrations | `Migrations/` (généré) | Classes C# qui décrivent comment passer d'une version du schéma à la suivante |
| Historique | table `__EFMigrationsHistory` | Liste des migrations déjà appliquées à la base |
| Base | `gestion_rh.db` (dossier du projet) | Fichier SQLite, versionné sur GitHub |

Au démarrage, l'application appelle `db.Database.Migrate()` (`Donnees/InitialiseurBase.cs`) :
les migrations pas encore appliquées le sont automatiquement, puis des données de démonstration
sont insérées si la base est vide.

## 2. Créer la base (une seule fois)

Ouvrir un terminal **dans le dossier du projet** (celui qui contient `GestionRH.csproj`).

```bash
# Outil en ligne de commande d'EF Core (une fois par machine)
dotnet tool install --global dotnet-ef --version 8.*

# 1) Générer la migration initiale à partir des classes du modèle
dotnet ef migrations add InitialCreate

# 2) Créer la base gestion_rh.db et ses tables
dotnet ef database update
```

Équivalent dans Visual Studio : **Outils → Gestionnaire de package NuGet → Console du Gestionnaire de package**

```powershell
Add-Migration InitialCreate
Update-Database
```

Le dossier `Migrations/` contient alors trois fichiers :

* `xxxx_InitialCreate.cs` : méthodes `Up()` (crée les tables) et `Down()` (les supprime) ;
* `xxxx_InitialCreate.Designer.cs` : état du modèle à ce moment ;
* `HrmContextModelSnapshot.cs` : état actuel du modèle, comparé à chaque nouvelle migration.

Lancer ensuite l'application (F5) : les données de démonstration sont ajoutées au premier démarrage.

## 3. Faire évoluer la base à partir du code (exemple : ajout d'une colonne)

Objectif : ajouter l'**adresse** d'un employé.

**a) Modifier le modèle** — `Modeles/Employe.cs`, sous `Telephone` :

```csharp
[MaxLength(200)]
public string Adresse { get; set; }
```

**b) Ajouter le champ dans l'interface** — `Vues/Dialogues/FormEmploye.cs` :

```csharp
// 1. déclaration (avec les autres TextBox)
private readonly TextBox adresse;

// 2. dans le constructeur, après « telephone = ... »
adresse = AjouterChamp("Adresse", Texte(employe.Adresse));

// 3. dans Enregistrer(), après « employe.Telephone = ... »
employe.Adresse = adresse.Text.Trim();
```

**c) Créer et appliquer la migration** :

```bash
dotnet ef migrations add AjoutAdresseEmploye
dotnet ef database update
```

Le fichier généré contient seulement le changement :

```csharp
migrationBuilder.AddColumn<string>(
    name: "Adresse", table: "Employes", type: "TEXT", maxLength: 200, nullable: true);
```

Les données existantes sont conservées. On peut vérifier avec `dotnet ef migrations list`.

## 4. Commandes utiles

| Commande | Effet |
|---|---|
| `dotnet ef migrations list` | Liste les migrations et indique celles qui sont appliquées |
| `dotnet ef migrations remove` | Supprime la dernière migration **si elle n'a pas été appliquée** |
| `dotnet ef database update InitialCreate` | Revient à une migration précédente (exécute les `Down()`) |
| `dotnet ef migrations script` | Affiche le SQL généré par les migrations |
| `dotnet ef database drop` | Supprime la base (elle sera recréée par `database update`) |

## 5. Problèmes fréquents

| Message | Solution |
|---|---|
| « Aucune migration n'a été trouvée » au lancement | Exécuter les deux commandes de la section 2 |
| `table "Departements" already exists` | La base a été créée par une ancienne version sans migrations : supprimer `gestion_rh.db` puis `dotnet ef database update` |
| `dotnet ef` introuvable | `dotnet tool install --global dotnet-ef --version 8.*` puis rouvrir le terminal |
| `Build failed` | Corriger d'abord les erreurs de compilation (la commande compile le projet) |
| `database is locked` | Fermer l'application avant `dotnet ef database update` |

## 6. Mettre la base sur GitHub

La base `gestion_rh.db` est volontairement **versionnée** (exigence du livrable) ; seuls ses fichiers
temporaires `-wal` / `-shm` sont ignorés. Fermer l'application avant de faire le commit :

```bash
git add Migrations gestion_rh.db
git commit -m "Base SQLite générée par les migrations EF Core"
git push
```
