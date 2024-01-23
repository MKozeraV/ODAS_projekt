using System.ComponentModel.DataAnnotations;

namespace ProjektODASAPI.Models
{
    public class UserDTO
    {
        public int Id { get; set; }

        public string Login { get; set; } = null;

        public string PasswordSet { get; set; } = null;

        public string CardNumber { get; set; } = null;

        public string DocumentNumber { get; set; } = null;
        public string Balance { get; set; }

        public string Salt { get; set; } = null;
        public string TransferHistory { get; set; } = null;
        public string AccountNumber { get; set; } = null;
    }
}
