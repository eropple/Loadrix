using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Loadrix.Core.ModSupport
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ModMetadata
    {
        [JsonProperty("UniqueName")]
        public String UniqueName { get; private set; }

        [JsonProperty("Version")]
        public Version Version { get; private set; }

        [JsonProperty("Constraints")]
        public IReadOnlyList<Constraint> Constraints { get; private set; }

        private ModMetadata() { }

        public override string ToString()
        {
            return String.Format("{0} {1}", UniqueName, Version);
        }
    }
}
