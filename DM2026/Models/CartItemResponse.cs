using System.Text.Json.Serialization;

namespace DM2026.Models
{
    public class CartItemResponse
    {
        #region attributs
        private int id;
        private string nomProduit;
        private int quantite;
        private double prixRetenu;
        private double total;
        private string imageUrl;
        #endregion

        #region constructeurs
        public CartItemResponse()
        {
            // Constructeur par défaut
        }

        public CartItemResponse(int id, string nomProduit, int quantite, double prixRetenu, double total, string imageUrl)
        {
            this.id = id;
            this.nomProduit = nomProduit;
            this.quantite = quantite;
            this.prixRetenu = prixRetenu;
            this.total = total;
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

        [JsonPropertyName("quantite")]
        public int Quantite
        {
            get => quantite;
            set => quantite = value;
        }

        [JsonPropertyName("prixretenu")]
        public double PrixRetenu
        {
            get => prixRetenu;
            set => prixRetenu = value;
        }

        [JsonPropertyName("total")]
        public double Total
        {
            get => total;
            set => total = value;
        }

        [JsonPropertyName("imageUrl")]
        public string ImageUrl
        {
            get => imageUrl;
            set => imageUrl = value;
        }
        #endregion

        #region methodes
        // Méthode pour convertir en CartItem
        public CartItem ToCartItem()
        {
            return new CartItem
            {
                Quantity = Quantite,
                Product = new Product
                {
                    Id = Id,
                    NomProduit = NomProduit,
                    Prix = PrixRetenu,
                    LesImages = new List<ProductImage>
                    {
                        new ProductImage
                        {
                            Url = ImageUrl
                        }
                    }
                }
            };
        }
        #endregion
    }
}