using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using DM2026.Models;

namespace DM2026.Services
{
    /// <summary>
    /// Service responsable de la récupération des données des produits depuis l'API.
    /// </summary>
    public class ProductService
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Constructeur qui initialise le client HTTP avec l'URL de base
        /// </summary>
        public ProductService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://213.130.144.159")
            };
        }

        /// <summary>
        /// Récupère la liste des produits les plus vendus depuis l'API
        /// </summary>
        /// <returns>Liste des produits les plus vendus ou une liste vide en cas d'erreur</returns>
        public async Task<List<TopSellingProduct>> GetTopSellingProductsAsync()
        {
            try
            {
                // Envoi d'une requête GET à l'endpoint des produits les plus vendus
                var response = await _httpClient.GetAsync("/api/mobile/lesplusvendus");

                if (response.IsSuccessStatusCode)
                {
                    // Lecture de la réponse comme chaîne JSON
                    var json = await response.Content.ReadAsStringAsync();

                    // Configuration des options de désérialisation pour ignorer la casse des propriétés
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    // Désérialisation de la réponse JSON en liste d'objets TopSellingProduct
                    // Si la désérialisation échoue, retourne une liste vide
                    return JsonSerializer.Deserialize<List<TopSellingProduct>>(json, options) ?? new List<TopSellingProduct>();
                }

                // Si la requête n'a pas réussi, retourne une liste vide
                return new List<TopSellingProduct>();
            }
            catch (Exception ex)
            {
                // Log l'erreur et retourne une liste vide
                System.Diagnostics.Debug.WriteLine($"Erreur lors du chargement des produits les plus vendus: {ex.Message}");
                return new List<TopSellingProduct>();
            }
        }
    }
}