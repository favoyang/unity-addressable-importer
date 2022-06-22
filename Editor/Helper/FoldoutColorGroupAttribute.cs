/// https://github.com/onewheelstudio/SirenixTutorialFiles/blob/master/CustomGroups/MyClass.cs

namespace UnityAddressableImporter.Helper
{
    using UnityEngine;

    public enum FoldoutColorMode
    {
        Always,
        OnExpanded,
        OnCollapsed
    }

    public enum FoldoutColorSelector
    {
        All,
        Header
    }

    public partial class FoldoutColorGroupAttribute
    {
        public Color Color { get; set; }

        public FoldoutColorMode ColorMode { get; set; }

        public FoldoutColorSelector ColorSelector { get; set; }

        public bool Expanded { get; set; }

        public bool BoldLabel { get; set; }

        public int PaddingTop { get; set; }

        public int PaddingBottom { get; set; }

        public int MarginTop { get; set; }

        public int MarginBottom { get; set; }
    }
}

#if !ODIN_INSPECTOR

namespace UnityAddressableImporter.Helper
{
    using System;
    using UnityEngine;

    partial class FoldoutColorGroupAttribute : Attribute
    {
        public string Group { get; set; }

        public string LabelText { get; set; }

        public bool ShowLabel { get; set; }

        public float Order { get; set; }

        public FoldoutColorGroupAttribute(
            string group,
            string label = "",
            float r = 1f, float g = 1f, float b = 1f,
            bool expanded = false,
            bool showLabel = true,
            bool boldLabel = false,
            float order = 0f)
        {
            Group = group;
            Color = new Color(r, g, b, 1f);
            LabelText = label;
            Expanded = expanded;
            ShowLabel = showLabel;
            Order = order;
            BoldLabel = boldLabel;
        }
    }
}

#endif

#if ODIN_INSPECTOR

namespace UnityAddressableImporter.Helper
{
    using System;
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.OdinInspector.Editor.ValueResolvers;
    using Sirenix.Utilities;
    using Sirenix.Utilities.Editor;
    using UnityEditor;
    using UnityEngine;

    partial class FoldoutColorGroupAttribute : BoxGroupAttribute
    {
        public FoldoutColorGroupAttribute(
            string group,
            string label = "",
            float r = 1f, float g = 1f, float b = 1f,
            bool expanded = false,
            bool showLabel = true,
            bool boldLabel = false,
            float order = 0f)
            : base(group, showLabel, false, order)
        {
            Color = new Color(r, g, b, 1f);
            LabelText = label;
            Expanded = expanded;
            BoldLabel = boldLabel;
        }

        protected override void CombineValuesWith(PropertyGroupAttribute other)
        {
            var otherAttr = (FoldoutColorGroupAttribute)other;
            var otherColor = otherAttr.Color;
            var color = Color;

            color.r = Math.Max(otherColor.r, Color.r);
            color.g = Math.Max(otherColor.g, Color.g);
            color.b = Math.Max(otherColor.b, Color.b);
            color.a = Math.Max(otherColor.a, Color.a);

            Color = color;
        }
    }

    public class FoldoutColorGroupAttributeDrawer : OdinGroupDrawer<FoldoutColorGroupAttribute>
    {
#if ODIN_INSPECTOR_3
        private ValueResolver<string> _groupNameResolver;
#endif

        private static readonly GUIStyle FOLDOUT_STYLE_BOLD;

        static FoldoutColorGroupAttributeDrawer()
        {
            FOLDOUT_STYLE_BOLD = new GUIStyle(SirenixGUIStyles.Foldout);
            FOLDOUT_STYLE_BOLD.fontStyle = FontStyle.Bold;
        }

        private LocalPersistentContext<bool> _isExpanded;

        protected override void Initialize()
        {
#if ODIN_INSPECTOR_3
            _groupNameResolver = ValueResolver.GetForString(Property, Attribute.LabelText ?? Attribute.GroupName);
#endif

            _isExpanded = this.GetPersistentValue(
                $"{nameof(FoldoutColorGroupAttributeDrawer)}.isExpanded",
                Attribute.Expanded);
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            var headerLabel = Attribute.LabelText;

            if (Attribute.ShowLabel)
            {
#if ODIN_INSPECTOR_3
                _groupNameResolver.DrawError();
                headerLabel = _groupNameResolver.GetValue();
#endif
            }
            else
            {
                headerLabel = string.Empty;
            }

            var colored = true;

            switch (Attribute.ColorMode)
            {
                case FoldoutColorMode.OnExpanded:
                    colored = _isExpanded.Value;
                    break;

                case FoldoutColorMode.OnCollapsed:
                    colored = !_isExpanded.Value;
                    break;
            }

            var coloredAll = Attribute.ColorSelector == FoldoutColorSelector.All;

            GUILayout.Space(Attribute.PaddingTop);

            if (colored && coloredAll)
                GUIHelper.PushColor(Attribute.Color);

            SirenixEditorGUI.BeginBox();

            BeginBoxHeader(colored && !coloredAll, Attribute.Color);

            if (colored && coloredAll)
                GUIHelper.PopColor();


            var foldoutStyle = Attribute.BoldLabel ? FOLDOUT_STYLE_BOLD : SirenixGUIStyles.Foldout;
            _isExpanded.Value = SirenixEditorGUI.Foldout(_isExpanded.Value, headerLabel, foldoutStyle);

            SirenixEditorGUI.EndBoxHeader();

            if (_isExpanded.Value)
                GUILayout.Space(Attribute.MarginTop);

            if (SirenixEditorGUI.BeginFadeGroup(this, _isExpanded.Value))
            {
                for (int i = 0; i < Property.Children.Count; i++)
                {
                    Property.Children[i].Draw();
                }
            }

            SirenixEditorGUI.EndFadeGroup();

            if (_isExpanded.Value)
                GUILayout.Space(Attribute.MarginBottom);

            SirenixEditorGUI.EndBox();
            GUILayout.Space(Attribute.PaddingBottom);
        }

        private static Rect BeginBoxHeader(bool colored, Color color)
        {
            GUILayout.Space(-3f);
            Rect rect = EditorGUILayout.BeginHorizontal(SirenixGUIStyles.BoxHeaderStyle, GUILayoutOptions.ExpandWidth());

            if (Event.current.type == EventType.Repaint)
            {
                rect.x -= 3f;
                rect.width += 6f;

                color = colored ? color : SirenixGUIStyles.HeaderBoxBackgroundColor;
                color.a = SirenixGUIStyles.HeaderBoxBackgroundColor.a;

                GUIHelper.PushColor(color);
                GUI.DrawTexture(rect, Texture2D.whiteTexture);
                GUIHelper.PopColor();
                SirenixEditorGUI.DrawBorders(rect, 0, 0, 0, 1, new Color(0f, 0f, 0f, 0.4f));
            }

            GUIHelper.PushLabelWidth(GUIHelper.BetterLabelWidth - 4f);
            return rect;
        }
    }
}

#endif
