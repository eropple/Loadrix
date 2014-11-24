using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Loadrix.Core.ModSupport
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Constraint : IEquatable<Constraint>
    {
        private Constraint() { }
        public Constraint(string uniqueName, Version minimumVersion, Version belowVersion)
        {
            BelowVersion = belowVersion;
            MinimumVersion = minimumVersion;
            UniqueName = uniqueName;
        }


        [JsonProperty("UniqueName")]
        public String UniqueName { get; private set; }
        [JsonProperty("MinimumVersion", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Version MinimumVersion { get; private set; }
        [JsonProperty("BelowVersion", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Version BelowVersion { get; private set; }

        public Boolean Matches(ModMetadata meta)
        {
            if (!meta.UniqueName.Equals(UniqueName)) return false;
            if (MinimumVersion != null && MinimumVersion.CompareTo(meta.Version) == -1) return false;

            return BelowVersion == null || BelowVersion.CompareTo(meta.Version) == -1;
        }

        public override string ToString()
        {
            return String.Format("{0}, {1} to {2}", UniqueName,
                MinimumVersion == null ? "ANY" : MinimumVersion.ToString(),
                BelowVersion == null ? "ANY" : BelowVersion.ToString());
        }

        public bool Equals(Constraint other)
        {
            return string.Equals(UniqueName, other.UniqueName) && Equals(MinimumVersion, other.MinimumVersion) && Equals(BelowVersion, other.BelowVersion);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Constraint) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (UniqueName != null ? UniqueName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (MinimumVersion != null ? MinimumVersion.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (BelowVersion != null ? BelowVersion.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
