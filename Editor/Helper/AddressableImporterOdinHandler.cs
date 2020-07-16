namespace UnityAddressableImporter.Editor.Helper
{
    using System;
    using Object = UnityEngine.Object;

#if ODIN_INSPECTOR
    
    using Sirenix.OdinInspector.Editor;
    using Sirenix.OdinInspector;
    
    public class AddressableImporterOdinHandler : IDisposable
    {
        private Object _target;
        private PropertyTree _drawer;
        
        public void Initialize(Object target)
        {
            _target = target;
            _drawer = PropertyTree.Create(_target);
        } 
        
        public void Draw() => _drawer?.Draw(false);

        public void Dispose()
        {
            _target = null;
            _drawer?.Dispose();
            _drawer = null;
        }
    }
    
#else

    public class AddressableImporterOdinHandler : IDisposable
    {
        public void Initialize(Object target) { } 
        
        public void Draw() { }

        public void Dispose() { }
    }

#endif
}
