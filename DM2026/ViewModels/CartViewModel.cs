using DM2026.Models;
using DM2026.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace DM2026.ViewModels
{
    /// <summary>
    /// ViewModel pour la gestion du panier et la réservation des commandes
    /// Implémente INotifyPropertyChanged pour notifier l'UI des changements
    /// </summary>
    public class CartViewModel : INotifyPropertyChanged
    {
        // Services utilisés par le ViewModel
        private readonly CartService _cartService;
        private readonly ReservationService _reservationService;

        // Propriétés d'état
        private bool _isLoading;
        private bool _isReservationDialogVisible;
        private List<TimeSlot> _availableTimeSlots;
        private TimeSlot _selectedTimeSlot;
        private int _orderId;

        /// <summary>
        /// Collection observable des articles dans le panier
        /// </summary>
        public ObservableCollection<CartItem> CartItems { get; } = new ObservableCollection<CartItem>();

        /// <summary>
        /// Indique si une opération est en cours de chargement
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Indique si la boîte de dialogue de réservation est visible
        /// </summary>
        public bool IsReservationDialogVisible
        {
            get => _isReservationDialogVisible;
            set
            {
                _isReservationDialogVisible = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Liste des créneaux horaires disponibles pour la réservation
        /// </summary>
        public List<TimeSlot> AvailableTimeSlots
        {
            get => _availableTimeSlots;
            set
            {
                _availableTimeSlots = value;
                OnPropertyChanged();
                Console.WriteLine($"AvailableTimeSlots updated: {_availableTimeSlots?.Count ?? 0} items");
            }
        }

        /// <summary>
        /// Créneau horaire sélectionné par l'utilisateur
        /// </summary>
        public TimeSlot SelectedTimeSlot
        {
            get => _selectedTimeSlot;
            set
            {
                _selectedTimeSlot = value;
                OnPropertyChanged();
                Console.WriteLine($"SelectedTimeSlot updated: {_selectedTimeSlot?.DisplayText ?? "null"}");
            }
        }

        /// <summary>
        /// Montant total du panier (somme des prix totaux des articles)
        /// </summary>
        public double TotalAmount => CartItems.Sum(i => i.TotalPrice);

        /// <summary>
        /// Indique si le panier est vide
        /// </summary>
        public bool IsCartEmpty => CartItems.Count == 0;

        // Commandes pour les actions sur le panier
        /// <summary>Commande pour augmenter la quantité d'un article</summary>
        public ICommand IncreaseQuantityCommand { get; }
        /// <summary>Commande pour diminuer la quantité d'un article</summary>
        public ICommand DecreaseQuantityCommand { get; }
        /// <summary>Commande pour supprimer un article</summary>
        public ICommand RemoveItemCommand { get; }
        /// <summary>Commande pour finaliser l'achat</summary>
        public ICommand CheckoutCommand { get; }
        /// <summary>Commande pour rafraîchir le panier</summary>
        public ICommand RefreshCartCommand { get; }
        /// <summary>Commande pour afficher la boîte de dialogue de réservation</summary>
        public ICommand ShowReservationDialogCommand { get; }
        /// <summary>Commande pour réserver la commande</summary>
        public ICommand ReserveOrderCommand { get; }
        /// <summary>Commande pour annuler la réservation</summary>
        public ICommand CancelReservationCommand { get; }

        /// <summary>
        /// Constructeur du ViewModel du panier
        /// </summary>
        public CartViewModel()
        {
            // Initialisation des services
            _cartService = CartService.Instance;
            _reservationService = ReservationService.Instance;

            // Initialisation des commandes
            IncreaseQuantityCommand = new Command<CartItem>(async (item) => await IncreaseQuantity(item));
            DecreaseQuantityCommand = new Command<CartItem>(async (item) => await DecreaseQuantity(item));
            RemoveItemCommand = new Command<CartItem>(async (item) => await RemoveItem(item));
            CheckoutCommand = new Command(async () => await CheckoutAsync());
            RefreshCartCommand = new Command(async () => await LoadCartFromServer());
            ShowReservationDialogCommand = new Command(async () => await ShowReservationDialog());
            ReserveOrderCommand = new Command(async () => await ReserveOrder());
            CancelReservationCommand = new Command(() => IsReservationDialogVisible = false);

            // Chargement initial du panier
            RefreshCart();
            _availableTimeSlots = new List<TimeSlot>();
        }

        /// <summary>
        /// Charge les articles du panier depuis le serveur
        /// </summary>
        public async Task LoadCartFromServer()
        {
            try
            {
                IsLoading = true;
                var cartItems = await _cartService.GetCurrentCartItemsAsync();

                CartItems.Clear();

                if (cartItems != null && cartItems.Count > 0)
                {
                    foreach (var item in cartItems)
                    {
                        CartItems.Add(item.ToCartItem());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading cart: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            finally
            {
                IsLoading = false;
                OnPropertyChanged(nameof(TotalAmount));
                OnPropertyChanged(nameof(IsCartEmpty));
            }
        }

        /// <summary>
        /// Rafraîchit le panier avec les données locales
        /// </summary>
        public void RefreshCart()
        {
            CartItems.Clear();

            foreach (var item in _cartService.CartItems)
            {
                CartItems.Add(item);
            }

            OnPropertyChanged(nameof(TotalAmount));
            OnPropertyChanged(nameof(IsCartEmpty));
        }

        /// <summary>
        /// Augmente la quantité d'un article dans le panier
        /// </summary>
        /// <param name="item">Article à modifier</param>
        private async Task IncreaseQuantity(CartItem item)
        {
            item.Quantity++;
            _cartService.UpdateQuantity(item.Product, item.Quantity);

            // Notifier du changement pour rafraîchir l'UI
            OnPropertyChanged(nameof(TotalAmount));
            RefreshItem(item);

            // Mettre à jour côté serveur
            await _cartService.UpdateProductQuantityAsync(item.Product, item.Quantity);
        }

        /// <summary>
        /// Diminue la quantité d'un article dans le panier
        /// Si la quantité devient 0, l'article est supprimé
        /// </summary>
        /// <param name="item">Article à modifier</param>
        private async Task DecreaseQuantity(CartItem item)
        {
            if (item.Quantity > 1)
            {
                item.Quantity--;
                _cartService.UpdateQuantity(item.Product, item.Quantity);

                // Notifier du changement pour rafraîchir l'UI
                OnPropertyChanged(nameof(TotalAmount));
                RefreshItem(item);

                // Mettre à jour côté serveur
                await _cartService.UpdateProductQuantityAsync(item.Product, item.Quantity);
            }
            else
            {
                await RemoveItem(item);
            }
        }

        /// <summary>
        /// Supprime un article du panier
        /// </summary>
        /// <param name="item">Article à supprimer</param>
        private async Task RemoveItem(CartItem item)
        {
            CartItems.Remove(item);
            _cartService.RemoveFromCart(item.Product);

            // La mise à jour côté serveur n'est peut-être pas parfaite ici car l'API ne semble pas avoir 
            // d'endpoint pour supprimer un produit directement. Nous utilisons donc la quantité 0.
            await _cartService.UpdateProductQuantityAsync(item.Product, 0);

            OnPropertyChanged(nameof(TotalAmount));
            OnPropertyChanged(nameof(IsCartEmpty));
        }

        /// <summary>
        /// Finalise l'achat en synchronisant avec le serveur puis en vidant le panier
        /// </summary>
        private async Task CheckoutAsync()
        {
            // Vérifiez d'abord que nous avons bien synchronisé avec le serveur
            await _cartService.SyncCartWithServerAsync();

            // TODO: Implémentez la logique de paiement ou de confirmation
            // Pour l'instant, nous allons simplement vider le panier
            _cartService.ClearCart();
            RefreshCart();
        }

        /// <summary>
        /// Rafraîchit un article spécifique dans la collection observable
        /// </summary>
        /// <param name="item">Article à rafraîchir</param>
        private void RefreshItem(CartItem item)
        {
            // Trouver l'index de l'élément dans la collection
            int index = CartItems.IndexOf(item);
            if (index >= 0)
            {
                // Forcer la mise à jour de l'élément dans la collection
                CartItems[index] = null;
                CartItems[index] = item;
            }
        }

        /// <summary>
        /// Affiche la boîte de dialogue de réservation avec les créneaux disponibles
        /// </summary>
        private async Task ShowReservationDialog()
        {
            try
            {
                IsLoading = true;
                Console.WriteLine("ShowReservationDialog called");

                // Vérification que l'utilisateur est connecté
                if (!UserService.Instance.IsLoggedIn)
                {
                    Console.WriteLine("User not logged in");
                    await Application.Current.MainPage.DisplayAlert(
                        "Erreur",
                        "Vous devez être connecté pour réserver une commande",
                        "OK");
                    return;
                }

                // Récupérer les horaires disponibles
                var timeSlots = await _reservationService.GetAvailableTimeSlotsAsync();
                Console.WriteLine($"Retrieved {timeSlots.Count} time slots");

                if (timeSlots.Count == 0)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Aucun horaire disponible",
                        "Aucun horaire de réservation n'est disponible actuellement",
                        "OK");
                    return;
                }

                AvailableTimeSlots = timeSlots;

                // Sélectionner le premier créneau par défaut
                if (AvailableTimeSlots.Count > 0)
                {
                    SelectedTimeSlot = AvailableTimeSlots[0];
                }

                // Récupérer l'ID de la commande
                var cartItems = await _cartService.GetCurrentCartItemsAsync();
                if (cartItems != null && cartItems.Count > 0)
                {
                    _orderId = cartItems.First().Id;
                    Console.WriteLine($"Order ID: {_orderId}");
                    IsReservationDialogVisible = true;
                }
                else
                {
                    Console.WriteLine("No items in cart");
                    await Application.Current.MainPage.DisplayAlert(
                        "Panier vide",
                        "Votre panier est vide",
                        "OK");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ShowReservationDialog: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                await Application.Current.MainPage.DisplayAlert(
                    "Erreur",
                    "Une erreur est survenue lors de la récupération des horaires",
                    "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Réserve la commande avec le créneau horaire sélectionné
        /// </summary>
        private async Task ReserveOrder()
        {
            try
            {
                IsLoading = true;

                // Vérification qu'un créneau est sélectionné
                if (SelectedTimeSlot == null)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Sélection requise",
                        "Veuillez sélectionner un horaire de réservation",
                        "OK");
                    return;
                }

                Console.WriteLine($"Reserving order: User ID={UserService.Instance.CurrentUser.Id}, " +
                    $"TimeSlot ID={SelectedTimeSlot.Id}, Order ID={_orderId}");

                // Appel au service de réservation
                var success = await _reservationService.ReserveOrderAsync(
                    UserService.Instance.CurrentUser.Id,
                    SelectedTimeSlot,
                    _orderId);

                if (success)
                {
                    // Réservation réussie, effacer le panier
                    _cartService.ClearCart();
                    RefreshCart();
                    IsReservationDialogVisible = false;

                    // Informer l'utilisateur du succès
                    await Application.Current.MainPage.DisplayAlert(
                        "Réservation réussie",
                        $"Votre commande a été réservée pour le {SelectedTimeSlot.FormattedDay} à {SelectedTimeSlot.HeureDebut.ToString("HH:mm")}.",
                        "OK");
                }
                else
                {
                    // Gérer l'échec de la réservation
                    await Application.Current.MainPage.DisplayAlert(
                        "Erreur",
                        "La réservation a échoué. Veuillez réessayer plus tard.",
                        "OK");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ReserveOrder: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                await Application.Current.MainPage.DisplayAlert(
                    "Erreur",
                    "Une erreur est survenue lors de la réservation",
                    "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Événement déclenché lorsqu'une propriété change
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Méthode pour notifier les observateurs qu'une propriété a changé
        /// </summary>
        /// <param name="propertyName">Nom de la propriété qui a changé</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}