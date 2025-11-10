using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using DM2026.Models;

namespace DM2026.ViewModels
{
    /// <summary>
    /// ViewModel pour gérer l'affichage et le tri des produits
    /// </summary>
    public class ProductsViewModel : INotifyPropertyChanged
    {
        // Champs privés pour les propriétés
        private ObservableCollection<Product> _products;
        private bool _isLoading;
        private string _currentSortCriteria = "nom";
        private bool _isAscending = true;
        private Color _defaultButtonColor = Color.FromArgb("#A0A0A0");
        private Color _activeButtonColor = Color.FromArgb("#F4B942");

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public ProductsViewModel()
        {
            // Initialisation de la collection de produits
            Products = new ObservableCollection<Product>();

            // Initialisation des commandes
            SortCommand = new Command<string>(OnSort);
            ToggleSortDirectionCommand = new Command(OnToggleSortDirection);
        }

        /// <summary>
        /// Collection observable des produits
        /// </summary>
        public ObservableCollection<Product> Products
        {
            get => _products;
            set
            {
                _products = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Indique si des données sont en cours de chargement
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
        /// Icône pour indiquer la direction du tri (ascendant ou descendant)
        /// </summary>
        public string SortDirectionIcon => _isAscending ? "arrow_up.png" : "arrow_down.png";

        /// <summary>
        /// Couleur du bouton de tri par nom
        /// </summary>
        public Color NomSortButtonColor => _currentSortCriteria == "nom" ? _activeButtonColor : _defaultButtonColor;

        /// <summary>
        /// Couleur du bouton de tri par prix
        /// </summary>
        public Color PrixSortButtonColor => _currentSortCriteria == "prix" ? _activeButtonColor : _defaultButtonColor;

        /// <summary>
        /// Couleur du bouton de tri par stock
        /// </summary>
        public Color StockSortButtonColor => _currentSortCriteria == "stock" ? _activeButtonColor : _defaultButtonColor;

        /// <summary>
        /// Commande pour trier les produits
        /// </summary>
        public ICommand SortCommand { get; }

        /// <summary>
        /// Commande pour basculer la direction du tri
        /// </summary>
        public ICommand ToggleSortDirectionCommand { get; }

        /// <summary>
        /// Méthode appelée lors du tri par critère
        /// </summary>
        /// <param name="criteria">Le critère de tri</param>
        private void OnSort(string criteria)
        {
            _currentSortCriteria = criteria;
            ApplySort();
            UpdateButtonColors();
        }

        /// <summary>
        /// Méthode pour basculer la direction du tri
        /// </summary>
        private void OnToggleSortDirection()
        {
            _isAscending = !_isAscending;
            ApplySort();
            OnPropertyChanged(nameof(SortDirectionIcon));
        }

        /// <summary>
        /// Met à jour les couleurs des boutons de tri
        /// </summary>
        private void UpdateButtonColors()
        {
            OnPropertyChanged(nameof(NomSortButtonColor));
            OnPropertyChanged(nameof(PrixSortButtonColor));
            OnPropertyChanged(nameof(StockSortButtonColor));
        }

        /// <summary>
        /// Applique le tri sur la collection de produits
        /// </summary>
        public void ApplySort()
        {
            // Vérification que la collection existe et n'est pas vide
            if (Products == null || Products.Count == 0)
                return;

            List<Product> sorted;

            // Tri en fonction du critère sélectionné
            switch (_currentSortCriteria)
            {
                case "nom":
                    sorted = _isAscending
                        ? Products.OrderBy(p => p.NomProduit).ToList()
                        : Products.OrderByDescending(p => p.NomProduit).ToList();
                    break;
                case "prix":
                    sorted = _isAscending
                        ? Products.OrderBy(p => p.PromotionalPrice).ToList()
                        : Products.OrderByDescending(p => p.PromotionalPrice).ToList();
                    break;
                case "stock":
                    sorted = _isAscending
                        ? Products.OrderBy(p => p.QuantiteDisponible).ToList()
                        : Products.OrderByDescending(p => p.QuantiteDisponible).ToList();
                    break;
                default:
                    sorted = _isAscending
                        ? Products.OrderBy(p => p.NomProduit).ToList()
                        : Products.OrderByDescending(p => p.NomProduit).ToList();
                    break;
            }

            // Mise à jour de la collection avec les produits triés
            Products.Clear();
            foreach (var product in sorted)
            {
                Products.Add(product);
            }
        }

        /// <summary>
        /// Événement de notification de changement de propriété
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Méthode pour déclencher l'événement PropertyChanged
        /// </summary>
        /// <param name="propertyName">Nom de la propriété modifiée</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}