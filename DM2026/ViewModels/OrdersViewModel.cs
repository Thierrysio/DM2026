// ViewModels/OrdersViewModel.cs

// Importation des espaces de noms nécessaires
using System.Collections.ObjectModel;   // Pour utiliser les collections observables qui notifient les changements
using System.ComponentModel;            // Pour l'interface INotifyPropertyChanged
using System.Runtime.CompilerServices;  // Pour l'attribut CallerMemberName
using System.Windows.Input;             // Pour les commandes
using DM2026.Models;           // Pour accéder aux modèles de données
using DM2026.Services;         // Pour accéder aux services
using DM2026.Views;            // Pour la navigation entre vues

namespace DM2026.ViewModels
{
    // Classe ViewModel pour la gestion des commandes, implémente INotifyPropertyChanged pour la liaison de données
    public class OrdersViewModel : INotifyPropertyChanged
    {
        // Déclaration des champs privés
        private readonly OrderService _orderService;                     // Service de gestion des commandes
        private bool _isLoading;                                         // Indique si un chargement est en cours
        private ObservableCollection<Order> _allOrders = new ObservableCollection<Order>(); // Liste complète des commandes
        private string _currentFilter = "All";                           // Filtre actuel appliqué

        // Collection observable des commandes filtrées - permet la mise à jour automatique de l'interface utilisateur
        public ObservableCollection<Order> Orders { get; } = new ObservableCollection<Order>();

