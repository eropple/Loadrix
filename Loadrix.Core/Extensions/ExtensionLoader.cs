using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Loadrix.Core.Extensions
{
    /// <summary>
    /// Reflection-based loader that scans a set of supplied assemblies for extension classes tagged
    /// with a specified attribute and key nane.
    /// </summary>
    public class ExtensionLoader : ILoader
    {
        public static readonly Object[] EmptyParams = { };
        public static readonly Type[] NoParameters = { };

        private readonly Dictionary<TypeInfo, ExtensionTypeRecord> _typeRecords;
        public IEnumerable<Type> SupportedTypes { get { return _typeRecords.Values.Select(etr => etr.ExtensionType); } }

        private readonly Dictionary<TypeInfo, MultiValueDictionary<String, ConstructorInfo>> _lookupTable;
        private readonly bool _throwIfConstructorMissing;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="assemblies">
        /// The set of assemblies, in descending order of importance, to scan for this loader.
        /// </param>
        /// <param name="typeRecords">
        /// The set of type records to use in this loader. Each must have a unique ExtensionType.
        /// </param>
        /// <param name="throwIfConstructorMissing">
        /// Determines whether an exception should be thrown (and, by implication, extension loading
        /// halted) if a constructor with the given signature is not found.
        /// </param>
        public ExtensionLoader(IEnumerable<Assembly> assemblies, IEnumerable<ExtensionTypeRecord> typeRecords, Boolean throwIfConstructorMissing = true)
        {
            _throwIfConstructorMissing = throwIfConstructorMissing;
            var typeRecordsList = typeRecords.ToList();
            var dupeTypeRecord =
                typeRecordsList.FirstOrDefault(
                    etr1 => typeRecordsList.Any(etr2 => etr1 != etr2 && etr1.ExtensionType == etr2.ExtensionType));
            if (dupeTypeRecord != null)
            {
                throw new LoadrixExtensionException("Duplicate ExtensionTypeRecords with ExtensionType of '{0}'.", dupeTypeRecord.ExtensionType.FullName);
            }


            _typeRecords = typeRecordsList.ToDictionary(tr => tr.ExtensionType.GetTypeInfo());
            _lookupTable = BuildLookupTable(assemblies);
        }



        private Dictionary<TypeInfo, MultiValueDictionary<String, ConstructorInfo>> BuildLookupTable(
                IEnumerable<Assembly> assemblies)
        {
            var ctorDict = new Dictionary<TypeInfo, MultiValueDictionary<String, ConstructorInfo>>(_typeRecords.Count);

            foreach (var t in _typeRecords.Values) ctorDict.Add(t.ExtensionType.GetTypeInfo(), 
                                                                   new MultiValueDictionary<string, ConstructorInfo>());

            // ensures that every type satisfies at least one entry in _typeRecords and can be purely
            // checked against ExtensionTypeRecord.AttributeType.
            var assemblyTypeLists = assemblies.Select(
                a => a.DefinedTypes.Where(ti => ctorDict.Keys.Any(tk => tk.IsAssignableFrom(ti)))
            );

            foreach (var typeList in assemblyTypeLists)
            {
                foreach (var type in typeList)
                {
                    var typeRecord = _typeRecords[type];
                    var typeCtorDict = ctorDict[type];

                    var attr = type.GetCustomAttribute(typeRecord.AttributeType) as ExtensionAttribute;
                    if (attr == null) continue;

                    var ctor = type.DeclaredConstructors.FirstOrDefault(
                        c =>
                            typeRecord.ConstructorParametersArray.SequenceEqual(
                                c.GetParameters().Select(p => p.ParameterType))
                        );

                    if (ctor == null)
                    {
                        if (_throwIfConstructorMissing)
                        {
                            throw new LoadrixExtensionException("Could not find appropriate constructor ({0}) for extension type '{1}'.",
                                                                String.Join(", ", typeRecord.ConstructorParametersArray.Select(t => t.FullName)),
                                                                type.FullName);
                        }
                        continue;
                    }
                    typeCtorDict.Add(attr.Key, ctor);
                }
            }

            return ctorDict;
        }

        public ConstructorInfo FindConstructor<TExtensionType>(String key)
            where TExtensionType : class
        {
            var retval = DeepFindConstructor<TExtensionType>(key).FirstOrDefault();
            if (retval == null)
            {
                throw new LoadrixExtensionException("No extension for type '{0}' and key '{1}' found.", 
                                                    typeof(TExtensionType).FullName, key);
            }
            return retval;
        }

        public IEnumerable<ConstructorInfo> DeepFindConstructor<TExtensionType>(String key)
            where TExtensionType : class
        {
            var info = typeof(TExtensionType).GetTypeInfo();

            MultiValueDictionary<String, ConstructorInfo> ctorDict;
            if (_lookupTable.TryGetValue(info, out ctorDict))
            {
                IReadOnlyCollection<ConstructorInfo> ctor;
                if (ctorDict.TryGetValue(key, out ctor))
                {
                    return ctor;
                }
                return Enumerable.Empty<ConstructorInfo>();
            }

            throw new LoadrixExtensionException("Extension type '{0}' unrecognized.", info.FullName);
        }

        /// <summary>
        /// Loads the highest priority extension for the given key.
        /// </summary>
        /// <typeparam name="TExtensionType">The extension type to load.</typeparam>
        /// <param name="key">The named key of the descendant of ExtensionAttribute you wish to load.</param>
        /// <param name="args">Arguments to be passed to the extended type's constructor.</param>
        /// <returns>A instance of an extension type's subclass.</returns>
        public TExtensionType Load<TExtensionType>(String key, params Object[] args)
            where TExtensionType : class
        {
            var ctor = FindConstructor<TExtensionType>(key);

            var argTypes = args.Select(o => o.GetType()).ToList();
            var extTypeInfo = typeof (TExtensionType).GetTypeInfo();
            var etr = _typeRecords[extTypeInfo];
            if (!etr.ConstructorParametersArray.SequenceEqual(argTypes))
            {
                throw new LoadrixExtensionException(
                    "Invalid arg types when attempting to load object of type '{0}': expected [{1}], got [{2}].",
                    extTypeInfo.FullName,
                    String.Join(", ", etr.ConstructorParametersArray.Select(t => t.FullName)),
                    String.Join(", ", argTypes.Select(t => t.FullName)));
            }

            return (TExtensionType) ctor.Invoke(args);
        }

        /// <summary>
        /// Lazily loads all extensions for the given key.
        /// </summary>
        /// <typeparam name="TExtensionType">The extension type to load.</typeparam>
        /// <param name="key">The named key of the descendant of ExtensionAttribute you wish to load.</param>
        /// <param name="args">Arguments to be passed to the extended type's constructor.</param>
        /// <returns>A lazy enumerable of instances of the specified type/key's extensions.</returns>
        public IEnumerable<TExtensionType> DeepLoad<TExtensionType>(String key, params Object[] args)
            where TExtensionType : class
        {
            var ctors = DeepFindConstructor<TExtensionType>(key);

            var argTypes = args.Select(o => o.GetType()).ToList();
            var extTypeInfo = typeof(TExtensionType).GetTypeInfo();
            var etr = _typeRecords[extTypeInfo];
            if (!etr.ConstructorParametersArray.SequenceEqual(argTypes))
            {
                throw new LoadrixExtensionException("Invalid arg types when attempting to load object of type '{0}': expected [{1}], got [{2}].",
                                                    extTypeInfo.FullName,
                                                    String.Join(", ", etr.ConstructorParametersArray.Select(t => t.FullName)),
                                                    String.Join(", ", argTypes.Select(t => t.FullName)));
            }

            return ctors.Select(ctor => (TExtensionType) ctor.Invoke(args));
        }

        /// <summary>
        /// Eagerly loads all extensions for the given key.
        /// </summary>
        /// <typeparam name="TExtensionType">The extension type to load.</typeparam>
        /// <param name="key">The named key of the descendant of ExtensionAttribute you wish to load.</param>
        /// <param name="args">Arguments to be passed to the extended type's constructor.</param>
        /// <returns>A list of instances of the specified type/key's extensions.</returns>
        public List<TExtensionType> DeepEagerLoad<TExtensionType>(String key, params Object[] args)
            where TExtensionType : class
        {
            return DeepLoad<TExtensionType>(key, args).ToList();
        }
    }
}
