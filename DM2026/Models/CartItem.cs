using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DM2026.Models
{
    public class CartItem : INotifyPropertyChanged
    {
        #region attributs
        private int _quantity;
        private Product product;
        #endregion

        #region constructeurs
        public CartItem()
        {
            // Constructeur par défaut
        }

        public CartItem(Product product, int quantity)
        {
            this.product = product;
            this._quantity = quantity;
        }
        #endregion

        #region getters/setters
        public int Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TotalPrice));
                }
            }
        }

        public Product Product
        {
            get => product;
            set => product = value;
        }
        #endregion

        #region propriétés calculées
        // Prend en compte le prix promotionnel si disponible
        public double TotalPrice => Product.HasActivePromotion
            ? Product.PromotionalPrice * Quantity
            : Product.Prix * Quantity;

        // Prix unitaire effectif (standard ou promotionnel)
        public double UnitPrice => Product.HasActivePromotion
            ? Product.PromotionalPrice
            : Product.Prix;
        #endregion

        #region gestion événements
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}