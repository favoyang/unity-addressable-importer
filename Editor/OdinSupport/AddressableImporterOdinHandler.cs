namespace UnityAddressableImporter.Editor.Helper
{
    using System;
    using Object = UnityEngine.Object;
#if ODIN_INSPECTOR
    using Sirenix.OdinInspector.Editor;
    using Sirenix.OdinInspector;
    using UnityEditor;
    using UnityEngine;

    public class AddressableImporterOdinHandler : IDisposable
    {
        private AddressableImportSettings _settings;
        private AddressableImporterFilterOdinHandler _importRulesContainer;
        private GUIContent _searchFieldLabel;
        private string _searchField;

        public void Initialize(AddressableImportSettings target)
        {
            _settings = target;
            _importRulesContainer = CreateDrawer(_settings);
        }

        public void Draw()
        {
            DrawInspectorTree(_searchField);

            EditorUtility.SetDirty(_settings);
        }

        public void Dispose()
        {
            _settings = null;
            if (_importRulesContainer) {
                Object.DestroyImmediate(_importRulesContainer);
                _importRulesContainer = null;
            }
        }

        private AddressableImporterFilterOdinHandler CreateDrawer(AddressableImportSettings settings)
        {
            _importRulesContainer = ScriptableObject.CreateInstance<AddressableImporterFilterOdinHandler>();
            _importRulesContainer.Initialize(settings);
            return _importRulesContainer;
        }

        private void DrawInspectorTree(string filter)
        {
            _importRulesContainer?.Draw();
        }
    }

#else

    public class AddressableImporterOdinHandler : IDisposable
    {
        public void Initialize(AddressableImportSettings target) { }

        public void Draw() { }

        public void Dispose() { }
    }

#endif
}
