using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityAddressableImporter.Helper;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

// [CreateAssetMenu(fileName = "AddressableImportSettings", menuName = "Addressable Assets/Import Settings", order = 50)]
public class AddressableImportSettings : ScriptableObject
{
    public const string kDefaultConfigObjectName = "addressableimportsettings";
    public const string kDefaultPath = "Assets/AddressableAssetsData/AddressableImportSettings.asset";

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

    public static AddressableImportSettings Instance
    {
        get
        {
            AddressableImportSettings so;
            // Try to locate settings via EditorBuildSettings.
            if (EditorBuildSettings.TryGetConfigObject(kDefaultConfigObjectName, out so))
                return so;
            // Try to locate settings via path.
            so = AssetDatabase.LoadAssetAtPath<AddressableImportSettings>(kDefaultPath);
            if (so != null) {
                EditorBuildSettings.AddConfigObject(kDefaultConfigObjectName, so, true);
            }
            else {
                var path = AssetDatabase.FindAssets("t: AddressableImportSettings");
                if (path.Length == 0) {
                    return so;
                }
                var assetPath = AssetDatabase.GUIDToAssetPath(path[0]);
                so = AssetDatabase.LoadAssetAtPath<AddressableImportSettings>(assetPath);
                EditorBuildSettings.AddConfigObject(assetPath, so, true);
            }
            return so;
        }
    }

    [MenuItem("Assets/Create/Addressable Assets/Import Settings", priority = 50)]
    public static void CreateAddressableImportAsset() {
        const string defaultAssetName = "AddressableImportSettings";
        // Find if asset already exist
        var tryLoadExist = AssetDatabase.FindAssets("t: AddressableImportSettings");
        var exist = tryLoadExist.Length > 0;
        if (exist) {
            var path = AssetDatabase.GUIDToAssetPath(tryLoadExist[0]);
            var instance = AssetDatabase.LoadAssetAtPath<AddressableImportSettings>(path);
            Debug.Log($"AddressableImportSettings already existed at path: {path}", instance);
            return;
        }

        var newInstance = CreateInstance<AddressableImportSettings>();
        newInstance.name = defaultAssetName;
        if (!Directory.Exists(kDefaultPath)) {
            Directory.CreateDirectory(kDefaultPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        AssetDatabase.CreateAsset(newInstance, kDefaultPath);
        EditorUtility.SetDirty(newInstance);
        AssetDatabase.SaveAssets();
    }
}