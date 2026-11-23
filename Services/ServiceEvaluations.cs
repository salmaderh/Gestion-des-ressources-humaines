using System.Collections.Generic;
using System.Linq;
using GestionRH.Donnees;
using GestionRH.Modeles;
using Microsoft.EntityFrameworkCore;

namespace GestionRH.Services
{
    public class ServiceEvaluations : ServiceCrud<Evaluation>
    {
        protected override IQueryable<Evaluation> Requete(HrmContext db)
        {
            return db.Evaluations.Include(v => v.Employe).OrderByDescending(v => v.DateEvaluation);
        }

        public List<Evaluation> Lister(int? employeId)
        {
            using (HrmContext db = NouveauContexte())
            {
                IQueryable<Evaluation> q = Requete(db).AsNoTracking();
                if (employeId.HasValue) q = q.Where(v => v.EmployeId == employeId.Value);
                return q.ToList();
            }
        }

        protected override void Valider(Evaluation v, HrmContext db)
        {
            Exiger(v.EmployeId > 0, "Choisissez un employé.");
            Exiger(!string.IsNullOrWhiteSpace(v.Periode), "La période est obligatoire (ex. 2026 ou 2026-S1).");
            Exiger(EstNoteValide(v.NoteCompetences) && EstNoteValide(v.NoteObjectifs) && EstNoteValide(v.NoteComportement),
                   "Les notes doivent être comprises entre 1 et 5.");
            Exiger(!db.Evaluations.Any(x => x.EmployeId == v.EmployeId && x.Periode == v.Periode && x.Id != v.Id),
                   "Cet employé a déjà une évaluation pour la période " + v.Periode + ".");
        }

        private static bool EstNoteValide(int note) => note >= 1 && note <= 5;
    }
}
