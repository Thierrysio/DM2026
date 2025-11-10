using DantecMarketApp.Models;
using DantecMarketApp.Services;

namespace DantecMarketApp.Views
{
    public partial class ProductDetailPage : ContentPage
    {
        private readonly CartService _cartService;
        private readonly Product _product;
        private FavoriteService _favoriteService;
        private bool _isFavorite = false;
        public ProductDetailPage(Product product)
        {
            InitializeComponent();
            _cartService = CartService.Instance;
            _product = product;
            BindingContext = product;
            _favoriteService = new FavoriteService(UserService.Instance);
            CheckFavoriteStatus();
        }

        private async void CheckFavoriteStatus()
        {
            _isFavorite = await _favoriteService.IsFavoriteAsync(((Product)BindingContext).Id);
            FavoriteButton.Source = _isFavorite ? "heart_filled.png" : "heart_unfilled.png";
        }

        private async void OnFavoriteClicked(object sender, EventArgs e)
        {
            var product = (Product)BindingContext;
            bool success;

            if (_isFavorite)
            {
                success = await _favoriteService.RemoveFromFavoritesAsync(product.Id);
            }
            else
            {
                success = await _favoriteService.AddToFavoritesAsync(product.Id);
            }

            if (success)
            {
                _isFavorite = !_isFavorite;
                FavoriteButton.Source = _isFavorite ? "heart_filled.png" : "heart_unfilled.png";
            }
        }

        private async void OnAddToCartClicked(object sender, EventArgs e)
        {
            try
            {
                if (!UserService.Instance.IsLoggedIn)
                {
                    await DisplayAlert("Attention", "Veuillez vous connecter pour ajouter des produits au panier", "OK");
                    return;
                }

                _cartService.AddToCart(_product);
                bool success = await _cartService.AddProductToOrderAsync(_product, 1);

                if (success)
                {
                    await DisplayAlert("Succès", "Produit ajouté au panier", "OK");
                }
                else
                {
                    await DisplayAlert("Erreur", "Impossible d'ajouter le produit au panier", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur", $"Une erreur est survenue : {ex.Message}", "OK");
            }
        }

        private async void OnViewCartClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CartPage());
        }
    }
}