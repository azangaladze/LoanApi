using System.Text.Json.Serialization;

namespace LoanProject.Infrastructure.Models
{
    public class LoanModel
    {
        [JsonIgnore]
        public int Id { get; set; }
        [JsonIgnore]
        public int UserId { get; set; }
        public string Type { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
        public int Period { get; set; }
        [JsonIgnore]
        public string Status { get; set; }
    }
}
