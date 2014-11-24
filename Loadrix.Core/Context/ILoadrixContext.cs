using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loadrix.Core.Content;
using Loadrix.Core.Extensions;

namespace Loadrix.Core.Context
{
    public interface ILoadrixContext
    {
        /// <summary>
        /// Gets the extension system for this context.
        /// </summary>
        IExtensionLoader Extensions { get; }

        /// <summary>
        /// Gets the content system for this context.
        /// </summary>
        IContent Content { get; }
    }
}
