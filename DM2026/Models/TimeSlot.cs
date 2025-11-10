using System.Text.Json.Serialization;
using System.Globalization;

namespace DM2026.Models
{
    public class TimeSlot
    {
        #region attributs
        private int id;
        private string jourString;
        private string heureDebutString;
        private string heureFinString;
        private DateTime _jour;
        private DateTime _heureDebut;
        private DateTime _heureFin;
        #endregion

        #region constructeurs
        public TimeSlot()
        {
            // Constructeur par défaut
        }

        public TimeSlot(int id, string jourString, string heureDebutString, string heureFinString)
        {
            this.id = id;
            this.jourString = jourString;
            this.heureDebutString = heureDebutString;
            this.heureFinString = heureFinString;
        }
        #endregion

        #region getters/setters
        public int Id
        {
            get => id;
            set => id = value;
        }

        [JsonPropertyName("jour")]
        public string JourString
        {
            get => jourString;
            set => jourString = value;
        }

        [JsonPropertyName("heureDebut")]
        public string HeureDebutString
        {
            get => heureDebutString;
            set => heureDebutString = value;
        }

        [JsonPropertyName("heureFin")]
        public string HeureFinString
        {
            get => heureFinString;
            set => heureFinString = value;
        }
        #endregion

        #region propriétés calculées
        [JsonIgnore]
        public DateTime Jour
        {
            get
            {
                if (_jour == default)
                {
                    DateTime.TryParse(JourString, CultureInfo.InvariantCulture, DateTimeStyles.None, out _jour);
                }
                return _jour;
            }
        }

        [JsonIgnore]
        public DateTime HeureDebut
        {
            get
            {
                if (_heureDebut == default)
                {
                    DateTime.TryParse(HeureDebutString, CultureInfo.InvariantCulture, DateTimeStyles.None, out _heureDebut);
                }
                return _heureDebut;
            }
        }

        [JsonIgnore]
        public DateTime HeureFin
        {
            get
            {
                if (_heureFin == default)
                {
                    DateTime.TryParse(HeureFinString, CultureInfo.InvariantCulture, DateTimeStyles.None, out _heureFin);
                }
                return _heureFin;
            }
        }

        // Pour l'affichage, on extrait seulement la partie date du champ jour
        public string FormattedDay => Jour.ToString("dd/MM/yyyy");

        // Pour l'affichage, on extrait seulement la partie heure des champs heureDebut et heureFin
        public string FormattedTimeRange => $"{HeureDebut.ToString("HH:mm")} - {HeureFin.ToString("HH:mm")}";

        // Pour l'affichage dans le Picker
        public string DisplayText => $"{FormattedDay} ({FormattedTimeRange})";
        #endregion
    }

    public class ReservationRequest
    {
        #region attributs
        private int idUser;
        private string jour;
        private string heureDebut;
        private int commandeId;
        private int id;
        #endregion

        #region constructeurs
        public ReservationRequest()
        {
            // Constructeur par défaut
        }

        public ReservationRequest(int idUser, string jour, string heureDebut, int commandeId, int id)
        {
            this.idUser = idUser;
            this.jour = jour;
            this.heureDebut = heureDebut;
            this.commandeId = commandeId;
            this.id = id;
        }
        #endregion

        #region getters/setters
        [JsonPropertyName("idUser")]
        public int IdUser
        {
            get => idUser;
            set => idUser = value;
        }

        [JsonPropertyName("jour")]
        public string Jour
        {
            get => jour;
            set => jour = value;
        }

        [JsonPropertyName("heureDebut")]
        public string HeureDebut
        {
            get => heureDebut;
            set => heureDebut = value;
        }

        [JsonPropertyName("commandeId")]
        public int CommandeId
        {
            get => commandeId;
            set => commandeId = value;
        }

        [JsonPropertyName("id")]
        public int Id
        {
            get => id;
            set => id = value;
        }
        #endregion
    }
}