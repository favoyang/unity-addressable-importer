using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using System;
using System.IO;
using System.Text.RegularExpressions;
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
                    var entry = CreateOrUpdateAddressableAssetEntry(settings, importSettings, rule, path);
                    if (entry != null)
                    {
                        entriesAdded.Add(entry);
                        if (rule.HasLabel)
                            Debug.LogFormat("[AddressableImporter] Entry created/updated for {0} with address {1} and labels {2}", path, entry.address, string.Join(", ", entry.labels));
                        else
                            Debug.LogFormat("[AddressableImporter] Entry created/updated for {0} with address {1}", path, entry.address);
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

    static AddressableAssetEntry CreateOrUpdateAddressableAssetEntry(
        AddressableAssetSettings settings,
        AddressableImportSettings importSettings,
        AddressableImportRule rule,
        string path)
    {
        // The regex to apply to the path. If Simplified is ticked, it a pattern that matches any path, capturing the path, filename and extension.
        // If the mode is Wildcard, the pattern will match and capture the entire path string.
        string pathRegex =
            rule.simplified
            ? @"(?<path>.*[/\\])+(?<filename>.+?)(?<extension>\.[^.]*$|$)"
            : (rule.matchType == AddressableImportRuleMatchType.Wildcard
                ? @"(.*)"
                : rule.path);

        // Set group
        AddressableAssetGroup group;
        var groupName = rule.ParseRegexPath(path, rule.groupName);
        if (!TryGetGroup(settings, groupName, out group))
        {
            if (importSettings.allowGroupCreation)
            {
                //TODO Specify on editor which type to create.
                group = CreateAssetGroup<BundledAssetGroupSchema>(settings, groupName);
            }
            else
            {
                Debug.LogErrorFormat("[AddressableImporter] Failed to find group {0} when importing {1}. Please check if the group exists, then reimport the asset.", rule.groupName, path);
                return null;
            }
        }
        var guid = AssetDatabase.AssetPathToGUID(path);
        var entry = settings.CreateOrMoveEntry(guid, group);

        // Address replacement.
        // The replacement string passed into Regex.Replace. If Simplified is ticked, it's the filename, without the extension.
        // If the mode is Wildcard, it's the entire path, i.e. the first capture group.
        string addressReplacement =
            rule.simplified
            ? @"${filename}"
            : (rule.matchType == AddressableImportRuleMatchType.Wildcard
                ? @"$1"
                : rule.addressReplacement);
        // Apply address replacement if address is empty or path.
        if (string.IsNullOrEmpty(entry.address) || entry.address.StartsWith("Assets/"))
        {
            if (!string.IsNullOrEmpty(pathRegex) && !string.IsNullOrEmpty(addressReplacement))
                entry.address = Regex.Replace(path, pathRegex, addressReplacement);
            else
                entry.address = path;
        }

        // Add labels
        if (rule.LabelMode == LabelWriteMode.Replace)
            entry.labels.Clear();
        foreach (var label in rule.labels)
        {
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
