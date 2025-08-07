using KampfrichterZuweisung.Domain;
using Newtonsoft.Json;

public class TurnierDatenService
{
    private readonly string _dataFilePath;

    public TurnierDatenService(string basePath)
    {
        _dataFilePath = Path.Combine(basePath, "Daten");
    }

    public string GetDatenpfad => _dataFilePath;

    public async Task<Turnier> LadeAsync(string dateiname)
    {
        var pfad = Path.Combine(_dataFilePath, dateiname);

        if (!File.Exists(pfad))
        {
            return new Turnier { Begegnungen = new List<Begegnung>() };
        }

        var json = await File.ReadAllTextAsync(pfad);

        return JsonConvert.DeserializeObject<Turnier>(json)
               ?? new Turnier { Begegnungen = new List<Begegnung>() };
    }

}