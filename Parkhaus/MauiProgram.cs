using Microsoft.Extensions.Logging;
using Parkhaus.Services;
using Parkhaus.Views; // NEU: Damit er die EntryPage findet

namespace Parkhaus;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Services registrieren (Einmalig für die App)
        builder.Services.AddSingleton<DatabaseService>();

        // Views registrieren (Wichtig für Dependency Injection)
        builder.Services.AddTransient<EntryPage>(); // <--- HIER EINGEFÜGT

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}