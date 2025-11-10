// ViewModels/FavoritesViewModel.cs

// Importation des espaces de noms nécessaires
using System.Collections.ObjectModel;   // Pour utiliser les collections observables qui notifient les changements
using System.ComponentModel;            // Pour l'interface INotifyPropertyChanged
using System.Runtime.CompilerServices;  // Pour l'attribut CallerMemberName
using System.Windows.Input;             // Pour les commandes
using DantecMarketApp.Models;           // Pour accéder aux modèles de données
using DantecMarketApp.Services;         // Pour accéder aux services

namespace DantecMarketApp.ViewModels
{
    // Classe ViewModel pour la gestion des favoris, implémente INotifyPropertyChanged pour la liaison de données
    public class FavoritesViewModel : INotifyPropertyChanged
    {
        // Déclaration des champs privés
        private readonly FavoriteService _favoriteService; // Service gérant les opérations liées aux favoris
        private bool _isLoading;                          // Indique si une opération de chargement est en cours
        private bool _isEmpty;                            // Indique si la liste des favoris est vide

        // Collection observable des articles favoris - permet la mise à jour automatique de l'interface utilisateur
        public ObservableCollection<FavoriteItem> Favorites { get; } = new ObservableCollection<FavoriteItem>();

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

        // Propriété indiquant si la liste des favoris est vide, avec notification de changement
        public bool IsEmpty
        {
            get => _isEmpty;
            set
            {
                _isEmpty = value;
                OnPropertyChanged(); // Notifie l'UI que la propriété a changé
            }
        }

        // Commande pour supprimer un article des favoris
        public ICommand RemoveFavoriteCommand { get; }

        // Commande pour rafraîchir la liste des favoris
        public ICommand RefreshCommand { get; }

        // Constructeur - initialise le service et les commandes
        public FavoritesViewModel(FavoriteService favoriteService)
        {
            _favoriteService = favoriteService; // Injection du service des favoris

            // Initialisation des commandes
            RemoveFavoriteCommand = new Command<int>(async (productId) => await RemoveFavorite(productId));
            RefreshCommand = new Command(async () => await LoadFavorites());
        }

        // Méthode pour charger les favoris depuis le service
        public async Task LoadFavorites()
        {
            IsLoading = true; // Indique le début du chargement

            try
            {
                // Récupère les favoris via le service
                var favorites = await _favoriteService.GetFavoritesAsync();

                // Vide la collection actuelle et ajoute les nouveaux favoris
                Favorites.Clear();
                foreach (var favorite in favorites)
                {
                    Favorites.Add(favorite);
                }

                // Met à jour l'indicateur de liste vide
                IsEmpty = Favorites.Count == 0;
            }
            catch (Exception ex)
            {
                // Journalise les erreurs de chargement
                Console.WriteLine($"Erreur lors du chargement des favoris: {ex.Message}");
            }
            finally
            {
                IsLoading = false; // Indique la fin du chargement, qu'il ait réussi ou échoué
            }
        }

        // Méthode pour supprimer un article des favoris
        private async Task RemoveFavorite(int productId)
        {
            try
            {
                // Appelle le service pour supprimer l'article des favoris
                var success = await _favoriteService.RemoveFromFavoritesAsync(productId);
                if (success)
                {
                    // Si la suppression a réussi, supprime l'article de la collection locale
                    var favoriteToRemove = Favorites.FirstOrDefault(f => f.Id == productId);
                    if (favoriteToRemove != null)
                    {
                        Favorites.Remove(favoriteToRemove);
                        IsEmpty = Favorites.Count == 0; // Met à jour l'indicateur de liste vide
                    }
                }
            }
            catch (Exception ex)
            {
                // Journalise les erreurs de suppression
                Console.WriteLine($"Erreur lors de la suppression du favori: {ex.Message}");
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