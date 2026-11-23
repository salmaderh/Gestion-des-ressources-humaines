using GestionRH.Donnees;

namespace GestionRH.Services
{
    /// <summary>
    /// Classe de base des services : chaque opération ouvre son propre contexte EF Core
    /// (courte durée de vie, recommandé pour une application de bureau).
    /// </summary>
    public abstract class ServiceBase
    {
        protected HrmContext NouveauContexte()
        {
            return new HrmContext();
        }

        protected static void Exiger(bool condition, string message)
        {
            if (!condition) throw new RegleMetierException(message);
        }
    }
}
