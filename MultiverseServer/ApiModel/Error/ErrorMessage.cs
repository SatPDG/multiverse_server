﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiverseServer.ApiModel.Error
{
    static class ErrorMessage
    {
        public const string REFRESH_TOKEN_EXPIRED_MESSAGE = "The refresh token has expire.";
        public const string BAD_CREDENTIAL_MESSAGE = "The credential sent are not valid.";
        public const string JSON_NOT_VALID_MESSAGE = "The Json sent is not valid. Some fields or missing or some of them are empty.";
        public const string USERNAME_ALREADY_TAKEN_MESSAGE = "The username is already taken.";
    }
}
