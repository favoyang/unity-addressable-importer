﻿#if ODIN_INSPECTOR

namespace UnityAddressableImporter.Helper
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEngine;
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;

    public class AddressableImportRuleAttributeProcessor<T> : OdinAttributeProcessor<T>
        where T : AddressableImportRule
    {
        private const string PROP_PATH = nameof(AddressableImportRule.path);
        private const string PROP_MATCH_TYPE = nameof(AddressableImportRule.matchType);
        private const string PROP_APPLICATION_MODE = nameof(AddressableImportRule.groupTemplateApplicationMode);

        private const string PROP_SIMPLIFIED = nameof(AddressableImportRule.simplified);
        private const string PROP_ADDRESS_REPLACEMENT = nameof(AddressableImportRule.addressReplacement);

        private const string RULE_FOLDOUT = "Rule";
        private static readonly string RULE_FOLDOUT_LABEL = $"@{PROP_PATH}";

        private static readonly Color RULE_COLOR = Color.green;
        private static readonly FoldoutColorMode RULE_COLOR_MODE = FoldoutColorMode.OnExpanded;
        private static readonly FoldoutColorSelector RULE_COLOR_SELECTOR = FoldoutColorSelector.Header;

        public override void ProcessChildMemberAttributes(
            InspectorProperty parentProperty,
            MemberInfo member,
            List<Attribute> attributes)
        {
            attributes.Add(new PropertySpaceAttribute(2));
            attributes.Add(new LabelWidthAttribute(130f));

            attributes.Add(new FoldoutColorGroupAttribute(RULE_FOLDOUT, RULE_FOLDOUT_LABEL)
            {
                Color = RULE_COLOR,
                ColorMode = RULE_COLOR_MODE,
                ColorSelector = RULE_COLOR_SELECTOR,
                PaddingTop = 5,
                PaddingBottom = 5,
                MarginTop = 2,
                MarginBottom = 5
            });

            switch (member.Name)
            {
                case PROP_MATCH_TYPE:
                    attributes.Add(new IndentAttribute());
                    break;

                case PROP_APPLICATION_MODE:
                    attributes.Add(new IndentAttribute());
                    attributes.Add(new LabelTextAttribute("Application Mode"));
                    break;

                case PROP_SIMPLIFIED:
                    attributes.Add(new LabelTextAttribute("Address Simplified"));
                    break;

                case PROP_ADDRESS_REPLACEMENT:
                    attributes.Add(new HideIfAttribute(PROP_MATCH_TYPE, AddressableImportRuleMatchType.Wildcard));
                    break;
            }
        }
    }
}

#endif
