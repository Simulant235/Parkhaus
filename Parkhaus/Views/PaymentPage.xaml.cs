using Parkhaus.Models;
using Parkhaus.Services;
using Parkhaus.Helper;

namespace Parkhaus.Views;

public partial class PaymentPage : ContentPage
{
    private readonly DatabaseService _dbService;
    private ParkingEntry? _currentEntry;

    public PaymentPage(DatabaseService dbService)
    {
        InitializeComponent();
        _dbService = dbService;
    }

    private async void OnSearchClicked(object sender, EventArgs e)
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
                InvoiceContainer.IsVisible = false;
                return;
            }

            // Unbezahlten Eintrag suchen (IsStillInside=true UND IsActive=true)
            _currentEntry = await _dbService.GetUnpaidEntryAsync(plate);

            if (_currentEntry == null)
            {
                // Prüfen WARUM nicht gefunden
                var latestEntry = await _dbService.GetLatestEntryByLicensePlateAsync(plate);

                if (latestEntry != null && latestEntry.IsStillInside && !latestEntry.IsActive)
                {
                    // ✅ HIER kommt jetzt die richtige Meldung!
                    StatusLabel.Text = "Dieses Fahrzeug wurde bereits bezahlt!";
                    StatusLabel.TextColor = Colors.Red;
                    InvoiceContainer.IsVisible = false;
                    return;
                }
                else
                {
                    StatusLabel.Text = "Fahrzeug ist nicht im Parkhaus!";
                    StatusLabel.TextColor = Colors.Red;
                    InvoiceContainer.IsVisible = false;
                    return;
                }
            }

            // Rechnung anzeigen (nur wenn NICHT bezahlt)
            ShowInvoice(_currentEntry);
            StatusLabel.Text = string.Empty;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Fehler", $"Datenbankfehler: {ex.Message}", "OK");
        }
    }

    private void ShowInvoice(ParkingEntry entry)
    {
        DateTime exitTime = DateTime.Now;
        decimal fee = PricingService.CalculateFee(entry.EntryTime, exitTime);
        TimeSpan duration = exitTime - entry.EntryTime;

        // Rechnung befüllen
        InvoicePlate.Text = entry.LicensePlate;
        InvoiceEntryTime.Text = entry.EntryTime.ToString("dd.MM.yyyy HH:mm");
        InvoiceExitTime.Text = exitTime.ToString("dd.MM.yyyy HH:mm");
        InvoiceDuration.Text = FormatDuration(duration);
        InvoiceTotal.Text = $"CHF {fee:F2}";

        // Rechnung anzeigen
        InvoiceContainer.IsVisible = true;
    }

    private string FormatDuration(TimeSpan duration)
    {
        int hours = (int)duration.TotalHours;
        int minutes = duration.Minutes;

        if (hours > 0 && minutes > 0)
            return $"{hours}h {minutes}min";
        else if (hours > 0)
            return $"{hours}h";
        else
            return $"{minutes}min";
    }

    private async void OnPayClicked(object sender, EventArgs e)
    {
        if (_currentEntry == null) return;

        // Prüfen ob bereits bezahlt wurde (Doppel-Bezahlung verhindern)
        if (!_currentEntry.IsActive)
        {
            await DisplayAlert("Hinweis", "Diese Rechnung wurde bereits bezahlt!", "OK");
            ResetForm();
            return;
        }

        try
        {
            // Button während Verarbeitung deaktivieren
            PayButton.IsEnabled = false;

            // Ausfahrtszeit und Gebühr speichern
            DateTime exitTime = DateTime.Now;
            _currentEntry.ExitTime = exitTime;
            _currentEntry.TotalFee = PricingService.CalculateFee(_currentEntry.EntryTime, exitTime);
            _currentEntry.IsActive = false;
            _currentEntry.IsStillInside = true;

            // In Datenbank speichern
            await _dbService.SaveEntryAsync(_currentEntry);

            // WICHTIG: Formular SOFORT zurücksetzen (vor DisplayAlert!)
            var totalFee = _currentEntry.TotalFee;
            ResetForm();

            // Erfolgsmeldung (NACH Reset, damit Button nicht mehr klickbar ist)
            await DisplayAlert("Bezahlt", 
                $"Die Zahlung von CHF {totalFee:F2} wurde erfolgreich erhalten.\n\nSie können sich nun zum Ausgang begeben.", 
                "OK");
        }
        catch (Exception ex)
        {
            PayButton.IsEnabled = true; // Bei Fehler wieder aktivieren
            await DisplayAlert("Fehler", $"Fehler beim Speichern: {ex.Message}", "OK");
        }
    }

    private void ResetForm()
    {
        LicenseEntry.Text = string.Empty;
        InvoiceContainer.IsVisible = false;
        StatusLabel.Text = string.Empty;
        _currentEntry = null;
    }
}
