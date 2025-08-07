namespace KampfrichterZuweisung.Domain.Logik;

public sealed record ZuordnungsRegeln
{
    public int Version { get; set; } = 1;
    public int MinPulsX { get; set; } = 0;
    public bool MindestensDreiUnterschiedlicheVerbaendeImKampfgericht { get; set; } = true;
    public bool MindestensZweiUnterschiedlicheVerbaendeImKampfgericht { get; set; } = false;
    public bool KampfrichterPunktrichter { get; set; } = false; //TODO Verstehe ich noch nicht

    //TODO Auschluss von Kampfrichter-IDs zu Verbänden (persönliche Gründe - ehemaliger kampfrichter von dort oder streit)
    //TODO Auschluss von Kampfrichter-IDs zu Ringer-IDs (persönliche Gründe - ex-mann, etc.)
}