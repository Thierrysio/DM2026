using System;
using System.Text.Json.Serialization;

namespace DM2026.Models
{
    public class TopSellingProduct
    {
        #region attributs
        private int id;
        private string nomProduit;
        private string descriptionCourte;
        private double prix;
        private string quantiteVendue;
        private string image;
        private double? prixPromo;
        private string nomCategoriePromo;
        #endregion

        #region constructeurs
        public TopSellingProduct()
        {
            this.nomProduit = string.Empty;
            this.descriptionCourte = string.Empty;
            this.quantiteVendue = string.Empty;
            this.image = string.Empty;
            this.nomCategoriePromo = string.Empty;
        }

        public TopSellingProduct(int id, string nomProduit, string descriptionCourte, double prix,
                                string quantiteVendue, string image, double? prixPromo = null,
                                string nomCategoriePromo = "")
        {
            this.id = id;
            this.nomProduit = nomProduit;
            this.descriptionCourte = descriptionCourte;
            this.prix = prix;
            this.quantiteVendue = quantiteVendue;
            this.image = image;
            this.prixPromo = prixPromo;
            this.nomCategoriePromo = nomCategoriePromo;
        }
        #endregion

        #region getters/setters
        public int Id { get => id; set => id = value; }

        [JsonPropertyName("nomProduit")]
        public string NomProduit { get => nomProduit; set => nomProduit = value; }

        [JsonPropertyName("descriptioncourte")]
        public string DescriptionCourte { get => descriptionCourte; set => descriptionCourte = value; }

        public double Prix { get => prix; set => prix = value; }

        [JsonPropertyName("quantite_vendue")]
        public string QuantiteVendue { get => quantiteVendue; set => quantiteVendue = value; }

        public string Image { get => image; set => image = value; }

        [JsonPropertyName("prixpromo")]
        public double? PrixPromo { get => prixPromo; set => prixPromo = value; }

        [JsonPropertyName("nomCategoriePromo")]
        public string NomCategoriePromo { get => nomCategoriePromo; set => nomCategoriePromo = value; }
        #endregion

        #region propriétés calculées
        [JsonIgnore]
        public bool HasPromo => PrixPromo.HasValue;

        [JsonIgnore]
        public string ImageUrl => !string.IsNullOrEmpty(Image) ?
            (Image.StartsWith("http") ? Image : $"http://213.130.144.159/{Image.TrimStart('/')}") :
            string.Empty;

        [JsonIgnore]
        public double DisplayPrice => PrixPromo ?? Prix;

        [JsonIgnore]
        public double DiscountPercentage => HasPromo ? Math.Round((1 - (PrixPromo.Value / Prix)) * 100) : 0;
        #endregion

        #region méthodes
        public string GetProductInfo()
        {
            return $"{NomProduit} - {DisplayPrice}€";
        }

        public string GetPromoDescription()
        {
            if (HasPromo)
            {
                return $"Promotion: {DiscountPercentage}% de réduction";
            }
            return "Pas de promotion en cours";
        }
        #endregion
    }
}