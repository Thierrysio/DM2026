using System.Globalization;

namespace DantecMarketApp.Converters
{
    /// <summary>
    /// Convertit une valeur booléenne en chaîne de caractères selon le paramètre fourni.
    /// Le paramètre doit être au format "chaîneSiVrai:chaîneSiFaux"
    /// </summary>
    public class BoolToStringConverter : IValueConverter
    {
        /// <summary>
        /// Convertit un booléen en chaîne de caractères.
        /// </summary>
        /// <param name="value">Valeur booléenne à convertir</param>
        /// <param name="targetType">Type cible (non utilisé)</param>
        /// <param name="parameter">Chaîne au format "chaîneSiVrai:chaîneSiFaux"</param>
        /// <param name="culture">Informations culturelles (non utilisées)</param>
        /// <returns>Chaîne correspondant à la valeur booléenne</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Vérifie que la valeur est un booléen et que le paramètre est une chaîne
            if (value is bool boolValue && parameter is string paramString)
            {
                // Divise le paramètre en deux valeurs séparées par ':'
                string[] values = paramString.Split(':');
                if (values.Length == 2)
                {
                    // Retourne la première valeur si vrai, la seconde si faux
                    return boolValue ? values[0] : values[1];
                }
            }
            // Retourne la valeur d'origine en cas d'échec
            return value;
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