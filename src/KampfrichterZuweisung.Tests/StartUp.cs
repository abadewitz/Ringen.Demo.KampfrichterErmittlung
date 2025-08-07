using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace KampfrichterZuweisung.Tests;

[SetUpFixture]
internal class StartUp
{
    public static bool IstInitialisiert = false;
    private static readonly object _lock = new object();
    public static ServiceProvider ServiceProvider;
    public static string RootDir;

    [OneTimeSetUp]
    public async Task Init()
    {

        lock (_lock)
        {
            if (IstInitialisiert == false)
            {
                var testDir = Path.GetDirectoryName(TestContext.CurrentContext.TestDirectory);
                RootDir = Path.Combine(testDir, "net9.0");

                var services = new ServiceCollection();
                services.AddSingleton<TurnierDatenService>(_ => new TurnierDatenService(RootDir));

                services.AddLogging();
                services.AddLocalization(options => { options.ResourcesPath = "Resources"; });


                var builder = services.BuildServiceProvider();
                ServiceProvider = builder;

                IstInitialisiert = true;
            }
        }
    }
}
