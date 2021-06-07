using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MultiverseServer.DatabaseContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiverseServer.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class UserController
    {
        private IConfiguration Config;
        private MultiverseDbContext DbContext;

        public UserController(IConfiguration config, MultiverseDbContext DbContext)
        {
            this.Config = config;
            this.DbContext = DbContext;
        }

        [Authorize]
        [HttpGet("{id}")]
        public IActionResult UserProfil()
        {
            return null;
        }

        [Authorize]
        [HttpGet("users")]
        public IActionResult Users()
        {
            return null;
        }

        [Authorize]
        [HttpGet("follower")]
        public IActionResult Follower()
        {
            return null;
        }

        [Authorize]
        [HttpGet("followed")]
        public IActionResult Followed()
        {
            return null;
        }

        [Authorize]
        [HttpDelete("follower/{id}")]
        public IActionResult DeteleFollower()
        {
            return null;
        }

        [Authorize]
        [HttpDelete("followed/{id}")]
        public IActionResult DeleteFollowed()
        {
            return null;
        }

        [Authorize]
        [HttpGet("follower/request")]
        public IActionResult FollowerRequest()
        {
            return null;
        }

        [Authorize]
        [HttpGet("followed/request")]
        public IActionResult FollowedRequest()
        {
            return null;
        }

        [Authorize]
        [HttpPost("follower/request/{id}")]
        public IActionResult SendFollowerRequest()
        {
            return null;
        }

        [Authorize]
        [HttpPost("follower/request/{id}")]
        public IActionResult AcceptFollowerRequest()
        {
            return null;
        }

        [Authorize]
        [HttpDelete("followed/request/{id}")]
        public IActionResult DeleteFollowedRequest()
        {
            return null;
        }
    }
}
