namespace Crypto.Pylot.Functions.Models
{
    public class Alert
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CryptocurrencyId { get; set; }
        public double TargetPrice { get; set; }
        public bool Notified { get; set; }
        public bool OverThePrice { get; set; }
    }
}
