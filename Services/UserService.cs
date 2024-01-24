using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using ProjektODASAPI.Context;
using ProjektODASAPI.Data;
using ProjektODASAPI.Models;
using System.Collections;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace ProjektODASAPI.Services
{
    public class UserService : IUsersService
    {
        private IConfiguration _configuration;
        private readonly BankContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserService(BankContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor; 
        }
        //do usuniecia
        public async Task<MainResponse> AddUser(UserDTO userDTO)
        {
            var response = new MainResponse();
            try
            {

                await _context.AddAsync(new UserData
                {
                    Login = userDTO.Login,
                    PasswordSet = userDTO.PasswordSet,
                    CardNumber = userDTO.CardNumber,
                    DocumentNumber = userDTO.DocumentNumber,
                    Balance = userDTO.Balance,
                    Salt = userDTO.Salt,
                    TransferHistory = userDTO.TransferHistory,
                    AccountNumber=userDTO.AccountNumber
                });

                await _context.SaveChangesAsync();
                response.IsSuccess = true;
                response.Content = "User added";
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
                response.IsSuccess = false;
            }
            return response;

        }
        public async Task<MainResponse> GetUsersData(string Login)
        {
            var response = new MainResponse();
            try
            {
                /*public string CardNumber { get; set; } = null;

        public string DocumentNumber { get; set; } = null;
        public string Balance { get; set; }
                public string AccountNumber { get; set; } = null;
                 */
                object? Content = await _context.Users.Where(f => f.Login == Login).FirstOrDefaultAsync();
                if(Content == null)
                {
                    response.ErrorMessage = "No such user";
                    response.IsSuccess = false;
                    return response;
                }
                System.Reflection.PropertyInfo pi = Content.GetType().GetProperty("CardNumber");
                String CardN = (String)(pi.GetValue(Content, null));

                System.Reflection.PropertyInfo pi1 = Content.GetType().GetProperty("DocumentNumber");
                String DocumentN = (String)(pi1.GetValue(Content, null));

                System.Reflection.PropertyInfo pi2 = Content.GetType().GetProperty("Balance");
                String Balance = (String)(pi2.GetValue(Content, null));

                System.Reflection.PropertyInfo pi3 = Content.GetType().GetProperty("AccountNumber");
                String AccountN = (String)(pi3.GetValue(Content, null));

                SensitiveData responseData = new SensitiveData(CardN, DocumentN, Balance, AccountN)
                {
                    CardNumber = CardN,
                    DocumentNumber = DocumentN,
                    Balance = Balance,
                    AccountNumber = AccountN
                };
                response.Content = responseData;
                //response.Content = await _context.Users.Where(f => f.Login == Login).FirstOrDefaultAsync();
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
                response.IsSuccess = false;
            }
            return response;
        }





        //do usuniecia
        public async Task<MainResponse> GetAllUsers()
        {
            var response = new MainResponse();
            try
            {
                response.Content = await _context.Users.ToListAsync();
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
                response.IsSuccess = false;
            }
            return response;
        }

        public async Task<MainResponse> ChangePassword(ChangePasswordResponse changeDTO)
        {
            var response = new MainResponse();
            try
            {   //
                /*var existingUserT = _context.Users.Where(f => f.Login == changeDTO.Login).FirstOrDefault();
                byte[] salt3 = System.Security.Cryptography.RandomNumberGenerator.GetBytes(128 / 8);
                existingUserT.Salt = Convert.ToBase64String(salt3);
                _context.SaveChanges();
                existingUserT = _context.Users.Where(f => f.Login == changeDTO.Login).FirstOrDefault();
                System.Diagnostics.Debug.WriteLine("KROK 111111111111");
                var passwordShuffledAndSalt1 = PasswordShuffler(changeDTO.NewPassword, existingUserT.Salt);
                System.Diagnostics.Debug.WriteLine("KROK 22222222222222");
                //string passwordShuffled = PasswordShuffler(changeDTO.NewPassword);
                existingUserT.PasswordSet = passwordShuffledAndSalt1.Item1;
                //existingUser.Salt = Convert.ToBase64String(passwordShuffledAndSalt.Item2);
                await _context.SaveChangesAsync();
                response.IsSuccess = true;
                response.Content = "SKONCZYLEM ROBOTE";
                System.Diagnostics.Debug.WriteLine(passwordShuffledAndSalt1.Item1);
                System.Diagnostics.Debug.WriteLine(passwordShuffledAndSalt1.Item2);
                return response;*/




                if (changeDTO.NewPassword.Length <10)
                {
                    response.IsSuccess = false;
                    response.Content = "Password is too weak";
                    return response;
                }
                double passwordEntropy = CalculateEntropy(changeDTO.NewPassword);
                if(passwordEntropy <60) 
                {
                    System.Diagnostics.Debug.WriteLine("HASLO JEST SLABE - NIE POWINIENES GO UZYWAC");
                    response.IsSuccess = false;
                    response.Content = "Password is too weak";
                    return response;
                }
                MainResponse responsePas = GetWholePasswordSync(changeDTO.Login);
                if (responsePas.IsSuccess != true)
                {
                    response.IsSuccess = false;
                    response.Content = "Can not change password-error1";
                    return response;
                }
                
                string passwordLong = (string)responsePas.Content;
                string correctPassword = GetPasswordHash(passwordLong, changeDTO.numbersT);

                string[] splitted = passwordLong.Split('*');
                string passwordRealLong = splitted[0];
                string salt1 = splitted[1];
                byte[] salt2 = Encoding.UTF8.GetBytes(salt1);
                string UserHashedPassword = Hasher2(changeDTO.OldPassword, salt2);
                //
                var existingUser = _context.Users.Where(f => f.Login == changeDTO.Login && UserHashedPassword == correctPassword).FirstOrDefault();
                if (existingUser != null)
                {
                    byte[] salt = System.Security.Cryptography.RandomNumberGenerator.GetBytes(128 / 8);
                    existingUser.Salt = Convert.ToBase64String(salt);
                    _context.SaveChanges();
                    existingUser = _context.Users.Where(f => f.Login == changeDTO.Login && UserHashedPassword == correctPassword).FirstOrDefault();
                    var passwordShuffledAndSalt = PasswordShuffler(changeDTO.NewPassword, existingUser.Salt);
                    //string passwordShuffled = PasswordShuffler(changeDTO.NewPassword);
                    existingUser.PasswordSet = passwordShuffledAndSalt.Item1;
                    //existingUser.Salt = Convert.ToBase64String(passwordShuffledAndSalt.Item2);
                    await _context.SaveChangesAsync();
                    response.IsSuccess = true;
                    response.Content = "Password updated";
                }
                else
                {
                    response.IsSuccess = false;
                    response.Content = "Can not change password-error2";
                }


            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
                response.IsSuccess = false;
            }
            return response;

        }
        //shuffler hasher 
        private (string,byte[]) PasswordShuffler(string passwordToShuffle, string saltS)
        {
            byte[] s = {};
            byte[] salt = Encoding.ASCII.GetBytes(saltS);
            string returnString = "";
            for (int j = 0; j < 10; j++)
            {
                Randomizer randomizer = new Randomizer();
                List<int> Lista;
                while (randomizer.randomList.Count < 4)
                {
                    randomizer.NewNumber();
                }
                Lista = new List<int>(randomizer.randomList);
                Lista.Sort();
                int length = passwordToShuffle.Length;
                string template = new string('.', length);
                string cutPas = "";
                StringBuilder sb1 = new StringBuilder(passwordToShuffle);
                for (int i = 0; i < Lista.Count; i++)
                {
                    StringBuilder sb = new StringBuilder(template);
                    if(i!=0)
                    cutPas = cutPas + passwordToShuffle[Lista.ElementAt(i)];
                    sb[Lista.ElementAt(i)] = '<';
                    template = sb.ToString();
                }
                //string hashedP = Hasher(cutPas);
                var hashedAndSalt = Hasher(cutPas, salt);
                returnString = returnString + hashedAndSalt.Item1 + "|" + template + "?";
                s = hashedAndSalt.Item2;
            }
            return (returnString, s);

        }
        private (string,byte[]) Hasher(string password, byte[] salt)
        {
            //hashowanie 
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: password!,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8));
            return (hashed,salt);
        }
        public string Hasher2(string password, byte[] salt)
        {
            //hashowanie 
            
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: password!,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8));
            return hashed;
        }

        //
        public List<int> GetNumbersAndNumber(string passwordString)
        {
            string[] splitted = passwordString.Split('?');
            Random a = new Random();
            int templateNumber = a.Next(0, 9);
            string passwordAndTemplate = splitted[templateNumber];
            string[] s1 = passwordAndTemplate.Split('|');
            string pass = s1[0];
            string template= s1[1];
            List<int> numbers = new List<int>();
            for (int i=0;i<template.Length;i++)
            {
                if (template[i] == '<')
                {
                    numbers.Add(i);
                }
            }
            numbers.Add(templateNumber);
            return numbers;
        }
        public string GetPasswordHash(string passwordString, List<int> numbersList)
        {
            string[] splitted = passwordString.Split('?');
            string passwordAndTemplate = splitted[numbersList.Last()];
            string[] s1 = passwordAndTemplate.Split('|');
            string pass = s1[0];
            string template = s1[1];
            return pass;
        }
        public async Task<MainResponse> GetWholePassword(string login)
        {
            var response = new MainResponse();
            try
            {
                object? Content = await _context.Users.Where(f => f.Login == login).FirstOrDefaultAsync();
                if (Content == null)
                {
                    response.ErrorMessage = "No such user";
                    response.IsSuccess = false;
                    return response;
                }
                System.Reflection.PropertyInfo pi = Content.GetType().GetProperty("PasswordSet");
                String WholePassword = (String)(pi.GetValue(Content, null));

                response.Content = WholePassword;
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
                response.IsSuccess = false;
            }
            return response;
        }
        
        public MainResponse GetWholePasswordSync(string login)
        {
            var response = new MainResponse();
            try
            {
                object? Content =  _context.Users.Where(f => f.Login == login).FirstOrDefault();
                if (Content == null)
                {
                    response.ErrorMessage = "No such user";
                    response.IsSuccess = false;
                    return response;
                }
                System.Reflection.PropertyInfo pi = Content.GetType().GetProperty("PasswordSet");
                string WholePassword = (string)(pi.GetValue(Content, null));
                System.Reflection.PropertyInfo pi1 = Content.GetType().GetProperty("Salt");
                string Salt = (string)(pi1.GetValue(Content, null));
                response.Content = WholePassword+'*'+Salt;
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
                response.IsSuccess = false;
            }
            return response;
        }


        public User AuthenticateUser(PasswordLogin user)
        {
            try
            {
                User _user = null;
                var response = GetWholePasswordSync(user.Login);
                if (response.Content == null)
                {
                    return _user;
                }
                List<int> numbers = new List<int>();
                numbers = user.NumbersT;
                string passwordLong = (string)response.Content;
                string correctPassword = GetPasswordHash(passwordLong, numbers);

                string[] splitted = passwordLong.Split('*');
                string passwordRealLong = splitted[0];
                string salt1 = splitted[1];
                byte[] salt2 = Encoding.UTF8.GetBytes(salt1);
                string UserHashedPassword = Hasher2(user.PasswordSet, salt2);

                if (correctPassword == UserHashedPassword)
                {
                    _user = new User { Login = user.Login };
                }
                return _user;
            }
            catch (Exception ex)
            {
                User _usernull = null;
                return _usernull;
            }
        }


        public string GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Login)
            };


            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], 
                _configuration["Jwt:Audience"], 
                claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public async Task<MainResponse> OperateTransfer(TransferData transferData)
        {
            var response = new MainResponse();
           /* bool validation = validateTransferData(transferData);
            if (!validation)
            {
                response.IsSuccess = false;
                response.Content = "Transfer data is bad, please change it to right one. Transfer is aborted.";
                return response;
            }*/
                try
                {
                var existingUser = _context.Users.Where(f => f.AccountNumber == transferData.accountNumberSender).FirstOrDefault();
                var existingUserTransferGoal = _context.Users.Where(f => f.AccountNumber == transferData.accountNumberReceiver).FirstOrDefault();
                if (existingUser != null && existingUserTransferGoal != null)
                {
                    if(transferData.amount<0 || transferData.amount> Double.Parse(existingUser.Balance))
                    {
                        response.IsSuccess = false;
                        response.Content = "This amount of money is not accepted. Please change it";
                        return response;
                    }
                    double senderBalance = Double.Parse(existingUser.Balance);
                    double receiverBalance=Double.Parse(existingUserTransferGoal.Balance);
                    senderBalance = senderBalance - transferData.amount;
                    receiverBalance = receiverBalance + transferData.amount;
                    existingUser.Balance = senderBalance.ToString();
                    existingUserTransferGoal.Balance = receiverBalance.ToString();
                    if(existingUser.TransferHistory == "none")
                    {
                        existingUser.TransferHistory = "";
                    }
                    existingUser.TransferHistory = existingUser.TransferHistory + transferData.title + '>' + transferData.name + '>'
                        + transferData.accountNumberReceiver + '>' + transferData.amount.ToString() +'>'+"Sent"+ '|';
                    await _context.SaveChangesAsync();
                    response.IsSuccess = true;
                    response.Content = "Money transfer went well";
                }
                else
                {
                    response.IsSuccess = false;
                    response.Content = "Something is wrong with number account. Please change it";
                }


            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
                response.IsSuccess = false;
            }
            return response;
        }

        public string GetMyLogin()
        {
            var userName = string.Empty;
            if(_httpContextAccessor.HttpContext is not null)
            {
                userName = _httpContextAccessor.HttpContext.User?.Identity?.Name;
            }
            return userName;
        }

        public double CalculateEntropy(string password)
        {
            var cardinality = 0;

            // Password contains lowercase letters.
            if (password.Any(c => char.IsLower(c)))
            {
                cardinality = 26;
            }

            // Password contains uppercase letters.
            if (password.Any(c => char.IsUpper(c)))
            {
                cardinality += 26;
            }

            // Password contains numbers.
            if (password.Any(c => char.IsDigit(c)))
            {
                cardinality += 10;
            }

            // Password contains symbols.
            if (password.IndexOfAny("\\|¬¦`!\"£$%^&*()_+-=[]{};:'@#~<>,./? ".ToCharArray()) >= 0)
            {
                cardinality += 36;
            }

            return Math.Log(cardinality, 2) * password.Length;
        }

        public async Task<MainResponse> GetTransferHistory(string user)
        {
            var response = new MainResponse();
            try
            {
        
                object? Content = await _context.Users.Where(f => f.Login == user).FirstOrDefaultAsync();
                if (Content == null)
                {
                    response.ErrorMessage = "No such user";
                    response.IsSuccess = false;
                    return response;
                }
                System.Reflection.PropertyInfo pi = Content.GetType().GetProperty("TransferHistory");
                String Hist = (String)(pi.GetValue(Content, null));



                
                response.Content = Hist;
                //response.Content = await _context.Users.Where(f => f.Login == Login).FirstOrDefaultAsync();
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
                response.IsSuccess = false;
            }
            return response;
        }


        //do usuniecia
        public async Task<MainResponse> ChangePassword222(TESTCHANGE1 changeDTO)
        {
            var response = new MainResponse();
            var existingUser = _context.Users.Where(f => f.Login == changeDTO.Login).FirstOrDefault();
            if (existingUser != null)
            {
                existingUser.PasswordSet = changeDTO.NewPassword;
                existingUser.Salt = changeDTO.Salt;
                await _context.SaveChangesAsync();
                response.IsSuccess = true;
                response.Content = "Password updated";
            }
            response.IsSuccess = false;
            response.ErrorMessage = "did not work";
            return response;
        }

    } 
}
