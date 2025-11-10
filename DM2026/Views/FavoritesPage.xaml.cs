using DM2026.Models;
using DM2026.ViewModels;
using System.Net.Http.Json;

namespace DM2026.Views
{
    public partial class FavoritesPage : ContentPage
    {
        private readonly FavoritesViewModel _viewModel;
        private List<Category> _categories;
        private List<Product> _allProducts;

        public Command ViewProductCommand { get; private set; }
        public Command BrowseProductsCommand { get; private set; }

        public FavoritesPage(FavoritesViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;

            ViewProductCommand = new Command<int>(OnViewProduct);
            BrowseProductsCommand = new Command(OnBrowseProducts);

            // Load products data when the page appears
            LoadProductsData();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadFavorites();
        }

        private async void LoadProductsData()
        {
            try
            {
                var httpClient = new HttpClient { BaseAddress = new Uri("http://213.130.144.159") };
                _categories = await httpClient.GetFromJsonAsync<List<Category>>("/api/mobile/allcategoriesParent");

                // Flatten all products from all categories
                _allProducts = new List<Product>();
                if (_categories != null)
                {
                    FlattenProducts(_categories);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading products: {ex.Message}");
                await DisplayAlert("Erreur", "Impossible de charger les produits", "OK");
            }
        }

        private void FlattenProducts(List<Category> categories)
        {
            foreach (var category in categories)
            {
                if (category.LesProduits != null)
                {
                    _allProducts.AddRange(category.LesProduits);
                }

                if (category.LesCategories != null)
                {
                    FlattenProducts(category.LesCategories);
                }
            }
        }

        private async void OnViewProduct(int productId)
        {
            try
            {
                // Find the product in our cached list
                var product = _allProducts?.FirstOrDefault(p => p.Id == productId);

                if (product != null)
                {
                    await Navigation.PushAsync(new ProductDetailPage(product));
                }
                else
                {
                    // If product not found in cache, reload data and try again
                    await DisplayAlert("Information", "Chargement du produit en cours...", "OK");
                    LoadProductsData();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error navigating to product: {ex.Message}");
                await DisplayAlert("Erreur", "Impossible d'afficher les détails du produit", "OK");
            }
        }

        private async void OnBrowseProducts()
        {
            await Navigation.PushAsync(new ProductsPage());
        }
    }
}