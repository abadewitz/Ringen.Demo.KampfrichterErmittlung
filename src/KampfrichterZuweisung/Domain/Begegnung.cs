using System.ComponentModel;
using Newtonsoft.Json;

namespace KampfrichterZuweisung.Domain;

[JsonObject("Bout")]
public sealed record Begegnung
{
    [JsonProperty("Id")]
    public int KampfNr { get; set; }

    [JsonProperty("Mat")]
    public int Matte { get; set; }

    [JsonProperty("Red")]
    public Ringer Rot { get; set; }

    [JsonProperty("Blue")]
    public Ringer Blau { get; set; }

    //REFEREEING BODY = Kampfgericht

    [JsonProperty("Referee")]
    public Kampfrichter? Kampfrichter { get; set; }

    [JsonProperty("Judge")]
    public Kampfrichter? Punktrichter { get; set; }

    [JsonProperty("MatChairman")]
    public Kampfrichter? Mattenpraesident { get; set; }

    [JsonIgnore]
    public bool IstNeutral => Rot.Verband.Equals(Blau.Verband, StringComparison.OrdinalIgnoreCase);
}

[JsonObject("Wrestler")]
public sealed record Ringer
{
    [JsonProperty("Id")]
    public int Id { get; set; }

    [JsonProperty("Org")]
    public string Verband { get; set; }
}

public enum Rolle
{
    Unklar,

    [Description("MP")]
    Mattenpraesident,

    [Description("PR")]
    Punktrichter,

    [Description("KR")]
    Kampfrichter
}

[JsonObject("Referee")]
public sealed record Kampfrichter
{
    [JsonProperty("Id")]
    public int Id { get; set; }

    [JsonProperty("Org")]
    public string Verband { get; set; } = string.Empty;

    [JsonProperty("Approvals")]
    public HashSet<Rolle> Einsatzfreigaben { get; set; } = new HashSet<Rolle>()
    {
    };

    [JsonIgnore]
    public HashSet<string> AusgeschlosseneVerbaende { get; set; } = new();

    [JsonIgnore]
    public HashSet<int> AusgeschlosseneRinger { get; set; } = new();

    [JsonIgnore]
    public int AnzahlEinsaetzeMattenpraesident { get; set; } = 0;

    [JsonIgnore]
    public int AnzahlEinsaetzePunktrichter { get; set; } = 0;
    
    [JsonIgnore]
    public int AnzahlEinsaetzeKampfrichter { get; set; } = 0;


    [JsonIgnore]
    public int AnzahlEinsaetzeMattenpraesident_Real { get; set; } = 0;

    [JsonIgnore]
    public int AnzahlEinsaetzePunktrichter_Real { get; set; } = 0;

    [JsonIgnore]
    public int AnzahlEinsaetzeKampfrichter_Real { get; set; } = 0;

    [JsonIgnore]
    public int AnzahlEinsaetze => AnzahlEinsaetzeMattenpraesident + AnzahlEinsaetzePunktrichter + AnzahlEinsaetzeKampfrichter;

    [JsonIgnore]
    public int AnzahlEinsaetze_Real => AnzahlEinsaetzeMattenpraesident_Real + AnzahlEinsaetzePunktrichter_Real + AnzahlEinsaetzeKampfrichter_Real;

    [JsonIgnore]
    public int? AktuelleMatte { get; set; }

    [JsonIgnore]
    public Rolle? AktuelleRolle { get; set; }

    [JsonIgnore]
    public bool IstVerfuegbar { get; set; } = true;

    public ViewData View { get; } = new ViewData();
    public class ViewData
    {
        public string AuswahlInfo { get; set; } = string.Empty;
    }
}