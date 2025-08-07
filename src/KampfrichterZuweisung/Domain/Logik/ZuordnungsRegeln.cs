namespace KampfrichterZuweisung.Domain.Logik;

public sealed record ZuordnungsRegeln
{
    public int Version { get; set; } = 1;
    public int MinPulsX { get; set; } = 0;
    public bool MindestensDreiUnterschiedlicheVerbaendeImKampfgericht { get; set; } = true;
    public bool MindestensZweiUnterschiedlicheVerbaendeImKampfgericht { get; set; } = false;
    public bool KampfrichterPunktrichter { get; set; } = false; //TODO Verstehe ich noch nicht

    //TODO Auschluss von Kampfrichter-IDs zu Verb�nden (pers�nliche Gr�nde - ehemaliger kampfrichter von dort oder streit)
    //TODO Auschluss von Kampfrichter-IDs zu Ringer-IDs (pers�nliche Gr�nde - ex-mann, etc.)
}