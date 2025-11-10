// Services/FavoriteService.cs
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using DM2026.Models;

namespace DM2026.Services
{
    /// <summary>
    /// Service responsable de la gestion des produits favoris de l'utilisateur.
    /// Permet d'ajouter, supprimer et récupérer les favoris depuis le serveur.
    /// </summary>
    public class FavoriteService
    {
        private readonly HttpClient _httpClient;
        private readonly UserService _userService;

        /// <summary>
        /// Constructeur qui initialise le service avec les dépendances nécessaires
        /// </summary>
        /// <param name="userService">Service utilisateur pour accéder aux informations de l'utilisateur courant</param>
        public FavoriteService(UserService userService)
        {
            // Initialise le client HTTP avec l'URL de base de l'API
            _httpClient = new HttpClient { BaseAddress = new Uri("http://213.130.144.159") };
            _userService = userService;
        }

        /// <summary>
        /// Récupère la liste des produits favoris de l'utilisateur depuis le serveur
        /// </summary>
        /// <returns>Liste des produits favoris ou une liste vide en cas d'erreur</returns>
        public async Task<List<FavoriteItem>> GetFavoritesAsync()
        {
            // Vérifie si l'utilisateur est connecté
            if (_userService.CurrentUser == null)
                return new List<FavoriteItem>();

            try
            {
                // Prépare la requête avec l'ID de l'utilisateur
                var request = new { userId = _userService.CurrentUser.Id };

                // Envoie une requête POST à l'API et attend la réponse
                var response = await _httpClient.PostAsJsonAsync("/api/mobile/getListeFavorisMobile", request);

                if (response.IsSuccessStatusCode)
                {
                    // Configure les options de désérialisation pour ignorer la casse des propriétés
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                    // Désérialise la réponse JSON en liste d'objets FavoriteItem
                    var favorites = await response.Content.ReadFromJsonAsync<List<FavoriteItem>>(options);

                    // Retourne la liste des favoris ou une liste vide si la réponse est null
                    return favorites ?? new List<FavoriteItem>();
                }

                return new List<FavoriteItem>();
            }
            catch (Exception ex)
            {
                // Log l'erreur et retourne une liste vide
                Console.WriteLine($"Erreur lors de la récupération des favoris: {ex.Message}");
                return new List<FavoriteItem>();
            }
        }

        /// <summary>
        /// Ajoute un produit aux favoris de l'utilisateur
        /// </summary>
        /// <param name="productId">ID du produit à ajouter</param>
        /// <returns>Vrai si l'opération a réussi, sinon faux</returns>
        public async Task<bool> AddToFavoritesAsync(int productId)
        {
            // Vérifie si l'utilisateur est connecté
            if (_userService.CurrentUser == null)
                return false;

            try
            {
                // Prépare la requête avec l'ID de l'utilisateur et l'ID du produit
                var request = new { userId = _userService.CurrentUser.Id, produitId = productId };

                // Envoie une requête POST à l'API et attend la réponse
                var response = await _httpClient.PostAsJsonAsync("/api/mobile/AjoutFavoriMobile", request);

                // Retourne vrai si la requête a réussi, sinon faux
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                // Log l'erreur et retourne faux
                Console.WriteLine($"Erreur lors de l'ajout aux favoris: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Supprime un produit des favoris de l'utilisateur
        /// </summary>
        /// <param name="productId">ID du produit à supprimer</param>
        /// <returns>Vrai si l'opération a réussi, sinon faux</returns>
        public async Task<bool> RemoveFromFavoritesAsync(int productId)
        {
            // Vérifie si l'utilisateur est connecté
            if (_userService.CurrentUser == null)
                return false;

            try
            {
                // Prépare la requête avec l'ID de l'utilisateur et l'ID du produit
                var request = new { userId = _userService.CurrentUser.Id, produitId = productId };

                // Envoie une requête POST à l'API et attend la réponse
                var response = await _httpClient.PostAsJsonAsync("/api/mobile/SupprimerFavoriMobile", request);

                // Retourne vrai si la requête a réussi, sinon faux
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                // Log l'erreur et retourne faux
                Console.WriteLine($"Erreur lors de la suppression des favoris: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Vérifie si un produit est dans les favoris de l'utilisateur
        /// </summary>
        /// <param name="productId">ID du produit à vérifier</param>
        /// <returns>Vrai si le produit est un favori, sinon faux</returns>
        public async Task<bool> IsFavoriteAsync(int productId)
        {
            // Récupère la liste des favoris et vérifie si le produit s'y trouve
            var favorites = await GetFavoritesAsync();
            return favorites.Any(f => f.Id == productId);
        }
    }
}