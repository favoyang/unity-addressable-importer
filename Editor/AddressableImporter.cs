using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using System;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;

#if UNITY_2021_2_OR_NEWER
using UnityEditor.SceneManagement;
#else
using UnityEditor.Experimental.SceneManagement;
#endif

public class AddressableImporter : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        // Skip if all imported and deleted assets are addressables configurations.
        var isConfigurationPass =
            (importedAssets.Length > 0 && importedAssets.All(x => x.StartsWith("Assets/AddressableAssetsData"))) &&
            (deletedAssets.Length > 0 && deletedAssets.All(x => x.StartsWith("Assets/AddressableAssetsData")));
        if (isConfigurationPass)
        {
            return;
        }
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            if (!EditorApplication.isUpdating && !EditorApplication.isCompiling)
            {
                Debug.LogWarningFormat("[Addressables] settings file not found.\nPlease go to Menu/Window/Asset Management/Addressables, then click 'Create Addressables Settings' button.");
            }
            return;
        }
        var importSettings = AddressableImportSettings.Instance;
        if (importSettings == null)
        {
            Debug.LogWarningFormat("[AddressableImporter] import settings file not found.\nPlease go to Assets/AddressableAssetsData folder, right click in the project window and choose 'Create > Addressables > Import Settings'.");
            return;
        }
        if (importSettings.rules == null || importSettings.rules.Count == 0)
            return;

        var dirty = false;

        // Apply import rules.
        var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
#if UNITY_2020_1_OR_NEWER
        string prefabAssetPath = prefabStage != null ? prefabStage.assetPath : null;
#else
        string prefabAssetPath = prefabStage != null ? prefabStage.prefabAssetPath : null;
