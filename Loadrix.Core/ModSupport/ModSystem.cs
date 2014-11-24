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
        private readonly IModProvider _provider;

        public ModSystem(IModProvider provider, IEnumerable<String> selectedMods)
        {
            _provider = provider;

            var modSet = new HashSet<String>(selectedMods);
            var allModMetadatas = provider.AvailableMods.ToDictionary(meta => meta.UniqueName);

            var missingMods = modSet.Where(allModMetadatas.ContainsKey).ToList();
            if (missingMods.Count > 0)
            {
                throw new LoadrixModException("Mods missing from system: {0}", String.Join(", ", missingMods));
            }

            var orderedMods = GenerateModOrder(modSet.Select(m => allModMetadatas[m]), allModMetadatas);
        }



        private static List<ModMetadata> GenerateModOrder(IEnumerable<ModMetadata> selectedMods,
            Dictionary<String, ModMetadata> allMods)
        {
            var mods = selectedMods.ToList();
            var working = new List<ModMetadata>(mods.Count);
            var completed = new HashSet<ModMetadata>();

            foreach (ModMetadata m in mods) GenerateModOrderImpl(m, working, completed, allMods, 0);

            return working;
        }

        private static void GenerateModOrderImpl(ModMetadata metadata, 
            List<ModMetadata> working, HashSet<ModMetadata> completed, 
            Dictionary<String, ModMetadata> allMods, Int32 stackDepth = 0)
        {
            if (stackDepth > 20) throw new LoadrixModException("Endless loop in mod resolution.");

            foreach (var constraint in metadata.Constraints)
            {
                ModMetadata dependent;
                if (!allMods.TryGetValue(constraint.UniqueName, out dependent))
                {
                    throw new LoadrixModException("Mod '{0}' requires '{1}', which could not be found.",
                                                  metadata, constraint);
                }
                if (!constraint.Matches(dependent))
                {
                    throw new LoadrixModException("Mod '{0}' requires '{1}', but got '{2}'.",
                                                  metadata, constraint, dependent);
                }

                if (!completed.Contains(dependent))
                {
                    GenerateModOrderImpl(dependent, working, completed, allMods, stackDepth + 1);
                }

                working.Add(metadata);
            }
        }
    }
}
