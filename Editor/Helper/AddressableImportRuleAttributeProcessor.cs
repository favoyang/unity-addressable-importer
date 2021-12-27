#if ODIN_INSPECTOR

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
        private const string PROP_GROUP_NAME = nameof(AddressableImportRule.groupName);

        private const string PROP_PATH = nameof(AddressableImportRule.path);
        private const string PROP_MATCH_TYPE = nameof(AddressableImportRule.matchType);

        private const string PROP_LABEL_MODE = nameof(AddressableImportRule.LabelMode);
        private const string PROP_LABEL_REFS = nameof(AddressableImportRule.labelRefs);
        private const string PROP_DYNAMIC_LABEL = nameof(AddressableImportRule.dynamicLabels);

        private const string PROP_GROUP_TEMPLATE = nameof(AddressableImportRule.groupTemplate);
        private const string PROP_APPLICATION_MODE = nameof(AddressableImportRule.groupTemplateApplicationMode);

        private const string PROP_SIMPLIFIED = nameof(AddressableImportRule.simplified);
        private const string PROP_ADDRESS_REPLACEMENT = nameof(AddressableImportRule.addressReplacement);

        private const string RULE_FOLDOUT = "Rule";
        private static readonly string RULE_FOLDOUT_LABEL = $"@{PROP_GROUP_NAME}";

        private static readonly string TITLE_PATH = $"{RULE_FOLDOUT}/Asset Path";
        private static readonly string TITLE_TEMPLATE = $"{RULE_FOLDOUT}/Group Template";
        private static readonly string TITLE_ADDRESS = $"{RULE_FOLDOUT}/Address";

        private static readonly string LABEL_FOLDOUT = $"{RULE_FOLDOUT}/Label";
        private const string LABEL_FOLDOUT_LABEL = "Label";

        private static readonly Color RULE_COLOR = Color.green;
        private static readonly FoldoutColorMode RULE_COLOR_MODE = FoldoutColorMode.OnExpanded;
        private static readonly FoldoutColorSelector RULE_COLOR_SELECTOR = FoldoutColorSelector.Header;

        private static readonly Color LABEL_COLOR = Color.yellow;
        private static readonly FoldoutColorMode LABEL_COLOR_MODE = FoldoutColorMode.OnExpanded;

        public override void ProcessChildMemberAttributes(
            InspectorProperty parentProperty,
            MemberInfo member,
            List<Attribute> attributes)
        {
            attributes.Add(new PropertySpaceAttribute(2));
            attributes.Add(new LabelWidthAttribute(130f));

            switch (member.Name)
            {
                case PROP_GROUP_NAME:
                    ProcessRuleGroup(attributes);
                    break;

                case PROP_PATH:
                case PROP_MATCH_TYPE:
                    attributes.Add(new TitleGroupAttribute(TITLE_PATH));
                    break;

                case PROP_LABEL_MODE:
                    ProcessLabelGroup(attributes, "Mode");
                    break;

                case PROP_LABEL_REFS:
                    ProcessLabelGroup(attributes, "References");
                    break;

                case PROP_DYNAMIC_LABEL:
                    ProcessLabelGroup(attributes, "Dynamic");
                    break;

                case PROP_GROUP_TEMPLATE:
                    ProcessTitleGroup(attributes, TITLE_TEMPLATE, "Template");
                    break;

                case PROP_APPLICATION_MODE:
                    ProcessTitleGroup(attributes, TITLE_TEMPLATE, "Application Mode");
                    break;

                case PROP_SIMPLIFIED:
                    ProcessTitleGroup(attributes, TITLE_ADDRESS, "Simplified");
                    break;

                case PROP_ADDRESS_REPLACEMENT:
                    ProcessRuleGroup(attributes);
                    ProcessTitleGroup(attributes, TITLE_ADDRESS, "Replacement");
                    break;
            }
        }

        private static void ProcessRuleGroup(List<Attribute> attributes)
        {
            attributes.Add(new FoldoutColorGroupAttribute(RULE_FOLDOUT, RULE_FOLDOUT_LABEL) {
                Color = RULE_COLOR,
                ColorMode = RULE_COLOR_MODE,
                ColorSelector = RULE_COLOR_SELECTOR,
                PaddingTop = 5,
                PaddingBottom = 5,
                MarginTop = 2,
                MarginBottom = 5
            });
        }

        private static void ProcessTitleGroup(List<Attribute> attributes, string titleGroup, string label)
        {
            attributes.Add(new TitleGroupAttribute(titleGroup));
            attributes.Add(new LabelTextAttribute(label));
        }

        private static void ProcessLabelGroup(List<Attribute> attributes, string label)
        {
            attributes.Add(new FoldoutColorGroupAttribute(LABEL_FOLDOUT, LABEL_FOLDOUT_LABEL) {
                Color = LABEL_COLOR,
                ColorMode = LABEL_COLOR_MODE,
                BoldLabel = true,
                PaddingTop = 14,
                MarginTop = 2,
                MarginBottom = 3
            });

            attributes.Add(new LabelTextAttribute(label));
        }
    }
}

#endif
