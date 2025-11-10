using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.Json;
using DM2026.Models;
using DM2026.ViewModels;

namespace DM2026.Views
{
    public partial class ProductsPage : ContentPage
    {
        private ProductsViewModel _viewModel;
        private HttpClient _httpClient;

        public ProductsPage()
        {
            InitializeComponent();
            _viewModel = new ProductsViewModel();
            BindingContext = _viewModel;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://213.130.144.159")
            };
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadProductsAsync();
        }

        private async Task LoadProductsAsync()
        {
            try
            {
                _viewModel.IsLoading = true;

                var response = await _httpClient.GetAsync("/api/mobile/produits");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var products = JsonSerializer.Deserialize<List<Product>>(json, options);
                    _viewModel.Products = new ObservableCollection<Product>(products);
                    _viewModel.ApplySort(); // Appliquer le tri initial
                }
                else
                {
                    await DisplayAlert("Erreur", "Impossible de charger les produits.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur", $"Une erreur s'est produite : {ex.Message}", "OK");
            }
            finally
            {
                _viewModel.IsLoading = false;
            }
        }

        private async void OnProductSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is Product selectedProduct)
            {
                // Désélectionner l'élément pour permettre une nouvelle sélection du même élément
                if (sender is ListView listView)
                {
                    listView.SelectedItem = null;
                }

                // Naviguer vers la page de détail du produit
                await Navigation.PushAsync(new ProductDetailPage(selectedProduct));
            }
        }
    }
}