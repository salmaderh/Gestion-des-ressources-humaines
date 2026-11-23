using System.Collections.Generic;
using System.Linq;
using GestionRH.Donnees;
using GestionRH.Modeles;
using Microsoft.EntityFrameworkCore;

namespace GestionRH.Services
{
    /// <summary>
    /// Opérations CRUD génériques (Create / Read / Update / Delete) avec Entity Framework.
    /// Les classes filles précisent la requête de lecture et les règles de validation.
    /// </summary>
    public abstract class ServiceCrud<T> : ServiceBase where T : class, IEntite
    {
        /// <summary>Requête de base (inclusions, tri). À redéfinir si besoin.</summary>
        protected virtual IQueryable<T> Requete(HrmContext db)
        {
            return db.Set<T>();
        }

        public virtual List<T> Lister()
        {
            using (HrmContext db = NouveauContexte())
                return Requete(db).AsNoTracking().ToList();
        }

        public T Obtenir(int id)
        {
            using (HrmContext db = NouveauContexte())
                return db.Set<T>().Find(id);
        }

        public virtual void Enregistrer(T entite)
        {
            using (HrmContext db = NouveauContexte())
            {
                Valider(entite, db);
                if (entite.Id == 0) db.Set<T>().Add(entite);
                else db.Set<T>().Update(entite);
                db.SaveChanges();
            }
        }

        public virtual void Supprimer(int id)
        {
            using (HrmContext db = NouveauContexte())
            {
                T entite = db.Set<T>().Find(id);
                if (entite == null) return;
                AvantSuppression(entite, db);
                db.Set<T>().Remove(entite);
                db.SaveChanges();
            }
        }

        /// <summary>Lève une RegleMetierException si l'entité n'est pas valide.</summary>
        protected virtual void Valider(T entite, HrmContext db) { }

        protected virtual void AvantSuppression(T entite, HrmContext db) { }
    }
}
