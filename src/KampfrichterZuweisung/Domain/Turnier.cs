namespace KampfrichterZuweisung.Domain;

public sealed record Turnier
{
    public int TId { get; set; }
    public string Bezeichnung { get; set; }
    public List<Begegnung> Begegnungen { get; set; }
}