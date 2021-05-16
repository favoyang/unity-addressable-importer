/// <summary>
/// ButtonMethodAttribute,
/// modified from https://github.com/Deadcows/MyBox/blob/master/Attributes/ButtonMethodAttribute.cs
/// </summary>
namespace UnityAddressableImporter.Helper.Internal
{
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEditor;

#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
#endif

    [CustomEditor(typeof(AddressableImportSettings), true), CanEditMultipleObjects]
    public class AddressableImportSettingsEditor : Editor
    {
        private List<MethodInfo> _methods;
        private AddressableImportSettings _target;

#if ODIN_INSPECTOR
        private PropertyTree _propertyTree;
#endif
        
        private void OnEnable()
        {
            _target = target as AddressableImportSettings;
            if (_target == null) return;

#if ODIN_INSPECTOR
            _propertyTree = PropertyTree.Create(_target);
#endif
            
            _methods = AddressableImporterMethodHandler.CollectValidMembers(_target.GetType());
        }

        public override void OnInspectorGUI()
        {
            
            if (DrawWithOdin() == false)
            {
                DrawBaseEditor();
            }

            DrawCommands();
            
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawBaseEditor()
        {
            base.OnInspectorGUI();
        }

        private void DrawCommands()
        {
            if (_methods == null) return;
            AddressableImporterMethodHandler.OnInspectorGUI(_target, _methods);
        }
        
        private void OnDisable()
        {
#if ODIN_INSPECTOR
            _propertyTree?.Dispose();
#endif
        }

        private bool DrawWithOdin()
        {
            var hasOdinAsset = false;
#if ODIN_INSPECTOR_3
            hasOdinAsset = true;
#endif
            if (!hasOdinAsset) return false;

#if ODIN_INSPECTOR
            _propertyTree?.Draw();
            _propertyTree?.ApplyChanges();
#endif
            
            return true;
        }
    }
}