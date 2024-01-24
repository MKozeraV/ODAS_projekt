using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.DataEncryption.Providers;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using ProjektODASAPI.Context;
using ProjektODASAPI.Models;
using ProjektODASAPI.Services;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ProjektODASAPI.Controllers
{
  
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private List<NumberListAndUsername> currentUsers = new List<NumberListAndUsername>();
        private List<String> currentUsersString = new List<String>();
        private readonly BankContext _bankContext;
        private readonly    IUsersService _usersService;
        private IConfiguration _configuration;
        public UsersController(BankContext bankContext, IUsersService usersService, IConfiguration configuration) 
        {
            _bankContext = bankContext;
            _usersService = usersService;
            _configuration = configuration;
        }




        [Authorize]
        [HttpGet("GetUserByLogin/{Login}")]
        public async Task<IActionResult> GetUser(string Login)
        {
            try
            {
                var identity = _usersService.GetMyLogin();
                if (identity != Login)
                {
                    return BadRequest("Please stop trying to steal other people data");
                }



                var response = await _usersService.GetUsersData(Login);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
       /* 
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var response = await _usersService.GetAllUsers();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }*/
        [Authorize]
        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePasswd([FromBody] ChangePasswordResponse changePas)
        {
            try
            {
                var identity = _usersService.GetMyLogin();
                if(identity != changePas.Login) 
                {
                    return BadRequest("Please stop trying to steal other people data");
                }


                    var response = await _usersService.ChangePassword(changePas);
                if (response.IsSuccess == true)
                {
                    return Ok(response);
                }
                else
                    return BadRequest(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
       /* [HttpPut("ChangePassword222")]
        public async Task<IActionResult> ZMIANATESTOWA([FromBody] TESTCHANGE1 changePas)
        {
            try
            {
                /*var identity = _usersService.GetMyLogin();
                if(identity != changePas.Login) 
                {
                    return BadRequest("Please stop trying to steal other people data");
                }


                var response = await _usersService.ChangePassword222(changePas);
                if (response.IsSuccess == true)
                {
                    return Ok(response);
                }
                else
                    return BadRequest(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
*/
        //zwraca liste numerkow z hasla 
        [AllowAnonymous]
        [HttpGet("GetNumbers")]
        public async Task<IActionResult> GetPasswordNumbers(string Login)
        {
            try
            {
                var response = await _usersService.GetWholePassword(Login);
                if (response.IsSuccess == false)
                {
                    return BadRequest("Wrong username");
                }
                string pass = response.Content.ToString();
                List<int> numbers = new List<int>();
                numbers = _usersService.GetNumbersAndNumber(pass);
                var response1 = new MainResponse();
                response1.IsSuccess = true;
                response1.Content = numbers;

                return Ok(response1);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login([FromBody] PasswordLogin user)
        {
            int milliseconds = 5000;
            Thread.Sleep(milliseconds);
            IActionResult response = Unauthorized();
            var user_ = _usersService.AuthenticateUser(user);
            if (user_ != null)
            {
                var token = _usersService.GenerateToken(user_);
                response = Ok(new {token = token});
            }
            return response;
        }
        [Authorize]
        [HttpPut("MoneyTransfer")]
        public async Task<IActionResult> test2([FromBody] TransferData transferData)
        {
            try
            {
                var identity = _usersService.GetMyLogin();
                if (identity != transferData.Login)
                {
                    return BadRequest("Please stop trying to steal other people money");
                }

                string User = transferData.Login;
                var response = await _usersService.OperateTransfer(transferData);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        //[Authorize]
        [HttpGet("GetUserTransferHistory/{Login}")]
        public async Task<IActionResult> GetHistory(string Login)
        {
            try
            {
               /* var identity = _usersService.GetMyLogin();
                if (identity != Login)
                {
                    return BadRequest("Please stop trying to steal other people data");
                }*/



                var response = await _usersService.GetTransferHistory(Login);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }




    }
}
