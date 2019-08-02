using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using System;
using System.IO;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;

public class AddressableImporter : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        var importSettings = AddressableImportSettings.Instance;
        if (importSettings == null || importSettings.rules == null || importSettings.rules.Count == 0)
            return;
        var entriesAdded = new List<AddressableAssetEntry>();
        foreach (string path in importedAssets)
        {
            foreach (var rule in importSettings.rules)
            {
                if (rule.Match(path))
                {
                    var entry = CreateOrUpdateAddressableAssetEntry(settings, path, rule.groupName, rule.labels, rule.simplified, importSettings);
                    if (entry != null)
                    {
                        entriesAdded.Add(entry);
                        if (rule.HasLabel)
                            Debug.LogFormat("[AddressableImporter] Entry created for {0} with labels {1}", path, string.Join(", ", entry.labels));
                        else
                            Debug.LogFormat("[AddressableImporter] Entry created for {0}", path);
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
            settings.groups.RemoveAll(_ => _.entries.Count == 0);
        }
    }

    static AddressableAssetGroup CreateAssetGroup<SchemaType>(AddressableAssetSettings settings, string groupName)
    {
        return settings.CreateGroup(groupName, false, false, false, new List<AddressableAssetGroupSchema> { settings.DefaultGroup.Schemas[0] }, typeof(SchemaType));
    }

    static AddressableAssetEntry CreateOrUpdateAddressableAssetEntry(AddressableAssetSettings settings, string path, string groupName, IEnumerable<string> labels, bool simplified, AddressableImportSettings importSettings)
    {
        var group = GetGroup(settings, groupName);
        if (group == null)
        {
            if (importSettings.allowGroupCreation)
            {
                //TODO Specify on editor which Schema to create.
                group = CreateAssetGroup<BundledAssetGroupSchema>(settings, groupName);
            }
            else
            {
                Debug.LogErrorFormat("[AddressableImporter] Failed to find group {0} when importing {1}. Please check the group exists, then reimport the asset.", groupName, path);
                return null;
            }
        }
        var guid = AssetDatabase.AssetPathToGUID(path);
        var entry = settings.CreateOrMoveEntry(guid, group);
        // Override address if address is a path
        if (string.IsNullOrEmpty(entry.address) || entry.address.StartsWith("Assets/"))
        {
            if (simplified)
                path = Path.GetFileNameWithoutExtension(path);
            entry.address = path;
        }
        // Add labels
        foreach (var label in labels)
        {
            if (!entry.labels.Contains(label))
                entry.labels.Add(label);
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

}

