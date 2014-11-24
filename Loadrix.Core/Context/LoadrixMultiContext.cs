using System.Collections.Generic;
using System.Linq;

namespace Loadrix.Core.Context
{
    public class LoadrixMultiContext : ILoadrixContext
    {
        private readonly List<ILoadrixContext> _childContexts;
        public IEnumerable<ILoadrixContext> ChildContexts { get { return _childContexts; } }

        public LoadrixMultiContext(IEnumerable<ILoadrixContext> childContexts)
        {
            _childContexts = childContexts.ToList();
        }
    }
}
