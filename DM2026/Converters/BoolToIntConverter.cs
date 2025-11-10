using System.Globalization;

namespace DM2026.Converters
{
    /// <summary>
    /// Convertit une valeur booléenne en entier selon le paramètre fourni.
    /// Le paramètre doit être au format "valeurSiVrai:valeurSiFaux"
    /// </summary>
    public class BoolToIntConverter : IValueConverter
    {
        /// <summary>
        /// Convertit un booléen en entier.
        /// </summary>
        /// <param name="value">Valeur booléenne à convertir</param>
        /// <param name="targetType">Type cible (non utilisé)</param>
        /// <param name="parameter">Chaîne au format "valeurSiVrai:valeurSiFaux"</param>
        /// <param name="culture">Informations culturelles (non utilisées)</param>
        /// <returns>Entier correspondant à la valeur booléenne</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Vérifie que la valeur est un booléen et que le paramètre est une chaîne
            if (value is bool boolValue && parameter is string paramString)
            {
                // Divise le paramètre en deux valeurs séparées par ':'
                string[] values = paramString.Split(':');
                if (values.Length == 2)
                {
                    // Tente de convertir les deux parties en entiers
                    if (int.TryParse(values[0], out int trueValue) && int.TryParse(values[1], out int falseValue))
                    {
                        // Retourne la première valeur si vrai, la seconde si faux
                        return boolValue ? trueValue : falseValue;
                    }
                }
            }
            // Retourne 0 par défaut en cas d'échec
            return 0;
        }

        /// <summary>
        /// Méthode de conversion inverse non implémentée.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}