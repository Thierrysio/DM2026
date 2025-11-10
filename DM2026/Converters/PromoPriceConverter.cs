using System.Globalization;
using DM2026.Models;

namespace DM2026.Converters
{
    /// <summary>
    /// Convertit un produit en affichage de prix, en tenant compte des promotions.
    /// </summary>
    public class PromoPriceConverter : IValueConverter
    {
        /// <summary>
        /// Convertit un produit en affichage de prix.
        /// </summary>
        /// <param name="value">Produit à convertir</param>
        /// <param name="targetType">Type cible (non utilisé)</param>
        /// <param name="parameter">Paramètre (non utilisé)</param>
        /// <param name="culture">Informations culturelles (non utilisées)</param>
        /// <returns>Prix formaté avec symbole € et décimales</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Product product)
            {
                // Si le produit a une promotion active, affiche le prix promotionnel
                if (product.HasActivePromotion)
                {
                    return product.PromotionalPrice.ToString("F2") + " €";
                }
                // Sinon, affiche le prix normal
                return product.Prix.ToString("F2") + " €";
            }
            // Chaîne vide en cas d'échec
            return string.Empty;
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