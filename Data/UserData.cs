using System.ComponentModel.DataAnnotations;

namespace ProjektODASAPI.Data
{
    public class UserData
    {
        [Key]
        public int Id { get; set; }
        [Encrypted]
        public string Login { get; set; } = null;
        [Encrypted]
        public string PasswordSet { get; set; } = null;
        [Encrypted]
        public string CardNumber { get; set; } = null;
        [Encrypted]
        public string DocumentNumber { get; set; } = null;
        [Encrypted]
        public string Balance { get; set; } = null;
        [Encrypted]
        public string Salt { get; set; } = null;
        [Encrypted]
        public string TransferHistory { get; set; } = null;
        [Encrypted]
        public string AccountNumber { get; set; } = null;
    }
}
