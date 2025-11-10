using DantecMarketApp.Models;

namespace DantecMarketApp.Services
{
    /// <summary>
    /// Service gérant les informations de l'utilisateur et son authentification
    /// Implémente le patron de conception Singleton
    /// </summary>
    public class UserService
    {
        // Instance unique du service (pattern Singleton)
        private static UserService _instance;
        // Utilisateur actuellement connecté
        private User _currentUser;

        /// <summary>
        /// Propriété statique pour accéder à l'instance unique du service
        /// </summary>
        public static UserService Instance
        {
            get
            {
                // Crée l'instance si elle n'existe pas encore (lazy initialization)
                if (_instance == null)
                {
                    _instance = new UserService();
                }
                return _instance;
            }
        }

        /// <summary>
        /// Propriété pour accéder ou définir l'utilisateur actuel
        /// </summary>
        public User CurrentUser
        {
            get => _currentUser;
            set => _currentUser = value;
        }

        /// <summary>
        /// Vérifie si un utilisateur est actuellement connecté
        /// </summary>
        public bool IsLoggedIn => _currentUser != null;

        /// <summary>
        /// Connecte un utilisateur en stockant ses informations
        /// </summary>
        /// <param name="user">L'utilisateur à connecter</param>
        public void Login(User user)
        {
            _currentUser = user;
        }

        /// <summary>
        /// Déconnecte l'utilisateur en effaçant ses informations
        /// Conserve les identifiants si "Se souvenir de moi" est activé
        /// </summary>
        public void Logout()
        {
            _currentUser = null;
            // Supprime uniquement les informations d'authentification, pas le choix de se souvenir
            if (!Preferences.Get("RememberMe", false))
            {
                Preferences.Remove("CurrentUserEmail");
                Preferences.Remove("CurrentUserPassword");
            }
        }

        /// <summary>
        /// Vérifie si des identifiants ont été sauvegardés précédemment
        /// </summary>
        /// <returns>true si des identifiants sont sauvegardés, false sinon</returns>
        public bool HasSavedCredentials()
        {
            return Preferences.Get("RememberMe", false) &&
                   !string.IsNullOrEmpty(Preferences.Get("CurrentUserEmail", string.Empty)) &&
                   !string.IsNullOrEmpty(Preferences.Get("CurrentUserPassword", string.Empty));
        }

        /// <summary>
        /// Récupère les identifiants sauvegardés
        /// </summary>
        /// <returns>Tuple contenant l'email et le mot de passe sauvegardés</returns>
        public (string Email, string Password) GetSavedCredentials()
        {
            return (
                Preferences.Get("CurrentUserEmail", string.Empty),
                Preferences.Get("CurrentUserPassword", string.Empty)
            );
        }
    }
}