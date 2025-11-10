using System.Globalization;

namespace DantecMarketApp.Converters
{
    /// <summary>
    /// Convertit l'état d'une commande en booléen indiquant si elle peut être annulée.
    /// Une commande peut être annulée si son état est vide ou "Confirmée".
    /// </summary>
    public class StringEqualsToCancelableStateConverter : IValueConverter
    {
        /// <summary>
        /// Convertit un état de commande en booléen d'annulabilité.
        /// </summary>
        /// <param name="value">État à vérifier</param>
        /// <param name="targetType">Type cible (non utilisé)</param>
        /// <param name="parameter">Paramètre (non utilisé)</param>
        /// <param name="culture">Informations culturelles (non utilisées)</param>
        /// <returns>Vrai si la commande peut être annulée</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string state)
            {
                // Une commande peut être annulée si son état est vide ou "Confirmée"
                return string.IsNullOrEmpty(state) || state == "Confirmée";
            }
            // Faux par défaut
            return false;
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