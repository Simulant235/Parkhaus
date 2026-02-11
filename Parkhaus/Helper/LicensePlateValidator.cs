using System.Text.RegularExpressions;

namespace Parkhaus.Helper
{
    public static class LicensePlateValidator
    {
        // Schweizer Kantone als private Liste (nur hier gebraucht)
        private static readonly HashSet<string> ValidKantone = new()
        {
            "AG", "AI", "AR", "BE", "BL", "BS", "FR", "GE", "GL", "GR", 
            "JU", "LU", "NE", "NW", "OW", "SG", "SH", "SO", "SZ", "TG", 
            "TI", "UR", "VD", "VS", "ZG", "ZH"
        };

        public static bool IsValid(string licensePlate)
        {
            // 1. Null/Leer prüfen
            if (string.IsNullOrWhiteSpace(licensePlate))
                return false;

            // 2. Länge prüfen (3-8)
            if (licensePlate.Length < 3 || licensePlate.Length > 8)
                return false;

            // 3. Regex prüfen (2 Buchstaben + 1-6 Ziffern)
            if (!Regex.IsMatch(licensePlate, @"^[A-Z]{2}\d{1,6}$"))
                return false;

            // 4. Kanton in ValidKantone prüfen
            string kanton = licensePlate.Substring(0, 2);
            if (!ValidKantone.Contains(kanton))
                return false;

            return true;
        }

        public static string Normalize(string licensePlate)
        {
            if (string.IsNullOrWhiteSpace(licensePlate))
                return string.Empty;

            // Leerzeichen entfernen, Großbuchstaben machen, Trimmen
            return licensePlate.Trim().Replace(" ", "").ToUpper();
        }
    }
}