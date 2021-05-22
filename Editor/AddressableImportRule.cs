using UnityEngine;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityAddressableImporter.Helper;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

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

public enum GroupTemplateApplicationMode
{
    ApplyOnGroupCreationOnly,
    AlwaysOverwriteGroupSettings
}



[System.Serializable]
public class AddressableImportRule
    : ISearchFilterable
{
    #region inspector
    
    /// <summary>
    /// Path pattern.
    /// </summary>
    [Tooltip("The assets in this path will be processed.")]
    public string path = string.Empty;

    /// <summary>
    /// Method used to parse the Path.
    /// </summary>
    [Tooltip("The path parsing method.")]
    public AddressableImportRuleMatchType matchType;

    /// <summary>
    /// The group the asset will be added.
    /// </summary>
    [Tooltip("The group name in which the Addressable will be added. Leave blank for the default group.")]
    public string groupName = string.Empty;


    /// <summary>
    /// Defines if labels will be added or replaced.
    /// </summary>
    public LabelWriteMode LabelMode;

    /// <summary>
    /// Label reference list.
    /// </summary>
    [Tooltip("The list of addressable labels (already existing in your project) to be added to the Addressable Asset")]
    public List<AssetLabelReference> labelRefs;

    [Tooltip("The list of dynamic labels to be added to the Addressable Asset. If an addressable label doesn't exist, then it will be create in your unity project")]
    public List<string> dynamicLabels;

    /// <summary>
    /// Group template to use. Default Group settings will be used if empty.
    /// </summary>
    [Tooltip("Group template that will be applied to the Addressable Group. Leave none to use the Default Group's settings.")]
    public AddressableAssetGroupTemplate groupTemplate = null;

    /// <summary>
    /// Controls wether group template will be applied only on group creation, or also to already created groups.
    /// </summary>
    [Tooltip("Defines if the group template will only be applied to new groups, or will also overwrite existing groups settings.")]
    public GroupTemplateApplicationMode groupTemplateApplicationMode = GroupTemplateApplicationMode.ApplyOnGroupCreationOnly;

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
    public string addressReplacement = string.Empty;

    #endregion
    
    
    private AddressableImportFilter _filter = new AddressableImportFilter();
    public AddressableImportFilter Filter => _filter ?? new AddressableImportFilter();

    
    public bool HasLabelRefs
    {
        get
        {
            return labelRefs != null && labelRefs.Count > 0;
        }
    }

    /// <summary>
    /// check current rule for matching with filter pattern
    /// </summary>
    /// <param name="searchString">filter string</param>
    /// <returns>filter result</returns>
    public bool IsMatch(string searchString)
    {
        return string.IsNullOrEmpty(searchString) || Filter.IsMatch(this,searchString);
    }
    
    /// <summary>
    /// Returns True if given assetPath matched with the rule.
    /// </summary>
    public bool Match(string assetPath)
    {
        path = path.Trim();
        if (string.IsNullOrEmpty(path))
            return false;
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

    /// <summary>
    /// Parse assetPath and replace all elements that match this.path regex
    /// with the groupName string.
    /// Returns null if this.path or groupName is empty.
    /// </summary>
    public string ParseGroupReplacement(string assetPath)
    {
        return ParseReplacement(assetPath, groupName);

    }

    /// <summary>
    /// Parse assetPath and replace all elements that match this.path regex
    /// with the <paramref name="name"/>
    /// Returns null if this.path or  <paramref name="name"/> is empty.
    /// </summary>
    /// <param name="assetPath"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public string ParseReplacement(string assetPath, string name)
    {
        if (string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(name))
            return null;

        var cleanedName = name.Trim().Replace('/', '-').Replace('\\', '-');

        // Parse path elements.
        var replacement = AddressableImportRegex.ParsePath(assetPath, cleanedName);
        // Parse this.path regex.
        if (matchType == AddressableImportRuleMatchType.Regex)
        {
            string pathRegex = path;
            replacement = Regex.Replace(assetPath, pathRegex, replacement);
        }
        return replacement;
    }

    /// <summary>
    /// Parse assetPath and replace all elements that match this.path regex
    /// with the addressReplacement string.
    /// Returns assetPath if this.path or addressReplacement is empty.
    /// </summary>
    public string ParseAddressReplacement(string assetPath)
    {
        if (string.IsNullOrWhiteSpace(path))
            return assetPath;
        if (!simplified && string.IsNullOrWhiteSpace(addressReplacement))
            return assetPath;
        // Parse path elements.
        if (addressReplacement == null)
            addressReplacement = "";
        var replacement = AddressableImportRegex.ParsePath(assetPath, addressReplacement);
        // Parse this.path regex.
        // If Simplified is ticked, it's a pattern that matches any path, capturing the path, filename and extension.
        // If the match type is Wildcard, the pattern will match and capture the entire path string.
        string pathRegex =
            simplified
                ? @"(?<path>.*[/\\])+(?<filename>.+?)(?<extension>\.[^.]*$|$)"
                : (matchType == AddressableImportRuleMatchType.Wildcard
                    ? @"(.*)"
                    : path);
        replacement =
            simplified
                ? @"${filename}"
                : (matchType == AddressableImportRuleMatchType.Wildcard
                    ? @"$1"
                    : replacement);
        replacement = Regex.Replace(assetPath, pathRegex, replacement);
        return replacement;
    }

    public IEnumerable<string> labelsRefsEnum
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

    /// <summary>
    /// Helper class for regex replacement.
    /// </summary>
    static class AddressableImportRegex
    {
        const string pathregex = @"\$\{PATH\[\-{0,1}\d{1,3}\]\}"; // ie: ${PATH[0]} ${PATH[-1]}

        static public string[] GetPathArray(string path)
        {
            return path.Split('/');
        }

        static public string GetPathAtArray(string path, int idx)
        {
            return GetPathArray(path)[idx];
        }

        /// <summary>
        /// Parse assetPath and replace all matched path elements (i.e. `${PATH[0]}`)
        /// with a specified replacement string.
        /// </summary>
        static public string ParsePath(string assetPath, string replacement)
        {
            var _path = assetPath;
            int i = 0;
            var slashSplit = _path.Split('/');
            var len = slashSplit.Length - 1;
            var matches = Regex.Matches(replacement, pathregex);
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
            var splitpath = Regex.Split(replacement, pathregex);
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