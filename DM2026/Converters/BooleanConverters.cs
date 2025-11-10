using System.Globalization;

namespace DantecMarketApp.Converters
{
    /// <summary>
    /// Convertit un booléen en son inverse (vrai devient faux et vice versa).
    /// Utile pour inverser la logique dans le XAML.
    /// </summary>
    public class InvertBooleanConverter : IValueConverter
    {
        /// <summary>
        /// Inverse la valeur booléenne.
        /// </summary>
        /// <param name="value">Valeur booléenne à inverser</param>
        /// <param name="targetType">Type cible (non utilisé)</param>
        /// <param name="parameter">Paramètre (non utilisé)</param>
        /// <param name="culture">Informations culturelles (non utilisées)</param>
        /// <returns>Booléen inversé</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Si la valeur est un booléen, retourne son inverse
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            // Sinon, retourne la valeur inchangée
            return value;
        }

        /// <summary>
        /// Conversion inverse, identique à la conversion directe car l'opération est symétrique.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Même logique que Convert car l'inversion est une opération symétrique
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return value;
        }
    }
}