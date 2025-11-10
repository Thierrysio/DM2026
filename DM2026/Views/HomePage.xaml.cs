using DantecMarketApp.Models;
using DantecMarketApp.Services;
using DantecMarketApp.ViewModels;
using System.Collections.ObjectModel;

namespace DantecMarketApp.Views;
public partial class HomePage : ContentPage
{
    private readonly User _user;
    private readonly ProductService _productService;
    public ObservableCollection<TopSellingProduct> TopSellingProducts { get; set; } = new();

    public HomePage(User user)
    {
        InitializeComponent();
        _user = user;
        _productService = new ProductService();

        // Affiche le nom de l'utilisateur
        if (!string.IsNullOrEmpty(user.Prenom) && !string.IsNullOrEmpty(user.Nom))
        {
            UserName.Text = $"{user.Prenom} {user.Nom}";
        }
        else
        {
            UserName.Text = user.Email;
        }

        // Relie le carousel aux produits
        TopSellingCarousel.ItemsSource = TopSellingProducts;
        TopSellingIndicators.SetBinding(IndicatorView.ItemsSourceProperty,
            new Binding(nameof(CarouselView.ItemsSource), source: TopSellingCarousel));
        TopSellingIndicators.SetBinding(IndicatorView.PositionProperty,
            new Binding(nameof(CarouselView.Position), source: TopSellingCarousel));

        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Charge les produits les plus vendus
        await LoadTopSellingProducts();
    }

    private async Task LoadTopSellingProducts()
    {
        try
        {
            var products = await _productService.GetTopSellingProductsAsync();

            if (products.Any())
            {
                // Efface et ajoute les nouveaux produits
                TopSellingProducts.Clear();
                foreach (var product in products)
                {
                    TopSellingProducts.Add(product);
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Impossible de charger les meilleures ventes: {ex.Message}", "OK");
        }
    }

    private async void OnTopSellingProductTapped(object sender, EventArgs e)
    {
        if (sender is Frame frame && frame.BindingContext is TopSellingProduct topSellingProduct)
        {
            // Convertir TopSellingProduct en Product comme précédemment
            Product product = new Product
            {
                Id = topSellingProduct.Id,
                NomProduit = topSellingProduct.NomProduit,
                Description = topSellingProduct.DescriptionCourte,
                DescriptionCourte = topSellingProduct.DescriptionCourte,
                Prix = topSellingProduct.Prix,
                LesImages = new List<ProductImage>
            {
                new ProductImage { Url = topSellingProduct.ImageUrl }
            }
            };

            // Si le produit a une promotion, ajoutons-la
            if (topSellingProduct.HasPromo && topSellingProduct.PrixPromo.HasValue)
            {
                product.Promotions = new List<Promotion>
            {
                new Promotion
                {
                    Prix = topSellingProduct.PrixPromo.Value,
                    DateDebutString = DateTime.Now.AddDays(-1).ToString(),
                    DateFinString = DateTime.Now.AddDays(7).ToString(),
                    PromotionCategory = new PromoCategory
                    {
                        Nom = topSellingProduct.NomCategoriePromo ?? "Promotion"
                    }
                }
            };
            }

            await Navigation.PushAsync(new ProductDetailPage(product));
        }
    }


    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Déconnexion", "Êtes-vous sûr de vouloir vous déconnecter?", "Oui", "Non");
        if (confirm)
        {
            UserService.Instance.Logout();
            await Navigation.PopToRootAsync();
        }
    }

    private async void OnSearchTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SearchPage());
    }

    // Gestionnaire d'événements pour la catégorie "Nos produits"
    private async void OnProductsCategoryTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ProductsPage());
    }

    private async void OnCartTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CartPage());
    }

    private async void OnOrdersTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new OrdersPage());
    }

    // Views/HomePage.xaml.cs (Ajouter cette méthode)
    private async void OnFavoritesTapped(object sender, EventArgs e)
    {
        // Création du FavoriteService avec l'instance de UserService
        var favoriteService = new FavoriteService(UserService.Instance);
        // Création du ViewModel avec le service correctement initialisé
        var viewModel = new FavoritesViewModel(favoriteService);
        await Navigation.PushAsync(new FavoritesPage(viewModel));
    }

    private async void OnPromotionsTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new PromotionsPage());
    }
}