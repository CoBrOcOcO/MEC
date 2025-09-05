using System.Text.RegularExpressions;

namespace MECWeb.Services
{
    public class RepositoryValidationService
    {
        // Regex für S-[Zahlen] Pattern (z.B. S-12345)
        private static readonly Regex SNumberPattern = new Regex(@"^S-\d+$", RegexOptions.Compiled);

        /// <summary>
        /// Validiert Repository-Namen nach dem S-[Zahlen] Muster
        /// </summary>
        /// <param name="repositoryName">Der zu validierende Repository-Name</param>
        /// <returns>ValidationResult mit Success und ErrorMessage</returns>
        public ValidationResult ValidateRepositoryName(string repositoryName)
        {
            if (string.IsNullOrWhiteSpace(repositoryName))
            {
                return new ValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Repository-Name darf nicht leer sein."
                };
            }

            // Prüfe ob das S-[Zahlen] Muster erfüllt ist
            if (!SNumberPattern.IsMatch(repositoryName))
            {
                return new ValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Repository-Name muss dem Format 'S-[Zahlen]' entsprechen (z.B. S-12345)."
                };
            }

            // Weitere Validierungen
            if (repositoryName.Length > 100)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Repository-Name ist zu lang (max. 100 Zeichen)."
                };
            }

            return new ValidationResult
            {
                IsValid = true,
                ErrorMessage = null
            };
        }

        /// <summary>
        /// Formatiert einen Repository-Namen automatisch zum S-[Zahlen] Format
        /// </summary>
        /// <param name="input">Eingabe (z.B. "12345" oder "S-12345")</param>
        /// <returns>Formatierter Name (z.B. "S-12345")</returns>
        public string FormatRepositoryName(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            input = input.Trim();

            // Wenn bereits korrekt formatiert
            if (SNumberPattern.IsMatch(input))
                return input;

            // Wenn nur Zahlen eingegeben wurden, S- hinzufügen
            if (Regex.IsMatch(input, @"^\d+$"))
                return $"S-{input}";

            // Wenn S ohne - eingegeben wurde (z.B. "S12345")
            var match = Regex.Match(input, @"^S(\d+)$");
            if (match.Success)
                return $"S-{match.Groups[1].Value}";

            // Andernfalls unverändert zurückgeben
            return input;
        }

        /// <summary>
        /// Prüft ob ein Repository-Name das korrekte Format hat
        /// </summary>
        /// <param name="repositoryName">Repository-Name</param>
        /// <returns>True wenn Format korrekt ist</returns>
        public bool IsValidRepositoryName(string repositoryName)
        {
            return ValidateRepositoryName(repositoryName).IsValid;
        }

        /// <summary>
        /// Extrahiert die Nummer aus einem S-[Zahlen] Repository-Namen
        /// </summary>
        /// <param name="repositoryName">Repository-Name (z.B. "S-12345")</param>
        /// <returns>Nummer als String (z.B. "12345") oder null wenn ungültig</returns>
        public string? ExtractNumber(string repositoryName)
        {
            if (string.IsNullOrWhiteSpace(repositoryName))
                return null;

            var match = Regex.Match(repositoryName, @"^S-(\d+)$");
            return match.Success ? match.Groups[1].Value : null;
        }

        /// <summary>
        /// Generiert Beispiel-Namen für die UI
        /// </summary>
        /// <returns>Liste von Beispiel Repository-Namen</returns>
        public List<string> GetExampleNames()
        {
            return new List<string>
            {
                "S-12345",
                "S-67890",
                "S-100001",
                "S-999"
            };
        }

        /// <summary>
        /// Validiert Ordner-Namen für die Ordner-Erstellung
        /// </summary>
        /// <param name="folderName">Ordner-Name</param>
        /// <returns>ValidationResult</returns>
        public ValidationResult ValidateFolderName(string folderName)
        {
            if (string.IsNullOrWhiteSpace(folderName))
            {
                return new ValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Ordnername darf nicht leer sein."
                };
            }

            // Prüfe auf ungültige Zeichen
            var invalidChars = new char[] { '/', '\\', ':', '*', '?', '"', '<', '>', '|' };
            if (folderName.IndexOfAny(invalidChars) >= 0)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Ordnername enthält ungültige Zeichen (/, \\, :, *, ?, \", <, >, |)."
                };
            }

            // Prüfe Länge
            if (folderName.Length > 50)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Ordnername ist zu lang (max. 50 Zeichen)."
                };
            }

            // Prüfe auf reservierte Namen
            var reservedNames = new[] { "CON", "PRN", "AUX", "NUL", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9" };
            if (reservedNames.Contains(folderName.ToUpper()))
            {
                return new ValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Dieser Ordnername ist reserviert und kann nicht verwendet werden."
                };
            }

            return new ValidationResult
            {
                IsValid = true,
                ErrorMessage = null
            };
        }

        /// <summary>
        /// Validiert Ordner-Beschreibung
        /// </summary>
        /// <param name="description">Beschreibung</param>
        /// <param name="minLength">Minimale Länge (Standard: 10)</param>
        /// <returns>ValidationResult</returns>
        public ValidationResult ValidateFolderDescription(string description, int minLength = 10)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                return new ValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Beschreibung darf nicht leer sein."
                };
            }

            if (description.Trim().Length < minLength)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    ErrorMessage = $"Beschreibung muss mindestens {minLength} Zeichen haben."
                };
            }

            if (description.Length > 500)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Beschreibung ist zu lang (max. 500 Zeichen)."
                };
            }

            return new ValidationResult
            {
                IsValid = true,
                ErrorMessage = null
            };
        }
    }

    /// <summary>
    /// Result-Klasse für Validierungen
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string? ErrorMessage { get; set; }
    }
}