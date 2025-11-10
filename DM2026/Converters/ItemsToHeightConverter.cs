using System.Globalization;

namespace DM2026.Converters
{
    /// <summary>
    /// Convertit un nombre d'éléments en hauteur pour l'interface utilisateur.
    /// Limite à 3 éléments maximum pour éviter un affichage trop grand.
    /// </summary>
    public class ItemsToHeightConverter : IValueConverter
    {
        /// <summary>
        /// Convertit un nombre d'éléments en hauteur.
        /// </summary>
        /// <param name="value">Nombre d'éléments</param>
        /// <param name="targetType">Type cible (non utilisé)</param>
        /// <param name="parameter">Paramètre (non utilisé)</param>
        /// <param name="culture">Informations culturelles (non utilisées)</param>
        /// <returns>Hauteur calculée en pixels</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int count)
            {
                // Limite à 3 éléments maximum et calcule la hauteur (76 pixels par élément)
                int maxItems = 3;
                int itemsToShow = Math.Min(count, maxItems);
                return itemsToShow * 76;
            }
            // Hauteur par défaut
            return 100;
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