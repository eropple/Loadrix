using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loadrix.Core.Extensions;

namespace Loadrix.Core.Context
{
    public interface ILoadrixContext
    {
        ExtensionLoader Extensions { get; }
    }
}
