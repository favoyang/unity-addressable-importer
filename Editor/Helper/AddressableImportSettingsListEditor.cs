/// <summary>
/// AddressableImportSettingsListEditor
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

    [CustomEditor(typeof(AddressableImportSettingsList), true), CanEditMultipleObjects]
    public class AddressableImportSettingsListEditor : ScriptableObjectEditor<AddressableImportSettingsList>
    {

    }
}