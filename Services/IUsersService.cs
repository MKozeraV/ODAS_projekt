using ProjektODASAPI.Models;

namespace ProjektODASAPI.Services
{
    public interface IUsersService
    {
        Task<MainResponse> AddUser(UserDTO userDTO);
        Task<MainResponse> GetUsersData(string Login);
        Task<MainResponse> GetAllUsers();
        Task<MainResponse> ChangePassword(ChangePasswordResponse changeDTO);
        List<int> GetNumbersAndNumber(string passwordString);
        string GetPasswordHash(string passwordString, List<int> numbersList);
        Task<MainResponse> OperateTransfer(TransferData transferData);
        Task<MainResponse> GetWholePassword(string login);
        MainResponse GetWholePasswordSync(string login);
        string Hasher2(string password, byte[] salt);
        User AuthenticateUser(PasswordLogin user);
        public string GenerateToken(User user);
        public string GetMyLogin();
    }
}
