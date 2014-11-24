using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Loadrix.Core.Extensions
{
    public class ExtensionLoader
    {
        public static readonly Object[] EmptyParams = { };
        public static readonly Type[] NoParameters = { };

        private readonly Dictionary<TypeInfo, ExtensionTypeRecord> _supportedTypes;
        public IEnumerable<ExtensionTypeRecord> SupportedTypes { get { return _supportedTypes.Values; } }

        private readonly Dictionary<Type, MultiValueDictionary<String, ConstructorInfo>> _lookupTable;

        public ExtensionLoader(IEnumerable<Assembly> assemblies, IEnumerable<ExtensionTypeRecord> types)
        {
            _supportedTypes = types.ToDictionary(tr => tr.ExtensionType.GetTypeInfo());
            _lookupTable = BuildLookupTable(assemblies, _supportedTypes);
        }



        private static Dictionary<Type, MultiValueDictionary<String, ConstructorInfo>> BuildLookupTable(
                IEnumerable<Assembly> assemblies, Dictionary<TypeInfo, ExtensionTypeRecord> types)
        {
            var dict = new Dictionary<Type, MultiValueDictionary<String, ConstructorInfo>>(types.Count);

            foreach (var t in types.Values) dict.Add(t.ExtensionType, 
                                                     new MultiValueDictionary<string, ConstructorInfo>());

            var assemblyTypes = assemblies.Select(
                a => a.DefinedTypes
            );

            // TODO: actually implement.

            return dict;
        }
    }
}
