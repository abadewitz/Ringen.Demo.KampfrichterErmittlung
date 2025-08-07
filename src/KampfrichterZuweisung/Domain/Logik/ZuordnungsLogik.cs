namespace KampfrichterZuweisung.Domain.Logik;

public sealed class ZuordnungsLogik
{
    public HashSet<Kampfrichter> AlleKampfrichter { get; }
    public List<Begegnung> Kaempfe { get; } = new();

    private IKampfrichterPool _mattenpraesidentPool;
    private IKampfrichterPool _punktrichterPool;
    private IKampfrichterPool _kampfrichterPool;
    private ZuordnungsRegeln _regeln;

    public ZuordnungsLogik(HashSet<Kampfrichter> alleKampfrichter, ZuordnungsRegeln regeln)
    {
        AlleKampfrichter = alleKampfrichter ?? new HashSet<Kampfrichter>();
        _regeln = regeln;

        switch (regeln.Version)
        {
            case 1:
                _mattenpraesidentPool = new KampfrichterPoolV1(regeln, AlleKampfrichter.Where(k => k.Einsatzfreigaben.Contains(Rolle.Mattenpraesident)).ToHashSet(), Rolle.Mattenpraesident);
                _punktrichterPool = new KampfrichterPoolV1(regeln, AlleKampfrichter.Where(k => k.Einsatzfreigaben.Contains(Rolle.Punktrichter)).ToHashSet(), Rolle.Punktrichter);
                _kampfrichterPool = new KampfrichterPoolV1(regeln, AlleKampfrichter.Where(k => k.Einsatzfreigaben.Contains(Rolle.Kampfrichter)).ToHashSet(), Rolle.Kampfrichter);
                break;
        }

    }

    public Begegnung AddKampf(Begegnung kampf)
    {
        Kaempfe.Add(kampf);

        return kampf;
    }

    public Begegnung KampfAbschliessen(Begegnung begegnung)
    {
        if (begegnung.Mattenpraesident != null)
        {
            begegnung.Mattenpraesident.AnzahlEinsaetzeMattenpraesident++;

            begegnung.Mattenpraesident.AktuelleRolle = null;
            begegnung.Mattenpraesident.AktuelleMatte = null;
        }

        if (begegnung.Punktrichter != null)
        {
            begegnung.Punktrichter.AnzahlEinsaetzePunktrichter++;
            
            begegnung.Punktrichter.AktuelleRolle = null;
            begegnung.Punktrichter.AktuelleMatte = null;
        }

        if (begegnung.Kampfrichter != null)
        {
            begegnung.Kampfrichter.AnzahlEinsaetzeKampfrichter++;

            begegnung.Kampfrichter.AktuelleRolle = null;
            begegnung.Kampfrichter.AktuelleMatte = null;
        }

        return begegnung;
    }

    private HashSet<string> TryAddVerband(HashSet<string> input, string? verband)
    {
        if (!string.IsNullOrWhiteSpace(verband))
        {
            input.Add(verband);
        }

        return input;
    }

    public Begegnung KampfrichterZuteilen(Begegnung begegnung)
    {
        //https://ringen-kampfrichter.info/ausbildung/kampfrichterausbildung/zusammenarbeit-im-dreimann-kampfgericht/
        var ausgeschlosseneVerbaende = new HashSet<string>();
        var ausgeschlosseneKampfrichter = AlleKampfrichter
            .Where(li => li.AusgeschlosseneVerbaende.Contains(begegnung.Rot.Verband)
                         || li.AusgeschlosseneVerbaende.Contains(begegnung.Blau.Verband)
                         || li.AusgeschlosseneRinger.Contains(begegnung.Rot.Id)
                         || li.AusgeschlosseneRinger.Contains(begegnung.Blau.Id))
            .Select(li => li.Id)
            .ToHashSet();

        if (begegnung.IstNeutral)
        {
            //Sollten beide Ringer aus demselben Landesverband sein kann auch ein Kampfrichter aus diesem Landesverband eingesetzt werden.
            //Das Kampfgericht kann auch komplett aus einem Landesverband kommen.
        }
        else
        {
            //Das Drei-Mann-Kampfgericht muss in seiner Zusammensetzung neutral sein, dies bedeutet dass niemand vom Kampfgericht dem Landesverband  eines Ringers angehören darf.
            if (!string.IsNullOrWhiteSpace(begegnung.Rot?.Verband))
            {
                TryAddVerband(ausgeschlosseneVerbaende, begegnung.Rot.Verband);
            }

            if (!string.IsNullOrWhiteSpace(begegnung.Blau?.Verband))
            {
                TryAddVerband(ausgeschlosseneVerbaende, begegnung.Blau.Verband);
            }
        }

        //Vorgegebene Reihenfolge MP > KR > PR
        //1. Lose Mattenpräsident
        begegnung.Mattenpraesident = _mattenpraesidentPool.ZieheKampfrichter(begegnung.KampfNr, ausgeschlosseneVerbaende, ausgeschlosseneKampfrichter);
        if (begegnung.Mattenpraesident != null)
        {
            ausgeschlosseneKampfrichter.Add(begegnung.Mattenpraesident.Id);

            begegnung.Mattenpraesident.AktuelleRolle = Rolle.Mattenpraesident;
            begegnung.Mattenpraesident.AktuelleMatte = begegnung.Matte;

            if (_regeln.MindestensDreiUnterschiedlicheVerbaendeImKampfgericht)
            {
                TryAddVerband(ausgeschlosseneVerbaende, begegnung.Mattenpraesident?.Verband);
            }
        }

        //2. Lose Kampfrichter
        begegnung.Kampfrichter = _kampfrichterPool.ZieheKampfrichter(begegnung.KampfNr, ausgeschlosseneVerbaende, ausgeschlosseneKampfrichter);
        if (begegnung.Kampfrichter != null)
        {
            ausgeschlosseneKampfrichter.Add(begegnung.Kampfrichter.Id);

            begegnung.Kampfrichter.AktuelleRolle = Rolle.Kampfrichter;
            begegnung.Kampfrichter.AktuelleMatte = begegnung.Matte;

            if (_regeln.MindestensDreiUnterschiedlicheVerbaendeImKampfgericht)
            {
                TryAddVerband(ausgeschlosseneVerbaende, begegnung.Kampfrichter?.Verband);
            }
        }

        //3. Lose Punktrichter
        begegnung.Punktrichter = _punktrichterPool.ZieheKampfrichter(begegnung.KampfNr, ausgeschlosseneVerbaende, ausgeschlosseneKampfrichter);
        if (begegnung.Punktrichter != null)
        {
            ausgeschlosseneKampfrichter.Add(begegnung.Punktrichter.Id);

            begegnung.Punktrichter.AktuelleRolle = Rolle.Punktrichter;
            begegnung.Punktrichter.AktuelleMatte = begegnung.Matte;

            if (_regeln.MindestensDreiUnterschiedlicheVerbaendeImKampfgericht)
            {
                TryAddVerband(ausgeschlosseneVerbaende, begegnung.Punktrichter?.Verband);
            }
        }


        return begegnung;
    }
}