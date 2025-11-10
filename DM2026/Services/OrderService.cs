using DantecMarketApp.Models;
using System.Text;
using System.Text.Json;

namespace DantecMarketApp.Services
{
    /// <summary>
    /// Service responsable de la gestion des commandes utilisateur,
    /// incluant la récupération et l'annulation de commandes.
    /// </summary>
    public class OrderService
    {
        // Instance unique (pattern Singleton)
        private static OrderService _instance;

        // Client HTTP pour les appels API
        private readonly HttpClient _httpClient;

        // Service utilisateur pour accéder aux informations de l'utilisateur connecté
        private readonly UserService _userService;

        // Ensemble des IDs de commandes annulées localement
        private HashSet<int> _cancelledOrderIds = new HashSet<int>();

        // URL de base de l'API
        private const string BaseUrl = "http://213.130.144.159";

        /// <summary>
        /// Propriété qui permet d'accéder à l'instance unique du service
        /// (implémentation du pattern Singleton)
        /// </summary>
        public static OrderService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new OrderService();
                }
                return _instance;
            }
        }

        /// <summary>
        /// Constructeur privé pour le pattern Singleton
        /// </summary>
        private OrderService()
        {
            _httpClient = new HttpClient();
            _userService = UserService.Instance;
        }

        /// <summary>
        /// Récupère toutes les commandes de l'utilisateur depuis le serveur
        /// </summary>
        /// <returns>Liste des commandes ou une liste vide en cas d'erreur</returns>
        public async Task<List<Order>> GetUserOrdersAsync()
        {
            try
            {
                // Vérifie si l'utilisateur est connecté
                if (!_userService.IsLoggedIn)
                {
                    return new List<Order>();
                }

                // Prépare les données pour la requête
                var requestData = new
                {
                    userId = _userService.CurrentUser.Id
                };

                // Sérialise les données en JSON
                string jsonData = JsonSerializer.Serialize(requestData);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                // Envoie la requête POST à l'API
                var response = await _httpClient.PostAsync($"{BaseUrl}/api/mobile/allcommandes", content);

                if (response.IsSuccessStatusCode)
                {
                    // Parse la réponse JSON
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API Response: {jsonResponse}");

                    // Désérialise la réponse en liste d'objets Order
                    var orders = JsonSerializer.Deserialize<List<Order>>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<Order>();

                    // Filtre les commandes annulées localement
                    return orders.Where(o => !_cancelledOrderIds.Contains(o.Id)).ToList();
                }

                return new List<Order>();
            }
            catch (Exception ex)
            {
                // Log l'erreur et retourne une liste vide
                Console.WriteLine($"Error getting orders: {ex.Message}");
                return new List<Order>();
            }
        }

        /// <summary>
        /// Annule une commande spécifique
        /// </summary>
        /// <param name="orderId">ID de la commande à annuler</param>
        /// <returns>Vrai si l'annulation a réussi, sinon faux</returns>
        public async Task<bool> CancelOrderAsync(int orderId)
        {
            try
            {
                // Vérifie si l'utilisateur est connecté
                if (!_userService.IsLoggedIn)
                {
                    return false;
                }

                // Prépare les données pour la requête
                var requestData = new
                {
                    userId = _userService.CurrentUser.Id,
                    commandeId = orderId
                };

                // Sérialise les données en JSON
                string jsonData = JsonSerializer.Serialize(requestData);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                // Envoie la requête POST à l'API
                var response = await _httpClient.PostAsync($"{BaseUrl}/api/mobile/SupprimerCommande", content);

                if (response.IsSuccessStatusCode)
                {
                    // Ajoute l'ID de la commande à la liste des commandes annulées localement
                    _cancelledOrderIds.Add(orderId);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                // Log l'erreur et retourne faux
                Console.WriteLine($"Error cancelling order: {ex.Message}");
                return false;
            }
        }
    }
}