# Parkhaus App

Eine .NET MAUI Applikation zur Verwaltung eines Parkhauses mit 20 Stellplätzen.

## Funktionen

### 1. Einfahrt
- Schweizer Kennzeichen-Validierung (26 Kantone)
- Kapazitätsprüfung (max. 20 Fahrzeuge)
- Verhindert Doppel-Einfahrten
- Echtzeit-Anzeige freier Parkplätze

### 2. Bezahlung
- Automatische Gebührenberechnung (CHF 2.00/Stunde)
- Parkdauer-Anzeige mit Rechnung
- Schutz vor Doppel-Bezahlungen
- Anti-Manipulation (Zeitvalidierung)

### 3. Ausfahrt
- Prüfung ob Zahlung erfolgt ist
- Freigabe des Parkplatzes
- Aktualisierung der Kapazitätsanzeige

## Technische Details

- **Framework:** .NET 10 MAUI
- **Datenbank:** SQLite (lokal)
- **Validierung:** Regex-basierte Kennzeichen-Prüfung
- **Pattern:** Service Layer + Dependency Injection

## Installation

1. Repository klonen
2. In Visual Studio 2026 öffnen
3. Android-Emulator oder Windows starten
4. Projekt ausführen
