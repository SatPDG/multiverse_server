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
using MultiverseServer.ApiServices;

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
            ApiResponse response = AuthenticationApiService.Login(Config, DbContext, model);

            Response.StatusCode = response.code;
            return new JsonResult(response.obj);
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequestModel model)
        {
            ApiResponse response = AuthenticationApiService.Register(Config, DbContext, model);

            Response.StatusCode = response.code;
            return new JsonResult(response.obj);
        }

        [HttpPost("refresh")]
        public IActionResult Refresh([FromBody] RefreshRequestModel request)
        {
            ApiResponse response = AuthenticationApiService.Refresh(Config, DbContext, request);

            Response.StatusCode = response.code;
            return new JsonResult(response.obj);
        }
    }
}
