namespace UnityAddressableImporter.Editor.Helper
{
#if ODIN_INSPECTOR

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using UnityEditor;
    using UnityEngine;

    public class AddressablesImporterFilterOdinHandler : ScriptableObject
    {
        private AddressableImportSettings                       _importSettings;
        private PropertyTree                                    _drawerTree;
        private List<Func<AddressableImportRule, string, bool>> _filters;
        //private List<AddressableImportRule>                     _filteredRules;
        private bool _sourceChanged = false;
        
        [SerializeField]
        [ListDrawerSettings(
            CustomAddFunction = nameof(CustomAddFunction),
            HideRemoveButton = true,
            OnEndListElementGUI = nameof(EndOfListItemGui),
            CustomRemoveElementFunction = nameof(CustomRemoveElementFunction),
            CustomRemoveIndexFunction = nameof(CustomRemoveIndexFunction)
        )]
        private List<AddressableImportRule> rulesView = new List<AddressableImportRule>();

        public void Initialize(AddressableImportSettings importSettings)
        {
            _importSettings = importSettings;
            _drawerTree     = PropertyTree.Create(this);
            //_filteredRules  = new List<AddressableImportRule>();
            _filters = new List<Func<AddressableImportRule, string, bool>>() {
                ValidateAddressableGroupName,
                ValidateRulePath,
                ValidateLabelsPath,
            };

            _drawerTree.OnPropertyValueChanged += (property, index) => EditorUtility.SetDirty(_importSettings);
        }

        public void Draw(string filter)
        {
            if (_sourceChanged) {
                _sourceChanged = false;
                return;
            }
            
            FilterRules(filter);
            
            var property = _drawerTree.GetPropertyAtPath(nameof(rulesView));
            property.Draw();

            ApplyChanges();

        }
        
        public bool ValidateRule(AddressableImportRule rule,string filter)
        {
            return string.IsNullOrEmpty(filter) || _filters.Any(x => x(rule,filter));
        }

        public bool ValidateAddressableGroupName(AddressableImportRule rule, string filter)
        {
            return rule.groupName.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0;
        }
        
        public bool ValidateRulePath(AddressableImportRule rule, string filter)
        {
            return rule.path.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0;
        }
        
        public bool ValidateLabelsPath(AddressableImportRule rule, string filter)
        {
            return rule.labels.Any(x => x.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private void FilterRules(string filter)
        {
            rulesView.Clear();
            var filteredRules = _importSettings.rules.
                Where(x => ValidateRule(x, filter));
            rulesView.AddRange(filteredRules);
        }

        private void ApplyChanges()
        {
            _drawerTree.ApplyChanges();

            for (var i = 0; i < rulesView.Count; i++) {
                var rule  = rulesView[i];
                var index = _importSettings.rules.IndexOf(rule);
                if(index < 0) continue;
                _importSettings.rules[i] = rulesView[i];
            }
            
        }
        
        private void CustomAddFunction()
        {
            _importSettings.rules.Add(new AddressableImportRule());
            _sourceChanged = true;
        }
        
        private void CustomRemoveIndexFunction(int index)
        {
            var removeResult = _importSettings.rules.Remove(rulesView[index]);
            _sourceChanged = true;
        }
        
        private void CustomRemoveElementFunction(AddressableImportRule item)
        {
            var index = rulesView.IndexOf(item);
            CustomRemoveIndexFunction(index);
        }

        private void EndOfListItemGui(int item)
        {
            if (GUILayout.Button("remove")) {
                CustomRemoveIndexFunction(item);
            }
        }

        private void OnDisable()
        {
            if (_drawerTree == null) return;
            _drawerTree.OnPropertyValueChanged -= OnPropertyChanged;
            _drawerTree.Dispose();
        }

        private void OnPropertyChanged(InspectorProperty property, int index)
        {
            if (_importSettings == null) return;
            EditorUtility.SetDirty(_importSettings);
        }
    }
#endif
}