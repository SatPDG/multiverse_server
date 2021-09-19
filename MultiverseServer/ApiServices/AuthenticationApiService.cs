using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MultiverseServer.ApiModel.Error;
using MultiverseServer.ApiModel.Model;
using MultiverseServer.ApiModel.Request;
using MultiverseServer.ApiModel.Response;
using MultiverseServer.Database.MultiverseDbModel;
using MultiverseServer.DatabaseContext;
using MultiverseServer.DatabaseModel;
using MultiverseServer.DatabaseService;
using MultiverseServer.RequestModel;
using MultiverseServer.Security.Hash;
using MultiverseServer.Security.Json;
using MultiverseServer.Security.Token;
using MultiverseServer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MultiverseServer.ApiServices
{
    public class AuthenticationApiService
    {
        private AuthenticationApiService()
        {

        }

        public static ApiResponse Register(IConfiguration config, MultiverseDbContext dbContext, RegisterRequestModel model)
        {
            ApiResponse response = new ApiResponse();

            // Validate the json
            if (!JsonValidator.ValidateJsonNotNullOrEmpty(model))
            {
                response.code = (int)HttpStatusCode.BadRequest;
                response.obj = new ErrorApiModel((int)ErrorType.JsonNotValid, ErrorMessage.JSON_NOT_VALID_MESSAGE);
                return response;
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
            int userID = UserDbService.CreateNewUser(dbContext, userDb);
            if (userID != -1)
            {
                // Generate the tokens
                var jwt = new JwtTokenService(config);
                string token = jwt.GenerateToken(userID, model.email, "0");
                string refreshToken = jwt.GenerateRefreshToken();

                // Push the refresh token in the database
                double expiresInMinute = double.Parse(config.GetSection("JwtConfig").GetSection("refreshExpirationInMinutes").Value);
                AuthenticationDbModel authenticationDb = new AuthenticationDbModel
                {
                    userID = userID,
                    token = refreshToken,
                    expireTime = DateTime.Now.AddMinutes(expiresInMinute),
                };
                AuthenticationDbService.UpdateAuthentication(dbContext, userID, authenticationDb);

                RegisterResponseModel responseModel = new RegisterResponseModel
                {
                    userID = userID,
                    token = token,
                    refreshToken = refreshToken,
                };

                response.obj = responseModel;
                return response;
            }
            else
            {
                response.code = (int)HttpStatusCode.Unauthorized;
                response.obj = new ErrorApiModel((int)ErrorType.UsernameAlreadyTaken, ErrorMessage.USERNAME_ALREADY_TAKEN_MESSAGE);
                return response;
            }
        }

        public static ApiResponse Login(IConfiguration config, MultiverseDbContext dbContext, LoginRequestModel request)
        {
            ApiResponse response = new ApiResponse();

            // Validate the json
            if (!JsonValidator.ValidateJsonNotNullOrEmpty(request))
            {
                response.code = (int)HttpStatusCode.BadRequest;
                response.obj = new ErrorApiModel((int)ErrorType.JsonNotValid, ErrorMessage.JSON_NOT_VALID_MESSAGE);
                return response;
            }

            // Search for the user
            UserDbModel userDb = UserDbService.GetUser(dbContext, request.email, request.password);
            if (userDb != null)
            {
                // Valid the credentials
                SHA256Hash hash = new SHA256Hash();
                string hashPassword = hash.ComputeSha256Hash(request.password);
                if (hashPassword.Equals(userDb.password))
                {
                    // Generate the tokens
                    var jwt = new JwtTokenService(config);
                    string token = jwt.GenerateToken(userDb.userID, request.email, "0");
                    string refreshToken = jwt.GenerateRefreshToken();

                    // Push the refresh token in the database
                    double expiresInMinute = double.Parse(config.GetSection("JwtConfig").GetSection("refreshExpirationInMinutes").Value);
                    AuthenticationDbModel authenticationDb = new AuthenticationDbModel
                    {
                        userID = userDb.userID,
                        token = refreshToken,
                        expireTime = DateTime.Now.AddMinutes(expiresInMinute),
                    };
                    AuthenticationDbService.UpdateAuthentication(dbContext, userDb.userID, authenticationDb);

                    // Build the answer
                    LoginResponseModel responseModel = new LoginResponseModel
                    {
                        token = token,
                        refreshToken = refreshToken
                    };
                    response.obj = responseModel;
                    return response;
                }
                else
                {
                    response.code = (int)HttpStatusCode.Unauthorized;
                    response.obj = new ErrorApiModel((int)ErrorType.CredentialNotValid, ErrorMessage.BAD_CREDENTIAL_MESSAGE);
                    return response;
                }
            }
            else
            {
                response.code = (int)HttpStatusCode.Unauthorized;
                response.obj = new ErrorApiModel((int)ErrorType.CredentialNotValid, ErrorMessage.BAD_CREDENTIAL_MESSAGE);
                return response;
            }
        }

        public static ApiResponse Refresh(IConfiguration config, MultiverseDbContext dbContext, RefreshRequestModel request)
        {
            ApiResponse response = new ApiResponse();

            // Validate the json
            if (!JsonValidator.ValidateJsonNotNullOrEmpty(request))
            {
                response.code = (int)HttpStatusCode.BadRequest;
                response.obj = new ErrorApiModel((int)ErrorType.JsonNotValid, ErrorMessage.JSON_NOT_VALID_MESSAGE);
                return response;
            }

            // Validate the token
            JwtTokenService jwt = new JwtTokenService(config);
            if (!jwt.ValidateJwtTokenWithoutLifetime(request.token))
            {
                response.code = (int)HttpStatusCode.Forbidden;
                response.obj = new ErrorApiModel((int)ErrorType.CredentialNotValid, ErrorMessage.BAD_CREDENTIAL_MESSAGE);
                return response;
            }

            // Get the user ID
            string token = request.token;
            int userID = int.Parse(new JwtTokenService(config).GetJwtClaim(token, "userID"));

            // Get the refresh token store in the database
            AuthenticationDbModel authentication = AuthenticationDbService.GetAuthentication(dbContext, userID);
            if (authentication == null)
            {
                response.code = (int)HttpStatusCode.Forbidden;
                response.obj = new ErrorApiModel((int)ErrorType.CredentialNotValid, ErrorMessage.BAD_CREDENTIAL_MESSAGE);
                return response;
            }

            // Check the validity of the refresh token
            if (!authentication.token.Equals(request.refreshToken))
            {
                response.code = (int)HttpStatusCode.Forbidden;
                response.obj = new ErrorApiModel((int)ErrorType.CredentialNotValid, ErrorMessage.BAD_CREDENTIAL_MESSAGE);
                return response;
            }

            // Make sure the refrseh token is still valid
            if (authentication.expireTime <= DateTime.Now)
            {
                // Remove the token from the database
                AuthenticationDbService.RemoveAuthentication(dbContext, userID);

                response.code = (int)HttpStatusCode.Forbidden;
                response.obj = new ErrorApiModel((int)ErrorType.RefreshTokenExpire, ErrorMessage.REFRESH_TOKEN_EXPIRED_MESSAGE);
                return response;
            }

            // Generate the new token
            UserDbModel userDb = UserDbService.GetUser(dbContext, userID);
            string newToken = jwt.GenerateToken(userDb.userID, userDb.email, "0");

            // Generate a new refresh token
            string newRefreshToken = jwt.GenerateRefreshToken();

            // Save the new refresh token in database
            double expiresInMinute = double.Parse(config.GetSection("JwtConfig").GetSection("refreshExpirationInMinutes").Value);
            authentication.token = newRefreshToken;
            authentication.expireTime = DateTime.Now.AddMinutes(expiresInMinute);
            AuthenticationDbService.UpdateAuthentication(dbContext, userID, authentication);

            RefreshResponseModel responseModel = new RefreshResponseModel()
            {
                token = newToken,
                refreshToken = newRefreshToken,
            };
            response.obj = responseModel;
            return response;
        }
    }
}
