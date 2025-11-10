using System;
using System.Text.Json.Serialization;

namespace DantecMarketApp.Models
{
    public class Promotion
    {
        #region attributs
        private int id;
        private string dateDebutString;
        private string dateFinString;
        private double prix;
        private int productId;
        private PromoCategory promotionCategory;
        #endregion

        #region constructeurs
        public Promotion()
        {
            // Constructeur par défaut
        }

        public Promotion(int id, string dateDebutString, string dateFinString, double prix, int productId, PromoCategory promotionCategory)
        {
            this.id = id;
            this.dateDebutString = dateDebutString;
            this.dateFinString = dateFinString;
            this.prix = prix;
            this.productId = productId;
            this.promotionCategory = promotionCategory;
        }
        #endregion

        #region getters/setters
        public int Id
        {
            get => id;
            set => id = value;
        }

        [JsonPropertyName("dateDebut")]
        public string DateDebutString
        {
            get => dateDebutString;
            set => dateDebutString = value;
        }

        [JsonPropertyName("dateFin")]
        public string DateFinString
        {
            get => dateFinString;
            set => dateFinString = value;
        }

        [JsonPropertyName("prix")]
        public double Prix
        {
            get => prix;
            set => prix = value;
        }

        [JsonPropertyName("leProduit")]
        public int ProductId
        {
            get => productId;
            set => productId = value;
        }

        [JsonPropertyName("laCategoriePromo")]
        public PromoCategory PromotionCategory
        {
            get => promotionCategory;
            set => promotionCategory = value;
        }
        #endregion

        #region propriétés calculées
        [JsonIgnore]
        public DateTime DateDebut => DateTime.TryParse(DateDebutString, out var result) ? result : DateTime.MinValue;

        [JsonIgnore]
        public DateTime DateFin => DateTime.TryParse(DateFinString, out var result) ? result : DateTime.MinValue;

        [JsonIgnore]
        public bool IsActive => DateTime.Now >= DateDebut && DateTime.Now <= DateFin;
        #endregion
    }

    public class PromoCategory
    {
        #region attributs
        private int id;
        private string nom;
        private List<object> promotionIds;
        #endregion

        #region constructeurs
        public PromoCategory()
        {
            promotionIds = new List<object>();
        }

        public PromoCategory(int id, string nom, List<object> promotionIds)
        {
            this.id = id;
            this.nom = nom;
            this.promotionIds = promotionIds ?? new List<object>();
        }
        #endregion

        #region getters/setters
        public int Id
        {
            get => id;
            set => id = value;
        }

        [JsonPropertyName("nom")]
        public string Nom
        {
            get => nom;
            set => nom = value;
        }

        [JsonPropertyName("lesPromos")]
        public List<object> PromotionIds
        {
            get => promotionIds;
            set => promotionIds = value ?? new List<object>();
        }
        #endregion
    }
}