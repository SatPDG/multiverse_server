using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiverseServer.ApiModel.Error
{
    public enum ErrorType : int
    {
        CredentialNotValid = 1,
        RefreshTokenExpire = 2,
        JsonNotValid = 10,
        UsernameAlreadyTaken = 50,
        BadIdentificationNumber = 100,
        IdentificationNumberDoNotGrantAccess = 101,
        IllegalAction = 102,
        BadListAccess = 110,
    }

    public class ErrorApiModel
    {
        public int errorType { get; set; }
        public string message { get; set; }

        public ErrorApiModel(int errorType)
        {
            this.errorType = errorType;
            this.message = "";
        }

        public ErrorApiModel(int errorType, string message)
        {
            this.errorType = errorType;
            this.message = message;
        }
    }
}
