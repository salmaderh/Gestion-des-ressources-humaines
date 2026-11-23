using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using GestionRH.Modeles;

namespace GestionRH.Vues
{
    /// <summary>Élément de liste déroulante : une valeur (id, énumération...) et le texte affiché.</summary>
    public class ElementListe
    {
        public object Valeur { get; }
        public string Texte { get; }

        public ElementListe(object valeur, string texte)
        {
            Valeur = valeur;
            Texte = texte;
        }

        public override string ToString() => Texte;

        public static IEnumerable<ElementListe> DepuisEnum<T>() where T : struct, Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>().Select(v => new ElementListe(v, ((Enum)(object)v).Libelle()));
        }

        public static void Remplir(ComboBox combo, IEnumerable<ElementListe> elements)
        {
            combo.DropDownStyle = ComboBoxStyle.DropDownList;
            combo.Items.Clear();
            foreach (ElementListe e in elements) combo.Items.Add(e);
            if (combo.Items.Count > 0) combo.SelectedIndex = 0;
        }

        public static void Selectionner(ComboBox combo, object valeur)
        {
            for (int i = 0; i < combo.Items.Count; i++)
            {
                ElementListe e = combo.Items[i] as ElementListe;
                if (e != null && Equals(e.Valeur, valeur)) { combo.SelectedIndex = i; return; }
            }
        }

        public static object ValeurDe(ComboBox combo)
        {
            ElementListe e = combo.SelectedItem as ElementListe;
            return e?.Valeur;
        }
    }
}
