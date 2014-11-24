using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loadrix.Core.ModSupport
{
    /// <summary>
    /// An implementation of Loadrix that supports both extensions and content
    /// loading.
    /// </summary>
    /// <remarks>
    /// ModSystem abstracts content and extension into the concept of a "mod".
    /// A mod is a package of code and assets that includes metadata for
    /// dependencies. It's designed to be fairly pluggable and to allow for
    /// customization to a particular project's needs through constructor-based
    /// dependency injection.
    /// </remarks>
    public class ModSystem
    {
        public IReadOnlyCollection<Mod> ActiveMods { get; private set; }
        private readonly ModProvider _provider;

        public ModSystem(ModProvider provider, IEnumerable<String> selectedMods)
        {
            _provider = provider;

            var modSet = new HashSet<String>(selectedMods);
            var allModMetadatas = provider.AvailableMods.ToDictionary(meta => meta.UniqueName);

            var missingMods = modSet.Where(allModMetadatas.ContainsKey).ToList();
            if (missingMods.Count > 0)
            {
                throw new LoadrixModException("Mods missing from system: {0}", String.Join(", ", missingMods));
            }

        }
    }
}
