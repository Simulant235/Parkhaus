using Parkhaus.Models;
using Parkhaus.Services;
using Parkhaus.Helper;

namespace Parkhaus.Views;

public partial class ExitPage : ContentPage
{
    private readonly DatabaseService _dbService;

    public ExitPage(DatabaseService dbService)
    {
        InitializeComponent();
        _dbService = dbService;
    }

    private async void OnExitClicked(object sender, EventArgs e)
    {
        try
        {
            // Eingabe validieren
            string input = LicenseEntry.Text;
            string plate = LicensePlateValidator.Normalize(input);

            if (!LicensePlateValidator.IsValid(plate))
            {
                StatusLabel.Text = "Ungültiges Kennzeichen!";
                StatusLabel.TextColor = Colors.Red;
                return;
            }

            // Auto suchen (ist es überhaupt im Parkhaus?)
            var entry = await _dbService.GetEntryStillInsideAsync(plate);

            if (entry == null)
            {
                // Nicht gefunden oder schon draußen
                StatusLabel.Text = "Fahrzeug ist nicht im Parkhaus!";
                StatusLabel.TextColor = Colors.Red;
                return;
            }

            // Prüfen ob bezahlt wurde
            if (entry.IsActive)
            {
                // Noch nicht bezahlt!
                StatusLabel.Text = "Bitte zuerst bezahlen!";
                StatusLabel.TextColor = Colors.Orange;
                return;
            }

            // Ausfahrt erlauben
            entry.IsStillInside = false;
            await _dbService.SaveEntryAsync(entry);

            // Erfolg
            await DisplayAlertAsync("Ausfahrt", $"Gute Fahrt!\n\nFahrzeug {plate} hat das Parkhaus verlassen.", "OK");

            // Formular zurücksetzen
            LicenseEntry.Text = string.Empty;
            StatusLabel.Text = string.Empty;
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Fehler", "Datenbankfehler", "OK");
        }
    }
}
