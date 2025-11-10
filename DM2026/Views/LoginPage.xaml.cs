namespace DantecMarketApp.Views;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using DantecMarketApp.ViewModels;
using DantecMarketApp.Services;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
        var viewModel = new LoginViewModel();

        // Charger les informations sauvegardées si "Se souvenir de moi" était coché
        if (Preferences.Get("RememberMe", false))
        {
            viewModel.Email = Preferences.Get("CurrentUserEmail", string.Empty);
            viewModel.Password = Preferences.Get("CurrentUserPassword", string.Empty);
            viewModel.RememberMe = true;
        }

        BindingContext = viewModel;
    }
}