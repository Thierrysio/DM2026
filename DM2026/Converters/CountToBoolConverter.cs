using System.Globalization;

namespace DM2026.Converters
{
    /// <summary>
    /// Convertit un nombre en booléen en le comparant à une valeur spécifiée.
    /// Retourne vrai si le compteur est égal à la valeur de comparaison.
    /// </summary>
    public class CountToBoolConverter : IValueConverter
    {
        /// <summary>
        /// Convertit un nombre en booléen.
        /// </summary>
        /// <param name="value">Nombre à comparer</param>
        /// <param name="targetType">Type cible (non utilisé)</param>
        /// <param name="parameter">Valeur de comparaison</param>
        /// <param name="culture">Informations culturelles (non utilisées)</param>
        /// <returns>Vrai si le nombre est égal à la valeur de comparaison</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Si la valeur est null, retourne vrai par défaut
            if (value == null) return true;

            // Convertit en entier
            int count = (int)value;
            // Utilise la valeur du paramètre ou 0 par défaut
            int compareValue = parameter != null ? int.Parse(parameter.ToString()) : 0;

            // Retourne vrai si les valeurs sont égales
            return count == compareValue;
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