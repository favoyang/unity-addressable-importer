using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AddressableImportSettings", menuName = "Addressable Assets/Import Settings", order = 50)]
public class AddressableImportSettings : ScriptableObject
{
    public const string kDefaultConfigObjectName = "addressableimportsettings";
    public const string kDefaultPath = "Assets/AddressableAssetsData/AddressableImportSettings.asset";

    public List<AddressableImportRule> rules;

    public static AddressableImportSettings Instance
    {
        get
        {
            AddressableImportSettings so;
            // Try to ocate settings via EditorBuildSettings.
            if (EditorBuildSettings.TryGetConfigObject(kDefaultConfigObjectName, out so))
                return so;
            // Try to locate settings via path.
            so = AssetDatabase.LoadAssetAtPath<AddressableImportSettings>(kDefaultPath);
            if (so == null)
            {
                // Create new settings.
                so = CreateInstance<AddressableImportSettings>();
                AssetDatabase.CreateAsset(so, kDefaultPath);
                AssetDatabase.SaveAssets();
                Debug.LogFormat("[AddressableImportSettings] Created default settings at {0}", kDefaultPath);
            }
            EditorBuildSettings.AddConfigObject(kDefaultConfigObjectName, so, true);
            return so;
        }
    }
}