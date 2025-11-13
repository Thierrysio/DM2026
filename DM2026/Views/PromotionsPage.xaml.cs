using System.Collections.ObjectModel;
using System.Text.Json;
using DM2026.Models;

namespace DM2026.Views
{
    public partial class PromotionsPage : ContentPage
    {
        public bool IsLoading { get; set; }
        public ObservableCollection<Product> PromotionProducts { get; set; } = new ObservableCollection<Product>();

        public PromotionsPage()
        {
            InitializeComponent();
            BindingContext = this;
            LoadPromotions();
        }

        private async void LoadPromotions()
        {
            try
            {
                IsLoading = true;
                OnPropertyChanged(nameof(IsLoading));

                var httpClient = new HttpClient();
                var response = await httpClient.GetAsync("http://213.130.144.159/api/mobile/allcategoriesParent");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var categories = JsonSerializer.Deserialize<List<Category>>(content, options);

                    // Récupérer tous les produits de toutes les catégories et sous-catégories
                    var allProducts = GetAllProducts(categories);

                    // Filtrer uniquement les produits avec une promotion active
                    var productsWithPromo = allProducts.Where(p => p.HasActivePromotion).ToList();

                    // Vérifier que les produits ont des prix valides
                    foreach (var product in productsWithPromo)
                    {
                        // S'assurer que le prix n'est pas à 0
                        if (product.Prix <= 0)
                        {
                            product.Prix = product.PromotionalPrice * 1.2; // Estimation du prix original si 0
                        }

                        // Vérifier les images
                        if (product.LesImages == null)
                        {
                            product.LesImages = new List<ProductImage>();
                        }

                        if (product.LesImages.Count == 0)
                        {
                            // Ajouter une image par défaut pour éviter les erreurs
                            product.LesImages.Add(new ProductImage { Id = 0, Url = "dotnet_bot.png" });
                        }
                    }

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        PromotionProducts.Clear();
                        foreach (var product in productsWithPromo)
                        {
                            PromotionProducts.Add(product);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur", $"Impossible de charger les promotions: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        private List<Product> GetAllProducts(List<Category> categories)
        {
            var allProducts = new List<Product>();

            foreach (var category in categories)
            {
                // Ajouter les produits directs de cette catégorie
                if (category.LesProduits != null)
                {
                    allProducts.AddRange(category.LesProduits);
                }

                // Récursivement récupérer les produits des sous-catégories
                if (category.LesCategories != null)
                {
                    allProducts.AddRange(GetAllProducts(category.LesCategories));
                }
            }

            return allProducts;
        }

        private async void OnProductSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is Product selectedProduct)
            {
                await Navigation.PushAsync(new ProductDetailPage(selectedProduct));
                ((ListView)sender).SelectedItem = null;
            }
        }
    }
}