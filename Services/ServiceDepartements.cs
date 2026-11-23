using System.Linq;
using GestionRH.Donnees;
using GestionRH.Modeles;
using Microsoft.EntityFrameworkCore;

namespace GestionRH.Services
{
    public class ServiceDepartements : ServiceCrud<Departement>
    {
        protected override IQueryable<Departement> Requete(HrmContext db)
        {
            return db.Departements.Include(d => d.Employes).OrderBy(d => d.Nom);
        }

        protected override void Valider(Departement d, HrmContext db)
        {
            Exiger(!string.IsNullOrWhiteSpace(d.Nom), "Le nom du département est obligatoire.");
            d.Nom = d.Nom.Trim();
            Exiger(!db.Departements.Any(x => x.Nom == d.Nom && x.Id != d.Id), "Ce département existe déjà.");
        }
    }
}
