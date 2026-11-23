using System.Collections.Generic;
using System.Linq;
using GestionRH.Donnees;
using GestionRH.Modeles;
using Microsoft.EntityFrameworkCore;

namespace GestionRH.Services
{
    public class ServiceEmployes : ServiceCrud<Employe>
    {
        protected override IQueryable<Employe> Requete(HrmContext db)
        {
            return db.Employes.Include(e => e.Departement).OrderBy(e => e.Nom).ThenBy(e => e.Prenom);
        }

        /// <summary>Recherche par nom, prénom, matricule ou poste, éventuellement dans un département.</summary>
        public List<Employe> Rechercher(string texte, int? departementId, bool actifsSeulement = false)
        {
            using (HrmContext db = NouveauContexte())
            {
                IQueryable<Employe> q = Requete(db).AsNoTracking();
                if (!string.IsNullOrWhiteSpace(texte))
                {
                    string t = texte.Trim().ToLower();
                    q = q.Where(e => e.Nom.ToLower().Contains(t) || e.Prenom.ToLower().Contains(t) ||
                                     e.Matricule.ToLower().Contains(t) ||
                                     (e.Poste != null && e.Poste.ToLower().Contains(t)));
                }
                if (departementId.HasValue) q = q.Where(e => e.DepartementId == departementId.Value);
                if (actifsSeulement) q = q.Where(e => e.Statut == StatutEmploye.Actif);
                return q.ToList();
            }
        }

        public List<Employe> ListerActifs()
        {
            return Rechercher(null, null, true);
        }

        public string ProchainMatricule()
        {
            using (HrmContext db = NouveauContexte())
                return ProchainMatricule(db);
        }

        /// <summary>Matricule suivant au format EMP001, EMP002...</summary>
        public static string ProchainMatricule(HrmContext db)
        {
            int max = 0;
            foreach (string m in db.Employes.Select(e => e.Matricule).ToList())
            {
                int n;
                if (m != null && m.StartsWith("EMP") && int.TryParse(m.Substring(3), out n) && n > max) max = n;
            }
            return "EMP" + (max + 1).ToString("000");
        }

        protected override void Valider(Employe e, HrmContext db)
        {
            Exiger(!string.IsNullOrWhiteSpace(e.Matricule), "Le matricule est obligatoire.");
            Exiger(!string.IsNullOrWhiteSpace(e.Nom), "Le nom est obligatoire.");
            Exiger(!string.IsNullOrWhiteSpace(e.Prenom), "Le prénom est obligatoire.");
            Exiger(e.SalaireBase >= 0, "Le salaire ne peut pas être négatif.");
            Exiger(e.SoldeConges >= 0, "Le solde de congés ne peut pas être négatif.");
            Exiger(string.IsNullOrWhiteSpace(e.Email) || (e.Email.Contains("@") && e.Email.Contains(".")),
                   "L'adresse e-mail n'est pas valide.");
            Exiger(!db.Employes.Any(x => x.Matricule == e.Matricule && x.Id != e.Id), "Ce matricule est déjà utilisé.");
        }
    }
}
