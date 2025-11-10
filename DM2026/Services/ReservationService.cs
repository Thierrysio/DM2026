using DantecMarketApp.Models;
using System.Text;
using System.Text.Json;

namespace DantecMarketApp.Services
{
    /// <summary>
    /// Service gérant les réservations de crénaux horaires pour la récupération des commandes
    /// Implémente le patron de conception Singleton
    /// </summary>
    public class ReservationService
    {
        // Instance unique du service (pattern Singleton)
        private static ReservationService _instance;
        // Client HTTP pour les appels à l'API
        private readonly HttpClient _httpClient;
        // URL de base de l'API
        private const string BaseUrl = "http://213.130.144.159";

        /// <summary>
        /// Propriété statique pour accéder à l'instance unique du service
        /// </summary>
        public static ReservationService Instance
        {
            get
            {
                // Crée l'instance si elle n'existe pas encore (lazy initialization)
                if (_instance == null)
                {
                    _instance = new ReservationService();
                }
                return _instance;
            }
        }

        /// <summary>
        /// Constructeur privé pour empêcher l'instanciation directe (pattern Singleton)
        /// </summary>
        private ReservationService()
        {
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Récupère les créneaux horaires disponibles depuis l'API
        /// </summary>
        /// <returns>Liste des créneaux horaires disponibles</returns>
        public async Task<List<TimeSlot>> GetAvailableTimeSlotsAsync()
        {
            try
            {
                // Appel à l'API pour récupérer les créneaux de la semaine courante
                var response = await _httpClient.GetAsync($"{BaseUrl}/api/mobile/semaine-courante");

                if (response.IsSuccessStatusCode)
                {
                    // Lecture de la réponse JSON
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API TimeSlots Response: {jsonResponse}");

                    // Options pour la désérialisation JSON (insensible à la casse)
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    // Désérialisation de la réponse JSON en liste d'objets TimeSlot
                    var timeSlots = JsonSerializer.Deserialize<List<TimeSlot>>(jsonResponse, options);

                    // Journalisation des créneaux horaires après désérialisation
                    if (timeSlots != null)
                    {
                        foreach (var slot in timeSlots)
                        {
                            Console.WriteLine($"TimeSlot: ID={slot.Id}, " +
                                $"JourString={slot.JourString}, " +
                                $"HeureDebutString={slot.HeureDebutString}, " +
                                $"HeureFinString={slot.HeureFinString}, " +
                                $"Jour={slot.Jour}, " +
                                $"HeureDebut={slot.HeureDebut}, " +
                                $"HeureFin={slot.HeureFin}, " +
                                $"DisplayText={slot.DisplayText}");
                        }
                    }

                    // Retourne la liste des créneaux ou une liste vide si null
                    return timeSlots ?? new List<TimeSlot>();
                }
                else
                {
                    // Journalisation de l'erreur en cas d'échec de l'appel API
                    Console.WriteLine($"Error getting time slots: {response.StatusCode}");
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error content: {errorContent}");
                }

                // Retourne une liste vide en cas d'erreur
                return new List<TimeSlot>();
            }
            catch (Exception ex)
            {
                // Gestion des exceptions
                Console.WriteLine($"Error getting time slots: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return new List<TimeSlot>();
            }
        }

        /// <summary>
        /// Envoie une demande de réservation d'un créneau horaire pour une commande
        /// </summary>
        /// <param name="userId">ID de l'utilisateur</param>
        /// <param name="selectedTimeSlot">Créneau horaire sélectionné</param>
        /// <param name="orderId">ID de la commande</param>
        /// <returns>true si la réservation a réussi, false sinon</returns>
        public async Task<bool> ReserveOrderAsync(int userId, TimeSlot selectedTimeSlot, int orderId)
        {
            try
            {
                // Création de l'objet de demande de réservation
                var reservation = new ReservationRequest
                {
                    IdUser = userId,
                    Jour = selectedTimeSlot.Jour.ToString("yyyy-MM-dd"),
                    HeureDebut = selectedTimeSlot.HeureDebut.ToString("HH:mm:ss"),
                    CommandeId = orderId,
                    Id = selectedTimeSlot.Id
                };

                // Sérialisation de l'objet en JSON
                string jsonData = JsonSerializer.Serialize(reservation);
                Console.WriteLine($"Sending reservation request: {jsonData}");
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                // Envoi de la requête POST à l'API
                var response = await _httpClient.PostAsync($"{BaseUrl}/api/mobile/reservermobile", content);

                // En cas d'échec, journalisation des détails de l'erreur
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Reservation failed: {response.StatusCode}");
                    Console.WriteLine($"Error content: {errorContent}");
                }

                // Retourne true si la requête a réussi, false sinon
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                // Gestion des exceptions
                Console.WriteLine($"Error reserving order: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }
    }
}