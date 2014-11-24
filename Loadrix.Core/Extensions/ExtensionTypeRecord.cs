using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loadrix.Core.Extensions
{
    public class ExtensionTypeRecord
    {
        public readonly Type ExtensionType;
        public readonly Type AttributeType;
        public readonly IEnumerable<Type> ConstructorParameters;

        public ExtensionTypeRecord(Type extensionType, Type attributeType, 
                                   IEnumerable<Type> constructorParameters)
        {
            ExtensionType = extensionType;
            AttributeType = attributeType;
            ConstructorParameters = constructorParameters.ToList(); // avoiding later mutation
        }
    }
}
