using System;
using System.Collections.Generic;

namespace GestionRH.Modeles
{
    public enum StatutEmploye { Actif, Inactif }

    public enum TypeConge { Annuel, Maladie, Maternite, SansSolde, Exceptionnel }

    public enum StatutDemande { EnAttente, Approuvee, Refusee }

    public enum StatutOffre { Ouverte, Fermee, Pourvue }

    public enum StatutCandidature { Recue, Entretien, Acceptee, Refusee, Embauchee }

    public enum RoleUtilisateur { Administrateur, GestionnaireRH }

    /// <summary>Libellés lisibles (avec accents) pour l'affichage des énumérations.</summary>
    public static class Libelles
    {
        private static readonly Dictionary<string, string> textes = new Dictionary<string, string>
        {
            { "EnAttente", "En attente" }, { "Approuvee", "Approuvée" }, { "Refusee", "Refusée" },
            { "Maternite", "Maternité" }, { "SansSolde", "Sans solde" },
            { "Fermee", "Fermée" }, { "Recue", "Reçue" }, { "Acceptee", "Acceptée" },
            { "Embauchee", "Embauchée" }, { "GestionnaireRH", "Gestionnaire RH" }
        };

        public static string Libelle(this Enum valeur)
        {
            string nom = valeur.ToString();
            string texte;
            return textes.TryGetValue(nom, out texte) ? texte : nom;
        }
    }
}
