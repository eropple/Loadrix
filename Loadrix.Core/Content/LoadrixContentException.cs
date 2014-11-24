using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loadrix.Core.Content
{
    public class LoadrixContentException : LoadrixException
    {
        public LoadrixContentException()
        {
        }

        public LoadrixContentException(string message) : base(message)
        {
        }

        public LoadrixContentException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public LoadrixContentException(Exception innerException, string message, params object[] args) : base(innerException, message, args)
        {
        }

        public LoadrixContentException(string message, params object[] args) : base(message, args)
        {
        }
    }
}
