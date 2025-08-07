using KampfrichterZuweisung.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddCircuitOptions(options => { options.DetailedErrors = true; })
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddSingleton<TurnierDatenService>(_ => new TurnierDatenService(builder.Environment.ContentRootPath));
var app = builder.Build();

app.UseWebAssemblyDebugging();
app.UseDeveloperExceptionPage(); 
app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    
    .AddAdditionalAssemblies(typeof(KampfrichterZuweisung.Client._Imports).Assembly);

app.Run();