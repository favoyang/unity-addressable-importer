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
    public class IndentFieldAttribute : PropertyAttribute
    {
        public int Level { get; private set; }

        public IndentFieldAttribute(int level)
        {
            Level = Mathf.Max(level, 0);
        }
    }
}

#if UNITY_EDITOR && !ODIN_INSPECTOR
namespace UnityAddressableImporter.Helper.Internal
{
    [CustomPropertyDrawer(typeof(IndentFieldAttribute))]
    public class IndentFieldAttributeDrawer : PropertyDrawer
    {
        private IndentFieldAttribute Attribute
        {
            get { return _attribute ?? (_attribute = attribute as IndentFieldAttribute); }
        }

        private IndentFieldAttribute _attribute;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.indentLevel += Attribute.Level;
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.indentLevel -= Attribute.Level;
        }
    }
}
#endif