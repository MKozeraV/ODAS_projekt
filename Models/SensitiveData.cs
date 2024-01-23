namespace ProjektODASAPI.Models
{
    public class SensitiveData
    {
        public string CardNumber { get; set; } = null;

        public string DocumentNumber { get; set; } = null;
        public string Balance { get; set; } = null;
        public string AccountNumber { get; set; } = null;

        public SensitiveData(string CardNumber, string DocumentNumber, string Balance, string AccountNumber) 
        {
            this.CardNumber = CardNumber;
            this.DocumentNumber = DocumentNumber;
            this.Balance = Balance;
            this.AccountNumber = AccountNumber;
        }

    }
}
