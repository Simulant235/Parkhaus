using Parkhaus.Models;
using Parkhaus.Services;
using Parkhaus.Helper;

namespace Parkhaus.Views;

public partial class EntryPage : ContentPage
{
    private readonly DatabaseService _dbService;
    private const int MaxCapacity = 20;

    public EntryPage(DatabaseService dbService)
    {
        InitializeComponent();
        _dbService = dbService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await UpdateAvailableSpotsAsync();
    }

    private async Task UpdateAvailableSpotsAsync()
    {
        try
        {
            int activeCount = await _dbService.GetActiveCountAsync();
            int freeSpots = MaxCapacity - activeCount;

            AvailableSpotsLabel.Text = $"{freeSpots} von {MaxCapacity}";
            AvailableSpotsLabel.TextColor = freeSpots > 0 ? Colors.Green : Colors.Red;
        }
        catch (Exception ex)
        {
            AvailableSpotsLabel.Text = "-- von 20";
            AvailableSpotsLabel.TextColor = Colors.Gray;
        }
    }

    private async void OnParkInClicked(object sender, EventArgs e)
    {
        string input = LicenseEntry.Text;
        string plate = LicensePlateValidator.Normalize(input);

        if (!LicensePlateValidator.IsValid(plate))
        {
            StatusLabel.Text = "Ungültiges Kennzeichen!";
            StatusLabel.TextColor = Colors.Red;
            return;
        }

        // Prüfen ob Kennzeichen bereits im Parkhaus ist (physisch drin)
        var existing = await _dbService.GetEntryStillInsideAsync(plate);
        if (existing != null)
        {
            StatusLabel.Text = "Fahrzeug ist bereits im Parkhaus!";
            StatusLabel.TextColor = Colors.Red;
            return;
        }

        // Prüfen ob Parkhaus voll ist
        int activeCount = await _dbService.GetActiveCountAsync();
        if (activeCount >= MaxCapacity)
        {
            StatusLabel.Text = "Parkhaus ist voll!";
            StatusLabel.TextColor = Colors.Red;
            return;
        }

        var newEntry = new ParkingEntry
        {
            LicensePlate = plate,
            EntryTime = DateTime.Now,
            IsActive = true,
            IsStillInside = true
        };

        await _dbService.SaveEntryAsync(newEntry);

        await DisplayAlert("Erfolg", $"Fahrzeug {plate} eingeparkt!", "OK");

        LicenseEntry.Text = string.Empty;
        StatusLabel.Text = string.Empty;

        // Platzanzeige aktualisieren
        await UpdateAvailableSpotsAsync();
    }
}