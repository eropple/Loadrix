using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Loadrix.Core.Content
{
    /// <summary>
    /// A scope in which content can be loaded and its lifecycle controlled.
    /// </summary>
    /// <remarks>
    /// Content scopes are hierarchical content managers that will attempt to load content and make it
    /// available for use. All objects owned by a content scope are disposed when the scope is disposed.
    /// </remarks>
    public abstract class ContentScope : EasyDisposable
    {
        public readonly Int32 ID;

        private ContentScope _parent;
        private readonly HashSet<ContentScope> _childScopes = new HashSet<ContentScope>();

        private readonly Dictionary<Type, Dictionary<String, Object>> _loadCache = new Dictionary<Type, Dictionary<String, Object>>(); 

        protected ContentScope(ContentScope parent)
        {
            ID = Interlocked.Increment(ref _scopeIdCounter);

            _parent = parent;
            if (_parent != null) _parent._childScopes.Add(this);
        }


        public Boolean HasLoaded<TContentType>(String assetPath)
        {
            if (_parent != null && _parent.HasLoaded<TContentType>(assetPath)) return true;

            Dictionary<String, Object> objDict;
            return _loadCache.TryGetValue(typeof (TContentType), out objDict) && objDict.ContainsKey(assetPath);
        }

        protected abstract ContentScope CreateChildScope();

        public TContentType Load<TContentType>(String assetPath)
            where TContentType : class
        {
            var contentType = typeof (TContentType);
            var retval = LoadIfCached<TContentType>(assetPath);
            if (retval != null) return retval;

            try
            {
                retval = DoLoad<TContentType>(assetPath);
                if (retval == null) throw new NullReferenceException("retval from DoLoad<> was null.");

                Dictionary<String, Object> objDict;
                if (!_loadCache.TryGetValue(contentType, out objDict))
                {
                    objDict = new Dictionary<String, Object>();
                    _loadCache.Add(contentType, objDict);
                }

                objDict.Add(assetPath, retval);

                return retval;
            }
            catch (Exception ex)
            {
                throw new LoadrixContentException(ex, "Attempting to load '{0}' of type '{1}' threw an exception.", assetPath, contentType.FullName);
            }
        }

        public Boolean TryLoad<TContentType>(String assetPath, out TContentType value)
            where TContentType : class
        {
            try
            {
                value = Load<TContentType>(assetPath);
                return true;
            }
            catch (Exception)
            {
                value = null;
                return false;
            }
        }

        public TContentType LoadIfCached<TContentType>(String assetPath, Boolean recursive = true)
            where TContentType : class
        {
            var contentType = typeof (TContentType);

            if (_parent != null && recursive)
            {
                var parentValue = LoadIfCached<TContentType>(assetPath);
                if (parentValue != null) return parentValue;
            }

            Dictionary<String, Object> objDict;
            if (_loadCache.TryGetValue(contentType, out objDict))
            {
                Object o;
                if (objDict.TryGetValue(assetPath, out o))
                {
                    return (TContentType) o;
                }
            }

            return null;
        }



        protected abstract TContentType DoLoad<TContentType>(String assetPath)
            where TContentType : class;

        protected override void DoDispose()
        {
            if (_childScopes.Any(c => !c.IsDisposed))
            {
                throw new LoadrixContentException("Tried to dispose of scope #{0}, but the following child scopes are still alive: [{1}]",
                                                  ID,
                                                  String.Join(", ", _childScopes.Where(c => !c.IsDisposed).Select(c => c.ID)));
            }

            foreach (var objDict in _loadCache.Values)
            {
                foreach (var obj in objDict.Values)
                {
                    var disposable = obj as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }
            }

            if (_parent != null)
            {
                _parent._childScopes.Remove(this);
                _parent = null;
            }
        }

        public override int GetHashCode()
        {
            return ID;
        }

        private static Int32 _scopeIdCounter = 0;
    }
}
