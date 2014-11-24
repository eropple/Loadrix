using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Loadrix.Core.ModSupport
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Version : IComparable<Version>, IEquatable<Version>
    {
        private Version() { }
        public Version(int major, int minor, int patch)
        {
            Patch = patch;
            Minor = minor;
            Major = major;
        }

        [JsonProperty("Major", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Int32 Major { get; private set; }
        [JsonProperty("Minor", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Int32 Minor { get; private set; }
        [JsonProperty("Patch", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Int32 Patch { get; private set; }

        public override string ToString()
        {
            return String.Format("v{0}.{1}.{2}", Major, Minor, Patch);
        }

        public Int32 CompareTo(Version other)
        {
            Int32 c = 0;
            c = Major.CompareTo(other.Major);
            if (c != 0) return c;
            c = Minor.CompareTo(other.Minor);
            if (c != 0) return c;
            c = Patch.CompareTo(other.Patch);

            return c;
        }

        public bool Equals(Version other)
        {
            return Major == other.Major && Minor == other.Minor && Patch == other.Patch;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Version) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Major;
                hashCode = (hashCode*397) ^ Minor;
                hashCode = (hashCode*397) ^ Patch;
                return hashCode;
            }
        }
    }
}