#endif
        foreach (var importedAsset in importedAssets)
        {
            if (prefabStage == null || prefabAssetPath != importedAsset) // Ignore current editing prefab asset.
                dirty |= ApplyImportRule(importedAsset, null, settings, importSettings);
        }

        for (var i = 0; i < movedAssets.Length; i++)
        {
            var movedAsset = movedAssets[i];
            var movedFromAssetPath = movedFromAssetPaths[i];
            if (prefabStage == null || prefabAssetPath != movedAsset) // Ignore current editing prefab asset.
                dirty |= ApplyImportRule(movedAsset, movedFromAssetPath, settings, importSettings);
        }

        foreach (var deletedAsset in deletedAssets)
        {
            if (TryGetMatchedRule(deletedAsset, importSettings, out var matchedRule))
            {
                var guid = AssetDatabase.AssetPathToGUID(deletedAsset);
                if (!string.IsNullOrEmpty(guid) && settings.RemoveAssetEntry(guid))
                {
                    dirty = true;
                    Debug.LogFormat("[AddressableImporter] Entry removed for {0}", deletedAsset);
                }
            }
        }

        if (dirty)
        {
            AssetDatabase.SaveAssets();
        }
    }

    static AddressableAssetGroup CreateAssetGroup<SchemaType>(AddressableAssetSettings settings, string groupName)
    {
        return settings.CreateGroup(groupName, false, false, false, new List<AddressableAssetGroupSchema> { settings.DefaultGroup.Schemas[0] }, typeof(SchemaType));
    }

    static bool ApplyImportRule(
        string assetPath,
        string movedFromAssetPath,
        AddressableAssetSettings settings,
        AddressableImportSettings importSettings)
    {
        var dirty = false;
        if (TryGetMatchedRule(assetPath, importSettings, out var matchedRule))
        {
            // Apply the matched rule.
            var entry = CreateOrUpdateAddressableAssetEntry(settings, importSettings, matchedRule, assetPath);
            if (entry != null)
            {
                if (matchedRule.HasLabelRefs)
                    Debug.LogFormat("[AddressableImporter] Entry created/updated for {0} with address {1} and labels {2}", assetPath, entry.address, string.Join(", ", entry.labels));
                else
                    Debug.LogFormat("[AddressableImporter] Entry created/updated for {0} with address {1}", assetPath, entry.address);
            }

            dirty = true;
        }
        else
        {
            // If assetPath doesn't match any of the rules, try to remove the entry.
            // But only if movedFromAssetPath has the matched rule, because the importer should not remove any unmanaged entries.
            if (!string.IsNullOrEmpty(movedFromAssetPath) && TryGetMatchedRule(movedFromAssetPath, importSettings, out matchedRule))
            {
                var guid = AssetDatabase.AssetPathToGUID(assetPath);
                if (settings.RemoveAssetEntry(guid))
                {
                    dirty = true;
                    Debug.LogFormat("[AddressableImporter] Entry removed for {0}", assetPath);
                }
            }
        }

        return dirty;
    }

    static AddressableAssetEntry CreateOrUpdateAddressableAssetEntry(
        AddressableAssetSettings settings,
        AddressableImportSettings importSettings,
        AddressableImportRule rule,
        string assetPath)
    {
        // Set group
        AddressableAssetGroup group;
        var groupName = rule.ParseGroupReplacement(assetPath);
        bool newGroup = false;
        if (!TryGetGroup(settings, groupName, out group))
        {
            if (importSettings.allowGroupCreation)
            {
                //TODO Specify on editor which type to create.
                group = CreateAssetGroup<BundledAssetGroupSchema>(settings, groupName);
                newGroup = true;
            }
            else
            {
                Debug.LogErrorFormat("[AddressableImporter] Failed to find group {0} when importing {1}. Please check if the group exists, then reimport the asset.", rule.groupName, assetPath);
                return null;
            }
        }

        // Set group settings from template if necessary
        if (rule.groupTemplate != null && (newGroup || rule.groupTemplateApplicationMode == GroupTemplateApplicationMode.AlwaysOverwriteGroupSettings))
        {
            rule.groupTemplate.ApplyToAddressableAssetGroup(group);
        }

        var guid = AssetDatabase.AssetPathToGUID(assetPath);
        var entry = settings.CreateOrMoveEntry(guid, group);

        if (entry != null)
        {
            // Apply address replacement if address is empty or path.
            if (string.IsNullOrEmpty(entry.address) ||
                entry.address.StartsWith("Assets/") ||
                rule.simplified ||
                !string.IsNullOrWhiteSpace(rule.addressReplacement))
            {
                entry.address = rule.ParseAddressReplacement(assetPath);
            }

            // Add labels
            if (rule.LabelMode == LabelWriteMode.Replace)
                entry.labels.Clear();

            if (rule.labelsRefsEnum != null)
            {
                foreach (var label in rule.labelsRefsEnum)
                {
                    entry.labels.Add(label);
                }
            }

            if (rule.dynamicLabels != null)
            {
                foreach (var dynamicLabel in rule.dynamicLabels)
                {
                    var label = rule.ParseReplacement(assetPath, dynamicLabel);
                    settings.AddLabel(label);
                    entry.labels.Add(label);
                }
            }
        }
        return entry;
    }

    static bool TryGetMatchedRule(
        string assetPath,
        AddressableImportSettings importSettings,
        out AddressableImportRule rule)
    {
        foreach (var r in importSettings.rules)
        {
            if (!r.Match(assetPath))
                continue;
            rule = r;
            return true;
        }

        rule = null;
        return false;
    }

    /// <summary>
    /// Find asset group by given name. Return default group if given name is null.
    /// </summary>
    static AddressableAssetGroup GetGroup(AddressableAssetSettings settings, string groupName)
    {
        if (groupName != null)
            groupName.Trim();
        if (string.IsNullOrEmpty(groupName))
            return settings.DefaultGroup;
        return settings.groups.Find(g => g.Name == groupName);
    }

    /// <summary>
    /// Attempts to get the group using the provided <paramref name="groupName"/>.
    /// </summary>
    /// <param name="settings">Reference to the <see cref="AddressableAssetSettings"/></param>
    /// <param name="groupName">The name of the group for the search.</param>
    /// <param name="group">The <see cref="AddressableAssetGroup"/> if found. Set to <see cref="null"/> if not found.</param>
    /// <returns>True if a group is found.</returns>
    static bool TryGetGroup(AddressableAssetSettings settings, string groupName, out AddressableAssetGroup group)
    {
        if (string.IsNullOrWhiteSpace(groupName))
        {
            group = settings.DefaultGroup;
            return true;
        }
        return ((group = settings.groups.Find(g => string.Equals(g.Name, groupName.Trim()))) == null) ? false : true;
    }

    /// <summary>
    /// Allows assets within the selected folder to be checked agains the Addressable Importer rules.
    /// </summary>
    public class FolderImporter
    {
        /// <summary>
        /// Reimporter folders.
        /// </summary>
        /// <param name="settings">Reference to the <see cref="AddressableAssetSettings"/></param>
        public static void ReimportFolders(IEnumerable<String> assetPaths)
        {
            HashSet<string> pathsToImport = new HashSet<string>();
            foreach (var assetPath in assetPaths)
            {
                if (Directory.Exists(assetPath))
                {
                    // Add the folder itself.
                    pathsToImport.Add(assetPath.Replace('\\', '/'));
                    // Add sub-folders.
                    var dirsToAdd = Directory.GetDirectories(assetPath, "*", SearchOption.AllDirectories);
                    foreach (var dir in dirsToAdd)
                    {
                        // Filter out .dirname and dirname~, those are invisible to Unity.
                        if (!dir.StartsWith(".") && !dir.EndsWith("~"))
                        {
                            pathsToImport.Add(dir.Replace('\\', '/'));
                        }
                    }
                    // Add files.
                    var filesToAdd = Directory.GetFiles(assetPath, "*", SearchOption.AllDirectories);
                    foreach (var file in filesToAdd)
                    {
                        // Filter out meta and DS_Store files.
                        if (!file.EndsWith(".meta") && !file.EndsWith(".DS_Store"))
                        {
                            pathsToImport.Add(file.Replace('\\', '/'));
                        }
                    }
                }
            }
            if (pathsToImport.Count > 0)
            {
                Debug.Log($"AddressableImporter: Found {pathsToImport.Count} asset paths...");
                OnPostprocessAllAssets(pathsToImport.ToArray(), new string[0], new string[0], new string[0]);
            }
        }

        /// <summary>
        /// Allows assets within the selected folder to be checked agains the Addressable Importer rules.
        /// </summary>
        [MenuItem("Assets/AddressableImporter: Check Folder(s)")]
        private static void CheckFoldersFromSelection()
        {
            List<string> assetPaths = new List<string>();
            // Folders comes up as Object.
            foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
            {
                var assetPath = AssetDatabase.GetAssetPath(obj);
                // Other assets may appear as Object, so a Directory Check filters directories from folders.
                if (Directory.Exists(assetPath))
                {
                    assetPaths.Add(assetPath);
                }
            }
            ReimportFolders(assetPaths);
        }

        // Note that we pass the same path, and also pass "true" to the second argument.
        [MenuItem("Assets/AddressableImporter: Check Folder(s)", true)]
        private static bool ValidateCheckFoldersFromSelection()
        {
            foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
            {
                if (Directory.Exists(AssetDatabase.GetAssetPath(obj)))
                {
                    return true;
                }
            }
            return false;
        }
    }


}