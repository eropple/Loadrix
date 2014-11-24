using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loadrix.Core.Extensions
{
    public class LoadrixExtensionException : LoadrixException
    {
        public LoadrixExtensionException()
        {
        }

        public LoadrixExtensionException(string message) : base(message)
        {
        }

        public LoadrixExtensionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public LoadrixExtensionException(Exception innerException, string message, params object[] args) : base(innerException, message, args)
        {
        }

        public LoadrixExtensionException(string message, params object[] args) : base(message, args)
        {
        }
    }
}
