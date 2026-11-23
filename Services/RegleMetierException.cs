using System;

namespace GestionRH.Services
{
    /// <summary>Violation d'une règle de gestion (message destiné à l'utilisateur).</summary>
    public class RegleMetierException : Exception
    {
        public RegleMetierException(string message) : base(message) { }
    }
}
