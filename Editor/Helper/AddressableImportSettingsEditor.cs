/// <summary>
/// AddressableImportSettingsEditor
/// </summary>
namespace UnityAddressableImporter.Helper
{
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEditor;

#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
#endif

    [CustomEditor(typeof(AddressableImportSettings), true), CanEditMultipleObjects]
    public class AddressableImportSettingsEditor : ScriptableObjectEditor<AddressableImportSettings>
    {

    }
}