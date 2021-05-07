/// <summary>
/// LabelAttribute,
/// modified from https://github.com/dbrizov/NaughtyAttributes/blob/master/Assets/NaughtyAttributes/Scripts/Editor/PropertyDrawers/LabelPropertyDrawer.cs
/// </summary>
using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityAddressableImporter.Helper
{
    [AttributeUsage(AttributeTargets.Field)]
    public class LabelAttribute : PropertyAttribute
    {
        public string Label { get; private set; }

        public LabelAttribute(string label)
        {
            this.Label = label;
        }
    }
}

#if UNITY_EDITOR
namespace UnityAddressableImporter.Helper.Internal
{
    [CustomPropertyDrawer(typeof(LabelAttribute))]
    public class LabelAttributeDrawer : PropertyDrawer
    {
        private LabelAttribute Attribute
        {
            get { return _attribute ?? (_attribute = attribute as LabelAttribute); }
        }

        private LabelAttribute _attribute;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var guiContent = new GUIContent(Attribute.Label);
            EditorGUI.PropertyField(position, property, guiContent, true);
        }
    }
}
#endif