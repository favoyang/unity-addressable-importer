using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;
using System.Collections.Generic;
using System.Linq;
using UnityAddressableImporter.Helper;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

[CreateAssetMenu(fileName = "AddressableImportSettings", menuName = "Addressable Assets/Import Settings", order = 50)]
public class AddressableImportSettings : ScriptableObject
{
    [Tooltip("Toggle rules enabled state")]
    [SerializeField]
    private bool rulesEnabled = true;
    
    [Tooltip("Creates a group if the specified group doesn't exist.")]
    public bool allowGroupCreation = false;

    [Tooltip("Rules for managing imported assets.")]
#if ODIN_INSPECTOR
    [ListDrawerSettings(HideAddButton = false,Expanded = false,DraggableItems = true,HideRemoveButton = false)]
    [Searchable(FilterOptions = SearchFilterOptions.ISearchFilterableInterface)]
#endif
    public List<AddressableImportRule> rules = new List<AddressableImportRule>();

    [ButtonMethod]
    public void Save()
    {
        AssetDatabase.SaveAssets();
    }

    [ButtonMethod]
    public void Documentation()
    {
        Application.OpenURL("https://github.com/favoyang/unity-addressable-importer/blob/master/Documentation~/AddressableImporter.md");
    }

    [ButtonMethod]
    public void CleanEmptyGroup()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            return;
        }
        var dirty = false;
        var emptyGroups = settings.groups.Where(x => x.entries.Count == 0 && !x.IsDefaultGroup()).ToArray();
        for (var i = 0; i < emptyGroups.Length; i++)
        {
            dirty = true;
            settings.RemoveGroup(emptyGroups[i]);
        }
        if (dirty)
        {
            AssetDatabase.SaveAssets();
        }
    }

    public static AddressableImportSettings[] Instances
    {
        get
        {
            return AssetDatabase.FindAssets($"t:{nameof(AddressableImportSettings)}")
                .Select(guid =>
                    AssetDatabase.LoadAssetAtPath<AddressableImportSettings>(AssetDatabase.GUIDToAssetPath(guid)))
                .Where(settings => settings.rulesEnabled)
                .ToArray();
        }
    }
}