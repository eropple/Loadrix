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
    public abstract class ModProvider
    {
        public abstract IEnumerable<ModMetadata> AvailableMods { get; }

        protected static List<ModMetadata> GenerateCompileOrder(IEnumerable<ModMetadata> selectedMods,
            Dictionary<String, ModMetadata> modSet)
        {
            var mods = selectedMods.ToList();
            var working = new List<ModMetadata>(mods.Count);
            var completed = new HashSet<ModMetadata>();

            foreach (ModMetadata m in mods) GenerateCompileOrderImpl(m, working, completed, modSet, 0);

            return working;
        }

        protected static void GenerateCompileOrderImpl(ModMetadata metadata,
            List<ModMetadata> working, HashSet<ModMetadata> completed,
            Dictionary<String, ModMetadata> modSet, Int32 stackDepth = 0)
        {
            if (stackDepth > 20) throw new LoadrixModException("Endless loop in mod resolution.");

            foreach (var constraint in metadata.Constraints)
            {
                ModMetadata dependent;
                if (!modSet.TryGetValue(constraint.UniqueName, out dependent))
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
                    GenerateCompileOrderImpl(dependent, working, completed, modSet, stackDepth + 1);
                }

                working.Add(metadata);
                completed.Add(metadata);
            }
        }
    }
}
