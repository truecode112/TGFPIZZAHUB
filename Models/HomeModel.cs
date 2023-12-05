namespace TGFPIZZAHUB.Models
{
    public class HomeModel
    {
        public string? ClientID { get; set; }
        public string? AuthCode { get; set; } 
        public string? AccessToken { get; set; }

        public string? AccountId { get; set; }
        public string? AccountName { get; set; }

        public string? LocationId { get; set;}
        public string? LocationName { get; set;}
        public string? CatalogId { get; set; }
        public string? CatalogName { get; set;
        }
    }
}
