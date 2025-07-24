using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelsWebApi.Auth
{
    public interface ITokenService
    {
        string  BuildToken(string key, string issuer, UserDto user);
    }
}