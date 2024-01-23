namespace ProjektODASAPI.Models
{
    public class ChangePasswordResponse
    {

        public string Login { get; set; } = null;

        public string OldPassword { get; set; } = null;

        public string NewPassword { get; set; } = null;
        public List<int> numbersT { get; set; } = null;
    }
}
