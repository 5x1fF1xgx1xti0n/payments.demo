using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Payments.API.Hash
{
    public interface IHasher
    {
        string Encrypt(string data);
        string Decrypt(string data);
    }
}
