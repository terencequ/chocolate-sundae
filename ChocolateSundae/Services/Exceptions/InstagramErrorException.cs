using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChocolateSundae.Services.Exceptions
{
    class InstagramErrorException : Exception
    {
        public InstagramErrorException(string message): base(message) {  }
    }
}
