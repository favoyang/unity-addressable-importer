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

    public class AddressableImporterFilterOdinHandler : ScriptableObject
    {
        private AddressableImportSettings                       _importSettings;
        private PropertyTree                                    _drawerTree;
        private List<Func<AddressableImportRule, string, bool>> _filters;
        //private List<AddressableImportRule>                     _filteredRules;
        private bool _sourceChanged = false;

        [SerializeField]
        [HideLabel]
        [OnValueChanged("OnFilterChanged")]
        private string _searchFilter;

        [SerializeField]
        [ListDrawerSettings(
            HideRemoveButton = true,
            Expanded = true,
            CustomAddFunction = nameof(CustomAddFunction),
            OnEndListElementGUI = nameof(EndOfListItemGui),
            CustomRemoveElementFunction = nameof(CustomRemoveElementFunction),
            CustomRemoveIndexFunction = nameof(CustomRemoveIndexFunction),
            ShowPaging = true
        )]
        private List<AddressableImportRule> rules = new List<AddressableImportRule>();

        public void Initialize(AddressableImportSettings importSettings)
        {
            _importSettings = importSettings;
            _drawerTree     = PropertyTree.Create(this);

            _filters = new List<Func<AddressableImportRule, string, bool>>() {
                ValidateAddressableGroupName,
                ValidateRulePath,
                ValidateLabelsPath,
            };

            _drawerTree.OnPropertyValueChanged += (property, index) => EditorUtility.SetDirty(_importSettings);
        }

        public void Draw()
        {
            try {
                FilterRules(_searchFilter);
                _drawerTree.Draw();
                ApplyChanges();
            }
            catch (Exception e) {
                Debug.LogError(e);
            }

        }

        [Button]
        public void Save() => _importSettings.Save();

        [Button]
        public void Documentation() => _importSettings.Documentation();

        [Button]
        public void CleanEmptyGroup() => _importSettings.CleanEmptyGroup();

        #region private methods

        private void OnFilterChanged()
        {

        }

        private bool ValidateRule(AddressableImportRule rule,string filter)
        {
            return string.IsNullOrEmpty(filter) || _filters.Any(x => x(rule,filter));
        }

        private bool ValidateAddressableGroupName(AddressableImportRule rule, string filter)
        {
            return rule.groupName.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private bool ValidateRulePath(AddressableImportRule rule, string filter)
        {
            return rule.path.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private bool ValidateLabelsPath(AddressableImportRule rule, string filter)
        {
            return rule.labels.Any(x => x.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private void FilterRules(string filter)
        {
            rules = new List<AddressableImportRule>();
            var filteredRules = _importSettings.rules.
                Where(x => ValidateRule(x, filter));
            rules.AddRange(filteredRules);
        }

        private void ApplyChanges()
        {
            _drawerTree.ApplyChanges();

            for (var i = 0; i < rules.Count; i++) {
                var rule  = rules[i];
                var index = _importSettings.rules.IndexOf(rule);
                if(index < 0) continue;
                _importSettings.rules[index] = rules[i];
            }

        }

        private void CustomAddFunction()
        {
            _importSettings.rules.Add(new AddressableImportRule());
            _sourceChanged = true;
        }

        private void CustomRemoveIndexFunction(int index)
        {
            var removeResult = _importSettings.rules.Remove(rules[index]);
            _sourceChanged = true;
        }

        private void CustomRemoveElementFunction(AddressableImportRule item)
        {
            var index = rules.IndexOf(item);
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

        #endregion
    }
#endif
}