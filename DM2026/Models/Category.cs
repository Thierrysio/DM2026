using System.Text.Json.Serialization;

namespace DM2026.Models
{
    public class Category
    {
        #region attributs
        private int id;
        private string nom;
        private List<Category> lesCategories;
        private List<Product> lesProduits;
        #endregion

        #region constructeurs
        public Category()
        {
            // Constructeur par défaut
        }

        public Category(int id, string nom, List<Category> lesCategories = null, List<Product> lesProduits = null)
        {
            this.id = id;
            this.nom = nom;
            this.lesCategories = lesCategories;
            this.lesProduits = lesProduits;
        }
        #endregion

        #region getters/setters
        public int Id
        {
            get => id;
            set => id = value;
        }

        public string Nom
        {
            get => nom;
            set => nom = value;
        }

        [JsonPropertyName("lescategories")]
        public List<Category>? LesCategories
        {
            get => lesCategories;
            set => lesCategories = value;
        }

        [JsonPropertyName("lesProduits")]
        public List<Product>? LesProduits
        {
            get => lesProduits;
            set => lesProduits = value;
        }
        #endregion
    }
}