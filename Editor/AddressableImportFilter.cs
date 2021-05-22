using System;
using System.Collections.Generic;
using System.Linq;

public class AddressableImportFilter
{
    #region static data

    private static List<string> matchTypeNames = Enum.GetNames(typeof(AddressableImportRuleMatchType)).ToList();

    #endregion

    private List<Func<AddressableImportRule, string, bool>> _filters;


    public AddressableImportFilter()
    {
        _filters = new List<Func<AddressableImportRule, string, bool>>()
        {
            ValidateAddressableGroupName,
            ValidateRulePath,
            ValidateLabelsPath,
            ValidateMatchType,
            ValidateGroupTemplate,
            ValidateAddressReplacement,
        };
    }

    public bool IsMatch(AddressableImportRule rule, string filter)
    {
        for (var i = 0; i < _filters.Count; i++)
        {
            var filterRule = _filters[i];
            var result = filterRule(rule, filter);
            if (result)
            {
                return true;
            }
        }

        return false;
    }

    private bool ValidateAddressableGroupName(AddressableImportRule rule, string filter)
    {
        var result = rule.groupName.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0;
        return result;
    }

    private bool ValidateAddressReplacement(AddressableImportRule rule, string filter)
    {
        var result = rule.addressReplacement.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0;
        return result;
    }

    private bool ValidateRulePath(AddressableImportRule rule, string filter)
    {
        var result = rule.path.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0;
        return result;
    }

    private bool ValidateGroupTemplate(AddressableImportRule rule, string filter)
    {
        var template = rule.groupTemplate;
        if (!template) return string.IsNullOrEmpty(filter);
        var result = template.Name.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0
                     || template.Description.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0;
        return result;
    }

    private bool ValidateMatchType(AddressableImportRule rule, string filter)
    {
        var matchTypeName = matchTypeNames[(int) rule.matchType];
        var result = matchTypeName.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0;
        return result;
    }

    private bool ValidateLabelsPath(AddressableImportRule rule, string filter)
    {
        var labels = rule.labelRefs
            .Select(x => x.labelString)
            .Concat(rule.dynamicLabels);

        var result = labels.Any(x => x.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0);
        return result;
    }
}