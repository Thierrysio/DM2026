using DM2026.Services;
using DM2026.ViewModels;

namespace DM2026.Views
{
    /// <summary>
    /// Page de gestion et d'affichage des commandes de l'utilisateur.
    /// Cette page s'appuie sur le ViewModel OrdersViewModel pour gérer les données et les interactions.
    /// </summary>
    public partial class OrdersPage : ContentPage
    {
        private readonly OrdersViewModel _viewModel;

        /// <summary>
        /// Constructeur de la page des commandes.
        /// Initialise les composants UI et instancie le ViewModel associé.
        /// </summary>
        public OrdersPage()
        {
            InitializeComponent();
            _viewModel = new OrdersViewModel();
            BindingContext = _viewModel;
        }

        /// <summary>
        /// Méthode appelée lorsque la page apparaît à l'écran.
        /// Charge les commandes de l'utilisateur de manière asynchrone.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadOrdersAsync();
        }
    }
}