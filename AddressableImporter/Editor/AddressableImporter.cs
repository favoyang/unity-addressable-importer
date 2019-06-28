using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using System;

public class AddressableImporter : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        var type = typeof(AddressableImporter);
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        var importSettings = AddressableImportSettings.Instance;
        if (importSettings.rules == null || importSettings.rules.Count == 0)
            return;
        var entriesAdded = new List<AddressableAssetEntry>();
        foreach (string path in importedAssets)
        {
            foreach (var rule in importSettings.rules)
            {
                if (rule.Match(path))
                {
                    var entry = CreateOrUpdateAddressableAssetEntry(settings, path, rule.labels);
                    entriesAdded.Add(entry);
                    if (rule.HasLabel)
                        Debug.LogFormat("[{0}] Entry created for {1} with labels {2}", type, path, string.Join(", ", entry.labels));
                    else
                        Debug.LogFormat("[{0}] Entry created for {1}", type, path);
                }
            }
        }
        if (entriesAdded.Count > 0)
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, true);
    }

    static AddressableAssetEntry CreateOrUpdateAddressableAssetEntry(AddressableAssetSettings settings, string path, IEnumerable<string> labels)
    {
        var group = settings.DefaultGroup;
        var guid = AssetDatabase.AssetPathToGUID(path);
        var entry = settings.CreateOrMoveEntry(guid, group);
        // Override address if address is a path
        if (string.IsNullOrEmpty(entry.address) || entry.address.StartsWith("Assets/"))
            entry.address = path;
        // Add labels
        foreach (var label in labels)
        {
            if (!entry.labels.Contains(label))
                entry.labels.Add(label);
        }
        return entry;
    }
}

