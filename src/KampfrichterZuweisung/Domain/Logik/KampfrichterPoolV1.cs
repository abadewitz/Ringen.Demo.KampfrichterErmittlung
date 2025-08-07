namespace KampfrichterZuweisung.Domain.Logik;

public interface IKampfrichterPool
{
    Kampfrichter? ZieheKampfrichter(int kampfNr, HashSet<string> ausgeschlosseneVerbaende, HashSet<int> ausgeschlosseneKampfrichter);
}

public sealed class KampfrichterPoolV1 : IKampfrichterPool
{
    private readonly ZuordnungsRegeln _regeln;
    private readonly HashSet<Kampfrichter> _pool;
    private readonly Rolle _rolle;
    private readonly Random _random = new();

    public KampfrichterPoolV1(ZuordnungsRegeln regeln, HashSet<Kampfrichter> kampfrichter, Rolle rolle)
    {
        _regeln = regeln;
        _pool = kampfrichter;
        _rolle = rolle;
    }

    public Kampfrichter? ZieheKampfrichter(int kampfNr, HashSet<string> ausgeschlosseneVerbaende, HashSet<int> ausgeschlosseneKampfrichter)
    {
        var kandidaten = _pool
            .Where(k => k.IstVerfuegbar)
            .Where(k => !ausgeschlosseneVerbaende.Contains(k.Verband))
            .Where(k => !ausgeschlosseneKampfrichter.Contains(k.Id))
            .ToList();

        if (!kandidaten.Any())
        {
            return null;
        }

        // zufällig einen auswählen, aber Fairness über Einsatzzähler wahren
        Func<Kampfrichter, int> counterSelector = _rolle switch
        {
            Rolle.Mattenpraesident => k => k.AnzahlEinsaetzeMattenpraesident,
            Rolle.Punktrichter => k => k.AnzahlEinsaetzePunktrichter,
            Rolle.Kampfrichter => k => k.AnzahlEinsaetzeKampfrichter,
            _ => _ => 0
        };

        var minCount = kandidaten.Min(counterSelector);
        var best = kandidaten.Where(k => counterSelector(k) <= minCount + _regeln.MinPulsX).ToList();
        
        var kampfrichter = best[_random.Next(best.Count)];
        if (kampfrichter != null)
        {
            var text = $"{_rolle} zufällig ausgewählt aus {best.Count} Kandidat(en)";
            kampfrichter.View.AuswahlInfo = text;
#if DEBUG
            Console.WriteLine(text);
#endif
        }
        else
        {
            throw new LogikException($"Kein Kampfrichter gefunden für Kampfnr. {kampfNr}");
        }

        return kampfrichter;
    }
}