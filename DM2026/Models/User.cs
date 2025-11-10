using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM2026.Models
{
    public class User
    {
        #region attributs
        private int id;
        private string email;
        private string userIdentifier;
        private List<string> roles;
        private string password;
        private string nom;
        private string prenom;
        private string telephone;
        private string classe;
        private int fidelite;
        private string? photoUrl;
        private List<object> lesMessages;
        #endregion

        #region constructeurs
        public User()
        {
            this.email = string.Empty;
            this.userIdentifier = string.Empty;
            this.roles = new List<string>();
            this.password = string.Empty;
            this.nom = string.Empty;
            this.prenom = string.Empty;
            this.telephone = string.Empty;
            this.classe = string.Empty;
            this.fidelite = 0;
            this.lesMessages = new List<object>();
        }

        public User(int id, string email, string userIdentifier, List<string> roles, string password,
                   string nom, string prenom, string telephone, string classe, int fidelite,
                   string? photoUrl = null)
        {
            this.id = id;
            this.email = email;
            this.userIdentifier = userIdentifier;
            this.roles = roles;
            this.password = password;
            this.nom = nom;
            this.prenom = prenom;
            this.telephone = telephone;
            this.classe = classe;
            this.fidelite = fidelite;
            this.photoUrl = photoUrl;
            this.lesMessages = new List<object>();
        }
        #endregion

        #region getters/setters
        public int Id { get => id; set => id = value; }
        public string Email { get => email; set => email = value; }
        public string UserIdentifier { get => userIdentifier; set => userIdentifier = value; }
        public List<string> Roles { get => roles; set => roles = value; }
        public string Password { get => password; set => password = value; }
        public string Nom { get => nom; set => nom = value; }
        public string Prenom { get => prenom; set => prenom = value; }
        public string Telephone { get => telephone; set => telephone = value; }
        public string Classe { get => classe; set => classe = value; }
        public int Fidelite { get => fidelite; set => fidelite = value; }
        public string? PhotoUrl { get => photoUrl; set => photoUrl = value; }
        public List<object> LesMessages { get => lesMessages; set => lesMessages = value; }
        #endregion

        #region methodes
        public string GetNomComplet()
        {
            return $"{Prenom} {Nom}";
        }

        public string GetUserInfo()
        {
            return $"Utilisateur: {GetNomComplet()} - Email: {Email} - Classe: {Classe}";
        }

        public bool HasRole(string role)
        {
            return Roles.Contains(role);
        }

        public bool IsAdmin()
        {
            return HasRole("ROLE_ADMIN");
        }

        public void AddFidelityPoints(int points)
        {
            this.fidelite += points;
        }
        #endregion
    }
}