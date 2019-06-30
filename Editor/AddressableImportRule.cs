using UnityEngine;
using UnityEditor;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public enum AddressableImportRuleMatchType
{
    /// <summary>
    /// Simple wildcard
    /// *, matches any number of characters
    /// ?, matches a single character
    /// </summary>
    Wildcard = 0,

    /// <summary>
    /// Regex pattern
    /// </summary>
    Regex
}

[System.Serializable]
public class AddressableImportRule
{
    /// <summary>
    /// Path pattern
    /// </summary>
    public string path;

    /// <summary>
    /// Path match type
    /// </summary>
    public AddressableImportRuleMatchType matchType;

    /// <summary>
    /// Addressable group name
    /// </summary>
    [Tooltip("Leaves blank for the default group.")]
    public string groupName;

    /// <summary>
    /// Label reference list
    /// </summary>
    public List<AssetLabelReference> labelRefs;

    /// <summary>
    /// Simplify address
    /// </summary>
    [Tooltip("Simplify address to filename without extension")]
    public bool simplified;

    public bool HasLabel
    {
        get
        {
            return labelRefs != null && labelRefs.Count > 0;
        }
    }

    /// <summary>
    /// Returns True if given assetPath matched with the rule.
    /// </summary>
    public bool Match(string assetPath)
    {
        if (matchType == AddressableImportRuleMatchType.Wildcard)
        {
            if (path.Contains("*") || path.Contains("?"))
            {
                var regex = "^" + Regex.Escape(path).Replace(@"\*", ".*").Replace(@"\?", ".");
                return Regex.IsMatch(assetPath, regex);
            }
            else
                return assetPath.StartsWith(path);
        } else if (matchType == AddressableImportRuleMatchType.Regex)
            return Regex.IsMatch(assetPath, path);
        return false;
    }

    public IEnumerable<string> labels
    {
        get
        {
            if (labelRefs == null)
                yield break;
            else
            {
                foreach (var labelRef in labelRefs)
                {
                    yield return labelRef.labelString;
                }
            }
        }
    }
}