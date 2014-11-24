using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loadrix.Core.ModSupport
{
    /// <summary>
    /// Handles the platform-specific aspects of the ModSystem. For example, it
    /// creates the appropriate ContentScope for MonoGame, compiles mods on
    /// platforms where that's supported, etc.
    /// </summary>
    public interface IModProvider
    {
        IEnumerable<ModMetadata> AvailableMods { get; }
    }
}
