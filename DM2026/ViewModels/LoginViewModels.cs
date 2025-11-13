// ViewModels/LoginViewModel.cs

// Importation des espaces de noms nécessaires
using System.ComponentModel;            // Pour l'interface INotifyPropertyChanged
using System.Runtime.CompilerServices;  // Pour l'attribut CallerMemberName
using System.Text;                      // Pour la classe StringBuilder
using System.Text.Json;                 // Pour la sérialisation/désérialisation JSON
using System.Text.RegularExpressions;   // Pour les expressions régulières (validation d'email)
using System.Windows.Input;             // Pour les commandes
using DM2026.Models;           // Pour accéder aux modèles de données
using DM2026.Services;         // Pour accéder aux services
using DM2026.Views;            // Pour la navigation entre vues

namespace DM2026.ViewModels
{
    // Classe ViewModel pour la gestion de la connexion, implémente INotifyPropertyChanged pour la liaison de données
    public class LoginViewModel : INotifyPropertyChanged
    {
        // Déclaration des champs privés
        private string? email;                        // Email saisi par l'utilisateur
        private string? password;                     // Mot de passe saisi par l'utilisateur
        private string message = string.Empty;        // Message à afficher à l'utilisateur
        private string emailError = string.Empty;     // Message d'erreur pour l'email
        private string passwordError = string.Empty;  // Message d'erreur pour le mot de passe
        private readonly HttpClient _httpClient;      // Client HTTP pour les appels API
        private bool rememberMe;                      // Indique si l'utilisateur veut enregistrer ses identifiants
        private bool isPasswordVisible;               // Indique si le mot de passe est visible

        // Constructeur - initialise le client HTTP et les commandes
        public LoginViewModel()
        {
            // Initialisation du client HTTP avec l'URL de base
            _httpClient = new HttpClient { BaseAddress = new Uri("http://213.130.144.159") };

            // Initialisation des commandes
            LoginCommand = new Command(async () => await Login());
            TogglePasswordVisibilityCommand = new Command(() => IsPasswordVisible = !IsPasswordVisible);
        }

        // Propriété pour la visibilité du mot de passe
        public bool IsPasswordVisible
        {
            get => isPasswordVisible;
            set
            {
                isPasswordVisible = value;
                OnPropertyChanged(); // Notifie l'UI du changement
            }
        }

        // Commande pour basculer la visibilité du mot de passe
        public ICommand TogglePasswordVisibilityCommand { get; }

        // Propriété pour l'option "Se souvenir de moi"
        public bool RememberMe
        {
            get => rememberMe;
            set
            {
                rememberMe = value;
                OnPropertyChanged(); // Notifie l'UI du changement
            }
        }

        // Propriété pour l'email avec validation
        public string? Email
        {
            get => email;
            set
            {
                email = value;
                OnPropertyChanged(); // Notifie l'UI du changement
                ValidateEmail();     // Valide l'email à chaque modification
            }
        }

        // Propriété pour le mot de passe avec validation
        public string? Password
        {
            get => password;
            set
            {
                password = value;
                OnPropertyChanged(); // Notifie l'UI du changement
                ValidatePassword();  // Valide le mot de passe à chaque modification
            }
        }

        // Propriété pour le message à afficher à l'utilisateur
        public string Message
        {
            get => message;
            set
            {
                message = value;
                OnPropertyChanged(); // Notifie l'UI du changement
            }
        }

        // Propriété pour le message d'erreur d'email
        public string EmailError
        {
            get => emailError;
            set
            {
                emailError = value;
                OnPropertyChanged(); // Notifie l'UI du changement
            }
        }

        // Propriété pour le message d'erreur de mot de passe
        public string PasswordError
        {
            get => passwordError;
            set
            {
                passwordError = value;
                OnPropertyChanged(); // Notifie l'UI du changement
            }
        }

        // Commande pour se connecter
        public ICommand LoginCommand { get; }

