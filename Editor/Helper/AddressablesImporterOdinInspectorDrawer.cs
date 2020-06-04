namespace UnityAddressableImporter.Editor.Helper
{
    using System;
    using System.Diagnostics;
    using Object = UnityEngine.Object;

    public class AddressablesImporterOdinInspectorDrawer : IDisposable
    {
        private Object _target;
#if ODIN_INSPECTOR
        private Sirenix.OdinInspector.Editor.PropertyTree _drawer;
#endif

        [Conditional("ODIN_INSPECTOR")]
        public void Initialize(Object target)
        {
            _target = target;
            _drawer = Sirenix.OdinInspector.Editor.PropertyTree.Create(_target);
        } 
        
        [Conditional("ODIN_INSPECTOR")]
        public void Draw()
        {
            if (!_target) return;
            _drawer.Draw(false);
        }

        public void Dispose()
        {
            _target = null;
#if ODIN_INSPECTOR
            _drawer?.Dispose();
#endif
        }
    }
}