        // Propriété indiquant si un chargement est en cours, avec notification de changement
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged(); // Notifie l'UI que la propriété a changé
            }
        }

        // Propriété calculée indiquant si la liste des commandes est vide
        public bool IsOrdersEmpty => Orders.Count == 0;

        // Propriété calculée donnant le nombre de commandes actives/en cours
        public int ActiveOrdersCount => _allOrders.Count(o => o.Etat == "Confirmée" || o.Etat == "En cours de traitement" || o.Etat == "-");

        // Propriété calculée donnant le nombre de commandes prêtes
        public int ReadyOrdersCount => _allOrders.Count(o => o.Etat == "Traitée");

        // Propriété calculée donnant le nombre de commandes livrées
        public int DeliveredOrdersCount => _allOrders.Count(o => o.Etat == "Livrée");

        // Commandes disponibles pour l'interface utilisateur
        public ICommand RefreshOrdersCommand { get; }      // Pour rafraîchir la liste des commandes
        public ICommand CancelOrderCommand { get; }        // Pour annuler une commande
        public ICommand FilterOrdersCommand { get; }       // Pour filtrer les commandes
        public ICommand ViewOrderDetailsCommand { get; }   // Pour voir les détails d'une commande
        public ICommand GoToProductsCommand { get; }       // Pour naviguer vers la page des produits

        // Constructeur - initialise le service et les commandes
        public OrdersViewModel()
        {
            _orderService = OrderService.Instance; // Récupération de l'instance du service des commandes

            // Initialisation des commandes
            RefreshOrdersCommand = new Command(async () => await LoadOrdersAsync());
            CancelOrderCommand = new Command<Order>(async (order) => await CancelOrderAsync(order));
            FilterOrdersCommand = new Command<string>(ApplyFilter);
            ViewOrderDetailsCommand = new Command<Order>(ShowOrderDetails);
            GoToProductsCommand = new Command(NavigateToProducts);

            // Chargement initial des commandes
            LoadOrdersAsync();
        }

        // Méthode pour appliquer un filtre sur les commandes
        private void ApplyFilter(string filter)
        {
            _currentFilter = filter; // Mémorisation du filtre actuel
            Orders.Clear();          // Effacement de la liste actuelle

            // Application du filtre selon le critère sélectionné
            switch (filter)
            {
                case "Processing": // Commandes en cours de traitement
                    foreach (var order in _allOrders.Where(o => o.Etat == "Confirmée" || o.Etat == "En cours de traitement" || o.Etat == "-"))
                    {
                        Orders.Add(order);
                    }
                    break;
                case "Completed": // Commandes terminées (traitées ou livrées)
                    foreach (var order in _allOrders.Where(o => o.Etat == "Traitée" || o.Etat == "Livrée"))
                    {
                        Orders.Add(order);
                    }
                    break;
                default: // Toutes les commandes
                    foreach (var order in _allOrders)
                    {
                        Orders.Add(order);
                    }
                    break;
            }

            // Notification du changement pour la propriété IsOrdersEmpty
            OnPropertyChanged(nameof(IsOrdersEmpty));
        }

        // Méthode pour afficher les détails d'une commande
        private void ShowOrderDetails(Order order)
        {
            // Affichage des détails de la commande dans une boîte de dialogue
            Application.Current.MainPage.DisplayAlert(
                $"Commande #{order.Id}",
                $"Statut: {order.Etat}\nDate: {order.DateCommande}\nCreneau: {order.PlanningDetails}\nMontant: {order.MontantTotal:F2}€",
                "Fermer");
        }

        // Méthode pour naviguer vers la page des produits
        private async void NavigateToProducts()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new ProductsPage());
        }

        // Méthode pour charger les commandes depuis le service
        public async Task LoadOrdersAsync()
        {
            try
            {
                IsLoading = true; // Indique le début du chargement

                // Récupération des commandes via le service
                var orders = await _orderService.GetUserOrdersAsync();

                // Mise à jour de la liste complète des commandes
                _allOrders.Clear();
                foreach (var order in orders)
                {
                    _allOrders.Add(order);
                }

                // Application du filtre actuel sur les nouvelles données
                ApplyFilter(_currentFilter);

                // Notification des changements pour les compteurs
                OnPropertyChanged(nameof(ActiveOrdersCount));
                OnPropertyChanged(nameof(ReadyOrdersCount));
                OnPropertyChanged(nameof(DeliveredOrdersCount));
            }
            catch (Exception ex)
            {
                // Gestion des erreurs
                Console.WriteLine($"Error loading orders: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Erreur", "Impossible de charger les commandes", "OK");
            }
            finally
            {
                IsLoading = false; // Indique la fin du chargement
            }
        }

        // Méthode pour annuler une commande
        private async Task CancelOrderAsync(Order order)
        {
            // Vérification si l'état de la commande permet l'annulation
            if (!string.IsNullOrEmpty(order.Etat) && order.Etat != "Confirmée")
            {
                // Affichage d'un message si l'annulation n'est pas possible
                await Application.Current.MainPage.DisplayAlert(
                    "Impossible d'annuler",
                    "Vous ne pouvez pas annuler cette commande car elle est déjà en cours de traitement.",
                    "OK");
                return;
            }

            // Demande de confirmation à l'utilisateur
            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Confirmer l'annulation",
                "Voulez-vous vraiment annuler cette commande ?",
                "Oui", "Non");

            if (confirm)
            {
                IsLoading = true; // Indique le début du processus

                // Appel au service pour annuler la commande
                bool success = await _orderService.CancelOrderAsync(order.Id);

                if (success)
                {
                    // Si l'annulation a réussi, suppression de la commande des listes
                    _allOrders.Remove(order);
                    Orders.Remove(order);

                    // Notification des changements pour les propriétés affectées
                    OnPropertyChanged(nameof(IsOrdersEmpty));
                    OnPropertyChanged(nameof(ActiveOrdersCount));
                    OnPropertyChanged(nameof(ReadyOrdersCount));
                    OnPropertyChanged(nameof(DeliveredOrdersCount));

                    // Message de confirmation
                    await Application.Current.MainPage.DisplayAlert("Succès", "Commande annulée avec succès", "OK");
                }
                else
                {
                    // Message d'erreur en cas d'échec
                    await Application.Current.MainPage.DisplayAlert("Erreur", "Impossible d'annuler la commande", "OK");
                }
                IsLoading = false; // Indique la fin du processus
            }
        }

        // Implémentation de l'événement PropertyChanged de l'interface INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        // Méthode pour déclencher l'événement PropertyChanged
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}