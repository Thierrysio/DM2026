using System.Text.Json.Serialization;

namespace DM2026.Models
{
    public class Order
    {
        #region attributs
        private int id;
        private string dateCommande;
        private double montantTotal;
        private List<OrderItem> lesCommandes;
        private string etat;
        private string planningDetails;
        #endregion

        #region constructeurs
        public Order()
        {
            lesCommandes = new List<OrderItem>();
        }

        public Order(int id, string dateCommande, double montantTotal, List<OrderItem> lesCommandes, string etat, string planningDetails)
        {
            this.id = id;
            this.dateCommande = dateCommande;
            this.montantTotal = montantTotal;
            this.lesCommandes = lesCommandes ?? new List<OrderItem>();
            this.etat = etat;
            this.planningDetails = planningDetails;
        }
        #endregion

        #region getters/setters
        public int Id
        {
            get => id;
            set => id = value;
        }

        [JsonPropertyName("dateCommande")]
        public string DateCommande
        {
            get => dateCommande;
            set => dateCommande = value;
        }

        [JsonPropertyName("montantTotal")]
        public double MontantTotal
        {
            get => montantTotal;
            set => montantTotal = value;
        }

        [JsonPropertyName("lesCommandes")]
        public List<OrderItem> LesCommandes
        {
            get => lesCommandes;
            set => lesCommandes = value ?? new List<OrderItem>();
        }

        [JsonPropertyName("etat")]
        public string Etat
        {
            get => etat;
            set => etat = value;
        }

        [JsonPropertyName("planningDetails")]
        public string PlanningDetails
        {
            get => planningDetails;
            set => planningDetails = value;
        }
        #endregion
    }

    public class OrderItem
    {
        #region attributs
        private int id;
        private int quantite;
        private Product leProduit;
        private double prixRetenu;
        #endregion

        #region constructeurs
        public OrderItem()
        {
            // Constructeur par défaut
        }

        public OrderItem(int id, int quantite, Product leProduit, double prixRetenu)
        {
            this.id = id;
            this.quantite = quantite;
            this.leProduit = leProduit;
            this.prixRetenu = prixRetenu;
        }
        #endregion

        #region getters/setters
        public int Id
        {
            get => id;
            set => id = value;
        }

        public int Quantite
        {
            get => quantite;
            set => quantite = value;
        }

        [JsonPropertyName("leProduit")]
        public Product LeProduit
        {
            get => leProduit;
            set => leProduit = value;
        }

        [JsonPropertyName("prixretenu")]
        public double PrixRetenu
        {
            get => prixRetenu;
            set => prixRetenu = value;
        }
        #endregion
    }

    // La classe commentée est conservée en commentaire comme dans le fichier original
    /*
    public class OrderResponse
    {
        [JsonPropertyName("commandes")]
        public List<Order> Commandes { get; set; }
    }
    */
}