        // Méthode pour valider le format de l'email
        private void ValidateEmail()
        {
            // Expression régulière pour la validation d'email
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            // Vérification si l'email est vide
            if (string.IsNullOrEmpty(Email))
            {
                EmailError = "L'email est requis.";
            }
            // Vérification si l'email correspond au format attendu
            else if (!Regex.IsMatch(Email, emailPattern))
            {
                EmailError = "Format d'email invalide.";
            }
            else
            {
                EmailError = string.Empty; // Pas d'erreur
            }
        }

        // Méthode pour valider le mot de passe
        private void ValidatePassword()
        {
            // Vérification si le mot de passe est vide
            if (string.IsNullOrEmpty(Password))
            {
                PasswordError = "Le mot de passe est requis.";
                return;
            }

            // Vérification des critères de sécurité du mot de passe
            var hasMinLength = Password.Length >= 8;         // Au moins 8 caractères
            var hasUpperCase = Password.Any(char.IsUpper);   // Au moins une majuscule
            var hasLowerCase = Password.Any(char.IsLower);   // Au moins une minuscule
            var hasDigit = Password.Any(char.IsDigit);       // Au moins un chiffre
            //var hasSpecialChar = Password.Any(ch => !char.IsLetterOrDigit(ch)); // Caractère spécial (commenté)

            var errors = new List<string>(); // Liste des erreurs

            // Ajout des erreurs si les critères ne sont pas respectés
            if (!hasMinLength) errors.Add("8 caractères minimum");
            if (!hasUpperCase) errors.Add("une majuscule");
            if (!hasLowerCase) errors.Add("une minuscule");
            if (!hasDigit) errors.Add("un chiffre");
            //if (!hasSpecialChar) errors.Add("un caractère spécial");

            // Construction du message d'erreur si nécessaire
            if (errors.Any())
            {
                PasswordError = $"Le mot de passe doit contenir : {string.Join(", ", errors)}.";
            }
            else
            {
                PasswordError = string.Empty; // Pas d'erreur
            }
        }

        // Méthode pour la connexion
        private async Task Login()
        {
            // Validation des champs
            ValidateEmail();
            ValidatePassword();

            // Vérification des erreurs de validation
            if (!string.IsNullOrEmpty(EmailError) || !string.IsNullOrEmpty(PasswordError))
            {
                return; // Arrêt si des erreurs sont présentes
            }

            // Vérification si les champs sont vides
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                Message = "Veuillez remplir tous les champs";
                return;
            }

            try
            {
                Message = "Tentative de connexion...";

                // Préparation des données de connexion
                var loginData = new
                {
                    Email = Email,
                    Password = Password
                };

                // Sérialisation des données en JSON
                var jsonContent = JsonSerializer.Serialize(loginData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Envoi de la requête HTTP
                var response = await _httpClient.PostAsync("/api/mobile/GetFindUser", content);

                if (response.IsSuccessStatusCode)
                {
                    // Lecture de la réponse
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true // Ignorer la casse des propriétés
                    };

                    // Désérialisation de l'utilisateur
                    var user = JsonSerializer.Deserialize<User>(jsonString, options);

                    if (user != null)
                    {
                        Message = "Connexion réussie !";

                        // Stockage de l'utilisateur connecté dans le service
                        UserService.Instance.Login(user);

                        // Gestion de l'option "Se souvenir de moi"
                        if (RememberMe)
                        {
                            // Enregistrement des informations de connexion
                            Preferences.Set("RememberMe", true);
                            Preferences.Set("CurrentUserEmail", Email);
                            Preferences.Set("CurrentUserPassword", Password);
                        }
                        else
                        {
                            // Suppression des informations de connexion
                            Preferences.Remove("RememberMe");
                            Preferences.Remove("CurrentUserEmail");
                            Preferences.Remove("CurrentUserPassword");
                        }

                        // Navigation vers la page d'accueil
                        await Application.Current.MainPage.Navigation.PushAsync(new HomePage(user));
                    }
                    else
                    {
                        Message = "Identifiants incorrects";
                    }
                }
                else
                {
                    // Gestion des erreurs HTTP
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Message = $"Échec de la connexion : {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                // Gestion des exceptions
                Message = $"Erreur de connexion : {ex.Message}";
            }
        }

        // Implémentation de l'événement PropertyChanged de l'interface INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        // Méthode pour déclencher l'événement PropertyChanged
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}