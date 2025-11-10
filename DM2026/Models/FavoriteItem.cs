using System.Text.Json.Serialization;

namespace DM2026.Models
{
    public class FavoriteItem
    {
        #region attributs
        private int id;
        private string nomProduit;
        private double prix;
        private string imageUrl;
        #endregion

        #region constructeurs
        public FavoriteItem()
        {
            // Constructeur par défaut
        }

        public FavoriteItem(int id, string nomProduit, double prix, string imageUrl)
        {
            this.id = id;
            this.nomProduit = nomProduit;
            this.prix = prix;
            this.imageUrl = imageUrl;
        }
        #endregion

        #region getters/setters
        public int Id
        {
            get => id;
            set => id = value;
        }

        [JsonPropertyName("nomProduit")]
        public string NomProduit
        {
            get => nomProduit;
            set => nomProduit = value;
        }

        public double Prix
        {
            get => prix;
            set => prix = value;
        }

        [JsonPropertyName("imageUrl")]
        public string ImageUrl
        {
            get => imageUrl;
            set => imageUrl = value;
        }
        #endregion

        #region propriétés calculées
        // Convertir l'URL en URL complète
        [JsonIgnore]
        public string FullImageUrl => !string.IsNullOrEmpty(ImageUrl) ? $"http://213.130.144.159/{ImageUrl}" : string.Empty;
        #endregion
    }
}