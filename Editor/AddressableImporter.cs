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
using UnityEditor.Experimental.SceneManagement;

public class AddressableImporter : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Debug.LogWarningFormat("[Addressables] settings file not found.\nPlease go to Menu/Window/Asset Management/Addressables/Groups, then click 'Create Addressables Settings' button.");
            return;
        }
        var importSettings = AddressableImportSettings.Instance;
        if (importSettings == null) {
            Debug.LogWarningFormat("[AddressableImporter] import settings file not found.\nPlease go to Assets/AddressableAssetsData folder, right click in the project window and choose 'Create > Addressable Assets > Import Settings'.");
            return;
        }
        else if (importSettings.rules == null || importSettings.rules.Count == 0)
            return;
        var entriesAdded = new List<AddressableAssetEntry>();
        var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
        foreach (string assetPath in importedAssets)
        {
            // Ignore current editing prefab asset.
            if (prefabStage != null && prefabStage.prefabAssetPath == assetPath)
                continue;
            foreach (var rule in importSettings.rules)
            {
                if (rule.Match(assetPath))
                {
                    var entry = CreateOrUpdateAddressableAssetEntry(settings, importSettings, rule, assetPath);
                    if (entry != null)
                    {
                        entriesAdded.Add(entry);
                        if (rule.HasLabel)
                            Debug.LogFormat("[AddressableImporter] Entry created/updated for {0} with address {1} and labels {2}", assetPath, entry.address, string.Join(", ", entry.labels));
                        else
                            Debug.LogFormat("[AddressableImporter] Entry created/updated for {0} with address {1}", assetPath, entry.address);
                    }
                }
            }
        }
        if (entriesAdded.Count > 0)
        {
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, true);
            AssetDatabase.SaveAssets();
        }
        if (importSettings.removeEmtpyGroups)
        {
            settings.groups.RemoveAll(_ => _.entries.Count == 0 && !_.IsDefaultGroup());
        }
    }

    static AddressableAssetGroup CreateAssetGroup<SchemaType>(AddressableAssetSettings settings, string groupName)
    {
        return settings.CreateGroup(groupName, false, false, false, new List<AddressableAssetGroupSchema> { settings.DefaultGroup.Schemas[0] }, typeof(SchemaType));
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
            foreach (var label in rule.labels)
            {
                entry.labels.Add(label);
            }
        }
        return entry;
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
        /// Remporter folders.
        /// </summary>
        /// <param name="settings">Reference to the <see cref="AddressableAssetSettings"/></param>
        public static void ReimportFolders(IEnumerable<String> assetPaths)
        {
            HashSet<string> filesToImport = new HashSet<string>();
            foreach (var assetPath in assetPaths)
            {
                if (Directory.Exists(assetPath))
                {
                    var filesToAdd = Directory.GetFiles(assetPath, "*", SearchOption.AllDirectories);
                    foreach (var file in filesToAdd)
                    {
                        // Filter out meta and DS_Store files.
                        if (!file.EndsWith(".meta") && !file.EndsWith(".DS_Store"))
                        {
                            filesToImport.Add(file.Replace('\\', '/'));
                        }
                    }
                }
            }
            if (filesToImport.Count > 0)
            {
                Debug.Log($"AddressablesImporter: reimport {filesToImport.Count} assets...");
                OnPostprocessAllAssets(filesToImport.ToArray(), null, null, null);
            }
            else
            {
                Debug.Log($"AddressablesImporter: No files to reimport");
            }
        }

        /// <summary>
        /// Allows assets within the selected folder to be checked agains the Addressable Importer rules.
        /// </summary>
        [MenuItem("Assets/AddressablesImporter: Check Folder(s)")]
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
        [MenuItem("Assets/AddressablesImporter: Check Folder(s)", true)]
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
