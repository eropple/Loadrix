using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loadrix.Core.ModSupport
{
    public class LoadrixModException : LoadrixException
    {
        public LoadrixModException()
        {
        }

        public LoadrixModException(string message) : base(message)
        {
        }

        public LoadrixModException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public LoadrixModException(Exception innerException, string message, params object[] args) : base(innerException, message, args)
        {
        }

        public LoadrixModException(string message, params object[] args) : base(message, args)
        {
        }
    }
}
