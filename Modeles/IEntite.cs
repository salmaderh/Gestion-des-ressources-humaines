namespace GestionRH.Modeles
{
    /// <summary>Toute entité persistée possède un identifiant (clé primaire).</summary>
    public interface IEntite
    {
        int Id { get; set; }
    }
}
