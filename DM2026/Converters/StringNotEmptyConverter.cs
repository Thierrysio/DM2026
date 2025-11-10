using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace DantecMarketApp.Converters
{
    /// <summary>
    /// Convertit une chaîne en booléen indiquant si elle n'est pas vide.
    /// Utile pour contrôler la visibilité des éléments d'interface.
    /// </summary>
    public class StringNotEmptyConverter : IValueConverter
    {
        /// <summary>
        /// Convertit une chaîne en booléen.
        /// </summary>
        /// <param name="value">Chaîne à vérifier</param>
        /// <param name="targetType">Type cible (non utilisé)</param>
        /// <param name="parameter">Paramètre (non utilisé)</param>
        /// <param name="culture">Informations culturelles (non utilisées)</param>
        /// <returns>Vrai si la chaîne n'est pas vide ni null</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Retourne vrai si la chaîne n'est pas vide ni null
            return !string.IsNullOrEmpty(value?.ToString());
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