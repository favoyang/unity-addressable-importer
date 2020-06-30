namespace UnityAddressableImporter.Editor.Helper
{
    using System;

#if ODIN_INSPECTOR
    using Sirenix.OdinInspector.Editor;
    using Sirenix.OdinInspector;
    using UnityEditor;
    using UnityEngine;

    public class AddressableImporterOdinHandler : IDisposable
    {
        private AddressableImportSettings _settings;
        private AddressablesImporterFilterOdinHandler _importRulesContainer;
        private PropertyTree _settingsTree;
        private GUIContent _searchFieldLabel;
        private string _searchField;
        
        public void Initialize(AddressableImportSettings target)
        {
            _settings = target;
            _importRulesContainer = ScriptableObject.CreateInstance<AddressablesImporterFilterOdinHandler>();
            _importRulesContainer.Initialize(_settings);
            _searchFieldLabel = new GUIContent("Filter:", "rules search filter");
            _settingsTree = PropertyTree.Create(_settings);
        }

        public void Draw()
        {
            _searchField = EditorGUILayout.TextField(_searchFieldLabel, _searchField);
            if (_settingsTree == null) {
                EditorGUILayout.LabelField("EMPTY DRAWER");
                return;
            }

            DrawInspectorTree(_searchField);
            
            EditorUtility.SetDirty(_settings);
            _settingsTree.ApplyChanges();
            
        }

        public void Dispose()
        {
            _settings = null;
            _settingsTree?.Dispose();
            _settingsTree = null;
        }

        private void DrawInspectorTree(string filter)
        {
            
            //_settingsTree.Draw();
            foreach (var property in _settingsTree.EnumerateTree(false)) {
                if (property.Name == nameof(_settings.rules)) {
                    _importRulesContainer.Draw(filter);
                    continue;
                }
                property.Draw();
            }

        }
    }
    
#else

    public class AddressablesImporterOdinHandler : IDisposable
    {
        public void Initialize(AddressableImportSettings target) { } 
        
        public void Draw() { }

        public void Dispose() { }
    }

#endif
}
