using System.Globalization;

namespace DantecMarketApp.Converters
{
    /// <summary>
    /// Convertit l'état d'une commande en couleur correspondante.
    /// </summary>
    public class EtatColorConverter : IValueConverter
    {
        /// <summary>
        /// Convertit un état de commande en couleur.
        /// </summary>
        /// <param name="value">État à convertir</param>
        /// <param name="targetType">Type cible (non utilisé)</param>
        /// <param name="parameter">Paramètre (non utilisé)</param>
        /// <param name="culture">Informations culturelles (non utilisées)</param>
        /// <returns>Couleur correspondant à l'état</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Vérifie que la valeur est une chaîne représentant un état
            if (value is string etat)
            {
                // Utilise une expression switch pour déterminer la couleur selon l'état
                return etat switch
                {
                    "Confirmée" => Color.FromArgb("#FFA500"),     // Orange
                    "En cours de traitement" => Color.FromArgb("#1E90FF"), // Bleu
                    "Traitée" => Color.FromArgb("#32CD32"),         // Vert
                    "Livrée" => Color.FromArgb("#4B0082"),          // Indigo
                    _ => Color.FromArgb("#808080"),                // Gris pour les états non reconnus
                };
            }
            // Couleur grise par défaut
            return Color.FromArgb("#808080");
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