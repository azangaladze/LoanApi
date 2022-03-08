using LoanProject.Core.FieldStrings;
using System.Text.Json.Serialization;

namespace LoanProject.Core.Entities
{
    public class Loan
    {
        
        public int Id { get; set; }
        public string Type { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
        public int Period { get; set; }
        public string Loanstatus { get; set; } = LoanStatus.Processing;
        public int UserId { get; set; }
        [JsonIgnore]
        public User User { get; set; }
    }
}
