using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NUnit.Framework;

namespace KampfrichterZuweisung.Tests;

[TestFixture]
public class TurnierDatenServiceTests
{
    [TestCase]
    public async Task Test()
    {
        var service = StartUp.ServiceProvider.GetRequiredService<TurnierDatenService>();
        var daten = await service.LadeAsync("2025_24104_Deutsche Meisterschaft Männer gr u. Fr. - Frauen.json");

        TestContext.WriteLine(JsonConvert.SerializeObject(daten, Formatting.Indented));
    }
}