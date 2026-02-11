using Parkhaus.Models;
using Parkhaus.Services;

namespace Parkhaus.Views;

public partial class EntryPage : ContentPage
{
    private readonly DatabaseService _dbService;

    public EntryPage(DatabaseService dbService)
    {
        InitializeComponent();
        _dbService = dbService;
    }

    private async void OnParkInClicked(object sender, EventArgs e)
    {
        var plate = LicenseEntry.Text?.Trim().ToUpper();

        // VALIDIERUNG (Wichtig für die Note!)
        if (string.IsNullOrWhiteSpace(plate))
        {
            StatusLabel.Text = "Bitte Kennzeichen eingeben!";
            return;
        }

        var newEntry = new ParkingEntry
        {
            LicensePlate = plate,
            EntryTime = DateTime.Now,
            IsActive = true
        };

        await _dbService.SaveEntryAsync(newEntry);

        await DisplayAlert("Erfolg", $"Fahrzeug {plate} eingeparkt!", "OK");

        LicenseEntry.Text = string.Empty;
        StatusLabel.Text = string.Empty;
    }
}