using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loadrix.Core.Content;
using Loadrix.Core.Extensions;

namespace Loadrix.Core.ModSupport
{
    public class Mod
    {
        public Mod(ModMetadata metadata, IContent content, IExtensionLoader extensions)
        {
            Extensions = extensions;
            Content = content;
            Metadata = metadata;
        }

        public ModMetadata Metadata { get; private set; }

        public IContent Content { get; private set; }

        public IExtensionLoader Extensions { get; private set; }
    }
}
