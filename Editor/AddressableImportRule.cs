using UnityEngine;
using UnityEditor;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityAddressableImporter.Helper;

public enum AddressableImportRuleMatchType
{
    /// <summary>
    /// Simple wildcard
    /// *, matches any number of characters
    /// ?, matches a single character
    /// </summary>
    [Tooltip("Simple wildcard.\n\"*\" matches any number of characters.\n\"?\" matches a single character.")]
    Wildcard = 0,

    /// <summary>
    /// Regex pattern
    /// </summary>
    [Tooltip("A regular expression pattern.")]
    Regex
}

public enum LabelWriteMode
{
    Add,
    Replace
}

[System.Serializable]
public class AddressableImportRule
{
    /// <summary>
    /// Path pattern.
    /// </summary>
    [Tooltip("The assets in this path will be processed.")]
    public string path;

    /// <summary>
    /// Method used to parse the Path.
    /// </summary>
    [Tooltip("The path parsing method.")]
    public AddressableImportRuleMatchType matchType;

    /// <summary>
    /// The group the asset will be added.
    /// </summary>
    [Tooltip("The group name in which the Addressable will be added. Leave blank for the default group.")]
    public string groupName;

    /// <summary>
    /// Defines if labels will be added or replaced.
    /// </summary>
    public LabelWriteMode LabelMode;

    /// <summary>
    /// Label reference list.
    /// </summary>
    [Tooltip("The list of labels to be added to the Addressable Asset")]
    public List<AssetLabelReference> labelRefs;

    /// <summary>
    /// Simplify address.
    /// </summary>
    [Tooltip("Simplify address to filename without extension.")]
    [Label("Address Simplified")]
    public bool simplified;

    /// <summary>
    /// Replacement string for the asset address. This is only useful with regex capture groups.
    /// </summary>
    [Tooltip("Replacement address string for regex matches.")]
    [ConditionalField("matchType", AddressableImportRuleMatchType.Regex, "simplified", false)]
    public string addressReplacement;

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
        }
        else if (matchType == AddressableImportRuleMatchType.Regex)
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

    public string ParseRegexPath(string assetPath, string replacement)
    {
        return AddressableImportRegex.ParsePath(assetPath, replacement);
    }

    static class AddressableImportRegex
    {
        const string pathregex = @"\%PATH\%\[\-{0,1}\d{1,3}\]"; // ie: $PATH$[0]

        static public string[] GetPathArray(string path)
        {
            return path.Split('/');
        }

        static public string GetPathAtArray(string path, int idx)
        {
            return GetPathArray(path)[idx];
        }

        static public string ParsePath(string assetPath, string targetGroupName)
        {
            var _path = assetPath;
            int i = 0;
            var slashSplit = _path.Split('/');
            var len = slashSplit.Length - 1;
            var matches = Regex.Matches(targetGroupName, pathregex);
            string[] parsedMatches = new string[matches.Count];
            foreach (var match in matches)
            {
                string v = match.ToString();
                var sidx = v.IndexOf('[') + 1;
                var eidx = v.IndexOf(']');
                int idx = int.Parse(v.Substring(sidx, eidx - sidx));
                while (idx > len)
                {
                    idx -= len;
                }
                while (idx < 0)
                {
                    idx += len;
                }
                //idx = Mathf.Clamp(idx, 0, slashSplit.Length - 1);
                parsedMatches[i++] = GetPathAtArray(_path, idx);
            }

            i = 0;
            var splitpath = Regex.Split(targetGroupName, pathregex);
            string finalPath = string.Empty;
            foreach (var split in splitpath)
            {
                finalPath += splitpath[i];
                if (i < parsedMatches.Length)
                {
                    finalPath += parsedMatches[i];
                }
                i++;
            }
            return finalPath;
        }
    }
}
