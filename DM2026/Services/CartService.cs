using System.Text;
using System.Text.Json;
using DM2026.Models;

namespace DM2026.Services
{
    /// <summary>
    /// Service responsable de la gestion du panier d'achat de l'utilisateur,
    /// incluant les opérations locales et la synchronisation avec le serveur.
    /// </summary>
    public class CartService
    {
        // Instance unique (pattern Singleton)
        private static CartService _instance;

        // Référence aux autres services
        private readonly UserService _userService;
        private readonly HttpClient _httpClient;

        // Collection des articles du panier
        private readonly List<CartItem> _cartItems = new List<CartItem>();

        // URL de base de l'API
        private const string BaseUrl = "http://213.130.144.159";

        /// <summary>
        /// Propriété qui permet d'accéder à l'instance unique du service
        /// (implémentation du pattern Singleton)
        /// </summary>
        public static CartService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CartService();
                }
                return _instance;
            }
        }

        /// <summary>
        /// Obtient la liste des articles du panier
        /// </summary>
        public List<CartItem> CartItems => _cartItems;

        /// <summary>
        /// Calcule et retourne le montant total du panier
        /// </summary>
        public double TotalAmount => _cartItems.Sum(item => item.TotalPrice);

        /// <summary>
        /// Retourne le nombre total d'articles dans le panier
        /// </summary>
        public int ItemCount => _cartItems.Sum(item => item.Quantity);

        /// <summary>
        /// Constructeur privé pour le pattern Singleton
        /// </summary>
        private CartService()
        {
            _userService = UserService.Instance;
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Ajoute un produit au panier ou incrémente sa quantité s'il est déjà présent
        /// </summary>
        /// <param name="product">Le produit à ajouter</param>
        /// <param name="quantity">La quantité à ajouter (défaut: 1)</param>
        public void AddToCart(Product product, int quantity = 1)
        {
            // Vérifie si le produit existe déjà dans le panier
            var existingItem = _cartItems.FirstOrDefault(item => item.Product.Id == product.Id);

            if (existingItem != null)
            {
                // Si le produit existe, augmente sa quantité
                existingItem.Quantity += quantity;
            }
            else
            {
                // Sinon, crée un nouvel article dans le panier
                _cartItems.Add(new CartItem
                {
                    Product = product,
                    Quantity = quantity
                });
            }
        }

        /// <summary>
        /// Met à jour la quantité d'un produit dans le panier
        /// </summary>
        /// <param name="product">Le produit à mettre à jour</param>
        /// <param name="quantity">La nouvelle quantité</param>
        public void UpdateQuantity(Product product, int quantity)
        {
            var existingItem = _cartItems.FirstOrDefault(item => item.Product.Id == product.Id);

            if (existingItem != null)
            {
                if (quantity <= 0)
                {
                    // Si la quantité est inférieure ou égale à 0, supprime le produit du panier
                    _cartItems.Remove(existingItem);
                }
                else
                {
                    // Sinon, met à jour la quantité
                    existingItem.Quantity = quantity;
                }
            }
        }

        /// <summary>
        /// Supprime un produit du panier
        /// </summary>
        /// <param name="product">Le produit à supprimer</param>
        public void RemoveFromCart(Product product)
        {
            var existingItem = _cartItems.FirstOrDefault(item => item.Product.Id == product.Id);

            if (existingItem != null)
            {
                _cartItems.Remove(existingItem);
            }
        }

        /// <summary>
        /// Vide complètement le panier
        /// </summary>
        public void ClearCart()
        {
            _cartItems.Clear();
        }

        /// <summary>
        /// Synchronise le panier local avec le serveur
        /// </summary>
        /// <returns>Vrai si la synchronisation a réussi, sinon faux</returns>
        public async Task<bool> SyncCartWithServerAsync()
        {
            try
            {
                // Vérifie si l'utilisateur est connecté et si le panier n'est pas vide
                if (!_userService.IsLoggedIn || _cartItems.Count == 0)
                {
                    return false;
                }

                // Envoie chaque article du panier au serveur
                foreach (var item in _cartItems)
                {
                    var success = await AddProductToOrderAsync(item.Product, item.Quantity);
                    if (!success)
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error syncing cart with server: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Ajoute un produit à la commande sur le serveur
        /// </summary>
        /// <param name="product">Le produit à ajouter</param>
        /// <param name="quantity">La quantité</param>
        /// <returns>Vrai si l'opération a réussi, sinon faux</returns>
        public async Task<bool> AddProductToOrderAsync(Product product, int quantity)
        {
            try
            {
                if (!_userService.IsLoggedIn)
                {
                    return false;
                }

                // Utilise le prix promotionnel si disponible
                double priceToUse = product.HasActivePromotion ? product.PromotionalPrice : product.Prix;

                // Prépare les données pour la requête
                var requestData = new
                {
                    userId = _userService.CurrentUser.Id,
                    produitId = product.Id,
                    quantite = quantity,
                    prix = priceToUse // Utilise le prix ajusté
                };

                // Sérialise les données en JSON
                string jsonData = JsonSerializer.Serialize(requestData);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                // Envoie la requête POST à l'API
                var response = await _httpClient.PostAsync($"{BaseUrl}/api/mobile/AjoutProduitCommandemobile", content);

                if (!response.IsSuccessStatusCode)
                {
                    // Log les erreurs pour le débogage
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Server error: {errorContent}");
                    Console.WriteLine($"Status code: {response.StatusCode}");
                    return false;
                }

                // Si succès, ajouter au panier local
                AddToCart(product, quantity);
                return true;
            }
            catch (Exception ex)
            {
                // Log les exceptions
                Console.WriteLine($"Error adding product to order: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// Met à jour la quantité d'un produit dans la commande sur le serveur
        /// </summary>
        /// <param name="product">Le produit à mettre à jour</param>
        /// <param name="quantity">La nouvelle quantité</param>
        /// <returns>Vrai si l'opération a réussi, sinon faux</returns>
        public async Task<bool> UpdateProductQuantityAsync(Product product, int quantity)
        {
            try
            {
                if (!_userService.IsLoggedIn)
                {
                    return false;
                }

                // Prépare les données pour la requête
                var requestData = new
                {
                    userId = _userService.CurrentUser.Id,
                    nomProduit = product.NomProduit,
                    quantite = quantity
                };

                // Sérialise les données en JSON
                string jsonData = JsonSerializer.Serialize(requestData);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                // Envoie la requête POST à l'API
                var response = await _httpClient.PostAsync($"{BaseUrl}/api/mobile/MajProduitCommandemobile", content);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating product quantity: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Récupère les articles du panier depuis le serveur
        /// </summary>
        /// <returns>Liste des articles du panier ou null en cas d'erreur</returns>
        public async Task<List<CartItemResponse>> GetCurrentCartItemsAsync()
        {
            try
            {
                if (!_userService.IsLoggedIn)
                {
                    return null;
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
                var response = await _httpClient.PostAsync($"{BaseUrl}/api/mobile/commandenonvalideemobile", content);

                if (response.IsSuccessStatusCode)
                {
                    // Parse la réponse JSON
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API Response: {jsonResponse}");

                    var cartItems = JsonSerializer.Deserialize<List<CartItemResponse>>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return cartItems;
                }

                return null;
            }
            catch (Exception ex)
            {
                // Log les exceptions
                Console.WriteLine($"Error getting current cart items: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return null;
            }
        }
    }
}