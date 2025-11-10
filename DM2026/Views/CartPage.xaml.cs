/*
 * CartPage.xaml.cs
 * 
 * Code-behind pour la page du panier d'achat
 * 
 * Fonctionnalités:
 * - Initialise le ViewModel qui gère les données du panier
 * - Charge les articles du panier depuis le serveur lors de l'apparition de la page
 */

using DM2026.Models;
using DM2026.ViewModels;

namespace DM2026.Views
{
    public partial class CartPage : ContentPage
    {
        private readonly CartViewModel _viewModel;

        public CartPage()
        {
            InitializeComponent();
            _viewModel = new CartViewModel();
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            // Récupère les données du panier depuis le serveur à chaque fois que la page devient visible
            await _viewModel.LoadCartFromServer();
        }
    }
}