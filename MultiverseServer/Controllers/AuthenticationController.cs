using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MultiverseServer.RequestModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using MultiverseServer.Security.Token;
using MultiverseServer.Security.Json;
using System.Security.Cryptography;
using System.Text;
using MultiverseServer.DatabaseContext;
using MultiverseServer.DatabaseModel;
using MultiverseServer.DatabaseService;
using System.Data.Entity.Spatial;
using MultiverseServer.ApiModel.Model;
using System.Net;
using MultiverseServer.ApiModel.Error;
using MultiverseServer.Services;
using MultiverseServer.Database.MultiverseDbModel;
using MultiverseServer.Security.Hash;
using MultiverseServer.ApiModel.Response;
using MultiverseServer.ApiModel.Request;
using MultiverseServer.Util.HttpRequestUtil;

namespace MultiverseServer.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class AuthenticationController : ControllerBase
    {

        private IConfiguration Config;
        private MultiverseDbContext DbContext;

        public AuthenticationController(IConfiguration config, MultiverseDbContext DbContext)
        {
            this.Config = config;
            this.DbContext = DbContext;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody]LoginRequestModel model)
        {
            // Validate the json
            if (!JsonValidator.ValidateJsonNotNullOrEmpty(model))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return new JsonResult(new ErrorApiModel((int)ErrorType.JsonNotValid, ErrorMessage.JSON_NOT_VALID_MESSAGE));
            }

            // Search for the user
            UserDbModel userDb = UserService.GetUser(DbContext, model.email, model.password);
            if(userDb != null)
            {
                // Valid the credentials
                SHA256Hash hash = new SHA256Hash();
                string hashPassword = hash.ComputeSha256Hash(model.password);
                if (hashPassword.Equals(userDb.password))
                {
                    // Generate the tokens
                    var jwt = new JwtTokenService(Config);
                    string token = jwt.GenerateToken(userDb.userID, model.email, "0");
                    string refreshToken = jwt.GenerateRefreshToken();

                    // Push the refresh token in the database
                    double expiresInMinute = double.Parse(Config.GetSection("JwtConfig").GetSection("refreshExpirationInMinutes").Value);
                    AuthenticationDbModel authenticationDb = new AuthenticationDbModel
                    {
                        userID = userDb.userID,
                        token = refreshToken,
                        expireTime = DateTime.Now.AddMinutes(expiresInMinute),
                    };
                    AuthenticationService.UpdateAuthentication(DbContext, userDb.userID, authenticationDb);

                    // Build the answer
                    LoginResponseModel responseModel = new LoginResponseModel
                    {
                        token = token,
                        refreshToken = refreshToken
                    };
                    return new JsonResult(responseModel);
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return new JsonResult(new ErrorApiModel((int)ErrorType.CredentialNotValid, ErrorMessage.BAD_CREDENTIAL_MESSAGE));
                }
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return new JsonResult(new ErrorApiModel((int)ErrorType.CredentialNotValid, ErrorMessage.BAD_CREDENTIAL_MESSAGE));
            }
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequestModel model)
        {
            // Validate the json
            if (!JsonValidator.ValidateJsonNotNullOrEmpty(model))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return new JsonResult(new ErrorApiModel((int)ErrorType.JsonNotValid, ErrorMessage.JSON_NOT_VALID_MESSAGE));
            }

            // Create the user
            UserDbModel userDb = new UserDbModel
            {
                email = model.email,
                password = model.password,
                firstname = model.firstname,
                lastname = model.lastname,
                lastLocation = new NetTopologySuite.Geometries.Point(model.lastLocation.longitude, model.lastLocation.latitude),
            };
            int userID = UserService.CreateNewUser(DbContext, userDb);
            if(userID != -1)
            {
                // Generate the tokens
                var jwt = new JwtTokenService(Config);
                string token = jwt.GenerateToken(userDb.userID, model.email, "0");
                string refreshToken = jwt.GenerateRefreshToken();

                // Push the refresh token in the database
                double expiresInMinute = double.Parse(Config.GetSection("JwtConfig").GetSection("refreshExpirationInMinutes").Value);
                AuthenticationDbModel authenticationDb = new AuthenticationDbModel
                {
                    userID = userDb.userID,
                    token = refreshToken,
                    expireTime = DateTime.Now.AddMinutes(expiresInMinute),
                };
                AuthenticationService.UpdateAuthentication(DbContext, userDb.userID, authenticationDb);

                RegisterResponseModel responseModel = new RegisterResponseModel
                {
                    userID = userID,
                    token = token,
                    refreshToken = refreshToken,
                };
                return new JsonResult(responseModel);
            }
            else
            {
                return new JsonResult(new ErrorApiModel((int)ErrorType.UsernameAlreadyTaken, ErrorMessage.USERNAME_ALREADY_TAKEN_MESSAGE));
            }
        }

        [HttpPost("refresh")]
        public IActionResult Refresh([FromBody] RefreshRequestModel model)
        {
            // Get the user ID
            string token = HttpRequestUtil.GetTokenFromRequest(Request);
            int userID = int.Parse(new JwtTokenService(Config).GetJwtClaim(token, "userID"));

            // Get the userID the refresh token store in the database
            AuthenticationDbModel authentication = AuthenticationService.GetAuthentication(DbContext, userID);
            if(authentication == null)
            {
                return new JsonResult(new ErrorApiModel((int)ErrorType.CredentialNotValid, ErrorMessage.BAD_CREDENTIAL_MESSAGE));
            }

            // Check the validity of the refresh token
            if (!authentication.token.Equals(model.refreshToken))
            {
                return new JsonResult(new ErrorApiModel((int)ErrorType.CredentialNotValid, ErrorMessage.BAD_CREDENTIAL_MESSAGE));
            }
            if(authentication.expireTime <= DateTime.Now)
            {
                // Remove the token from the database
                AuthenticationService.RemoveAuthentication(DbContext, userID);

                return new JsonResult(new ErrorApiModel((int)ErrorType.RefreshTokenExpire, ErrorMessage.REFRESH_TOKEN_EXPIRED_MESSAGE));
            }

            // Generate the new token
            UserDbModel userDb = UserService.GetUser(DbContext, userID);

            JwtTokenService jwt = new JwtTokenService(Config);
            string newToken = jwt.GenerateToken(userDb.userID, userDb.email, "0");

            RefreshResponseModel responseModel = new RefreshResponseModel()
            {
                token = newToken,
                refreshToken = model.refreshToken,
            };

            return new JsonResult(responseModel);
        }

        [HttpGet("test")]
        public void test()
        {
            
        }
    }
}
