/// <summary>
/// LabelAttribute,
/// modified from https://github.com/dbrizov/NaughtyAttributes/blob/master/Assets/NaughtyAttributes/Scripts/Editor/PropertyDrawers/LabelPropertyDrawer.cs
/// </summary>
using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityAddressableImporter.Helper
{
    [AttributeUsage(AttributeTargets.Field)]
    public class IndentLabelAttribute : PropertyAttribute
    {
        public string Label { get; private set; }

        public int Level { get; private set; }

        public IndentLabelAttribute(string label, int level)
        {
            Label = label;
            Level = Mathf.Max(level, 0);
        }
    }
}

#if UNITY_EDITOR && !ODIN_INSPECTOR
namespace UnityAddressableImporter.Helper.Internal
{
    [CustomPropertyDrawer(typeof(IndentLabelAttribute))]
    public class IndentLabelAttributeDrawer : PropertyDrawer
    {
        private IndentLabelAttribute Attribute
        {
            get { return _attribute ?? (_attribute = attribute as IndentLabelAttribute); }
        }

        private IndentLabelAttribute _attribute;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var guiContent = new GUIContent(Attribute.Label);
            EditorGUI.indentLevel += Attribute.Level;
            EditorGUI.PropertyField(position, property, guiContent, true);
            EditorGUI.indentLevel -= Attribute.Level;
        }
    }
}
#endif