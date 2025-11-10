using System.Text.Json.Serialization;

namespace DM2026.Models
{
    public class Product
    {
        #region attributs
        private int id;
        private string nomProduit = string.Empty;
        private string _description = string.Empty;
        private string cleanDescription = string.Empty;
        private double prix;
        private int quantiteDisponible;
        private string descriptionCourte = string.Empty;
        private List<ProductImage> lesImages = new List<ProductImage>();
        private List<Promotion> promotions = new List<Promotion>();
        #endregion

        #region constructeurs
        public Product()
        {
            // Constructeur par défaut
        }

        public Product(int id, string nomProduit, string description, double prix, int quantiteDisponible,
                     string descriptionCourte, List<ProductImage> lesImages, List<Promotion> promotions)
        {
            this.id = id;
            this.nomProduit = nomProduit;
            this.Description = description; // Utilisez la propriété pour déclencher le nettoyage
            this.prix = prix;
            this.quantiteDisponible = quantiteDisponible;
            this.descriptionCourte = descriptionCourte;
            this.lesImages = lesImages ?? new List<ProductImage>();
            this.promotions = promotions ?? new List<Promotion>();
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
            set => nomProduit = value ?? string.Empty;
        }

        [JsonPropertyName("description")]
        public string Description
        {
            get => _description;
            set
            {
                if (value == null)
                {
                    _description = string.Empty;
                    cleanDescription = string.Empty;
                    return;
                }

                _description = value;
                cleanDescription = System.Text.RegularExpressions.Regex
                    .Replace(value, "<[^>]*>", "")
                    .Replace("&nbsp;", " ")
                    .Replace("\r\n", " ")
                    .Replace("\n", " ")
                    .Trim();
            }
        }

        public string CleanDescription
        {
            get => cleanDescription;
            private set => cleanDescription = value;
        }

        public double Prix
        {
            get => prix;
            set => prix = value;
        }

        [JsonPropertyName("quantiteDisponible")]
        public int QuantiteDisponible
        {
            get => quantiteDisponible;
            set => quantiteDisponible = value;
        }

        [JsonPropertyName("descriptioncourte")]
        public string DescriptionCourte
        {
            get => descriptionCourte;
            set => descriptionCourte = value ?? string.Empty;
        }

        [JsonPropertyName("lesImages")]
        public List<ProductImage> LesImages
        {
            get => lesImages;
            set => lesImages = value ?? new List<ProductImage>();
        }

        [JsonPropertyName("lesPromos")]
        public List<Promotion> Promotions
        {
            get => promotions;
            set => promotions = value ?? new List<Promotion>();
        }
        #endregion

        #region propriétés calculées
        [JsonIgnore]
        public Promotion ActivePromotion => Promotions?.FirstOrDefault(p => p.IsActive);

        [JsonIgnore]
        public bool HasActivePromotion => ActivePromotion != null;

        [JsonIgnore]
        public double PromotionalPrice => HasActivePromotion ? ActivePromotion.Prix : Prix;

        [JsonIgnore]
        public double DiscountPercentage => HasActivePromotion ? Math.Round((1 - (PromotionalPrice / Prix)) * 100) : 0;
        #endregion
    }

    public class ProductImage
    {
        #region attributs
        private int id;
        private string _url = string.Empty;
        #endregion

        #region constructeurs
        public ProductImage()
        {
            // Constructeur par défaut
        }

        public ProductImage(int id, string url)
        {
            this.id = id;
            this.Url = url; // Utilise la propriété pour appliquer la logique de formatage
        }
        #endregion

        #region getters/setters
        public int Id
        {
            get => id;
            set => id = value;
        }

        public string Url
        {
            get => _url;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    // Vérifier si l'URL est déjà complète ou a besoin d'être préfixée
                    if (value.StartsWith("http"))
                    {
                        _url = value;
                    }
                    else
                    {
                        _url = $"http://213.130.144.159/{value.TrimStart('/')}";
                    }
                }
                else
                {
                    _url = string.Empty;
                }
            }
        }
        #endregion
    }
}