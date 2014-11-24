using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loadrix.Core
{
    public class LoadrixException : Exception
    {
        public LoadrixException()
        {
        }

        public LoadrixException(string message)
            : base(message)
        {
        }

        public LoadrixException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public LoadrixException(Exception innerException, String message, params Object[] args)
            : base(String.Format(message, args), innerException)
        { }

        public LoadrixException(String message, params Object[] args)
            : base(String.Format(message, args), null)
        { }
    }
}
