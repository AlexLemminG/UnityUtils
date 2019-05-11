using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class PrefabsCreator : MonoBehaviour {
    const string className = "Prefabs";
    const string rootFolder = "Assets/Prefabs/Global";
    const string prefabsAssetFile = "Assets/Resources/Prefabs.asset";
    [UnityEditor.MenuItem ("Tools/UpdatePrefabs")]
    static void Create () {
        const string ss =
            @"using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class __TYPE__ : ScriptableObject {
    static __TYPE__ s_instanceCache;
    static __TYPE__ s_instance {
        get {
            if (!s_instanceCache) {
                s_instanceCache = Resources.Load<__TYPE__> (""Prefabs"");
            }
            return s_instanceCache;
        }
    }
__ROOT__
}";
        var dict = CollectPrefabs ();
        string listString = "";
        string[] assetGUIDS = AssetDatabase.FindAssets ("t:prefab", new string[] { rootFolder });
        string[] assetPaths = new string[assetGUIDS.Length];
        for (int i = 0; i < assetGUIDS.Length; i++) {
            assetPaths[i] = AssetDatabase.GUIDToAssetPath (assetGUIDS[i]);
        }
        foreach (var pathObj in dict) {
            if (pathObj.Value) {
                string prettyName = pathObj.Key;
                listString += "\n\n";
                listString += @"    [SerializeField] private GameObject m_" + prettyName + " = null ;\n";
                listString += @"    public static GameObject " + prettyName + "{get{return s_instance.m_" + prettyName + ";}}";
            }
        }
        string scriptFilePath = System.IO.Path.Combine (Application.dataPath, "Scripts/" + className + ".cs");
        string scriptFile = ss.Replace ("__ROOT__", listString);
        scriptFile = scriptFile.Replace ("__TYPE__", className);

        bool fileChanged = true;
        if (System.IO.File.Exists (scriptFilePath)) {
            fileChanged = System.IO.File.ReadAllText (scriptFilePath) != scriptFile;
        }
        if (fileChanged) {
            System.IO.File.WriteAllText (scriptFilePath, scriptFile);

            AssetDatabase.DeleteAsset (prefabsAssetFile);

            AssetDatabase.Refresh ();
        } else {
            CreateAssetIfNeeded ();
        }
    }

    static Dictionary<string, GameObject> CollectPrefabs () {
        var dict = new Dictionary<string, GameObject> ();
        string[] assetGUIDS = AssetDatabase.FindAssets ("t:prefab", new string[] { rootFolder });
        string[] assetPaths = new string[assetGUIDS.Length];
        for (int i = 0; i < assetGUIDS.Length; i++) {
            assetPaths[i] = AssetDatabase.GUIDToAssetPath (assetGUIDS[i]);
        }
        foreach (var path in assetPaths) {
            var asset = AssetDatabase.LoadAssetAtPath<GameObject> (path);
            if (asset) {
                string prettyName = path;
                prettyName = prettyName.Replace (".prefab", "");
                prettyName = prettyName.Remove (0, rootFolder.Length + 1);
                prettyName = prettyName.Replace (System.IO.Path.DirectorySeparatorChar, '_');
                prettyName = prettyName.Replace (' ', '_');
                dict[prettyName] = asset;
            }
        }
        return dict;
    }

    [InitializeOnLoadMethod]
    static void CreateAssetIfNeeded () {
        bool typeExists = System.Type.GetType(className, false) != null;
        if(!typeExists){
            return;
        }
        if (AssetDatabase.LoadAssetAtPath<Object> (prefabsAssetFile) != null) {
            return;
        }

        var dict = CollectPrefabs ();
        var prefabsAsset = ScriptableObject.CreateInstance (className);
        if (prefabsAsset == null) {
            Debug.LogError ("Could not create Prefabs asset");
            return;
        }

        foreach (var kv in dict) {
            prefabsAsset.GetType ().GetField ("m_" + kv.Key, BindingFlags.NonPublic | BindingFlags.Instance).SetValue (prefabsAsset, kv.Value);
        }
        dict.Clear ();

        AssetDatabase.CreateAsset (prefabsAsset, prefabsAssetFile);

        AssetDatabase.Refresh ();
    }

}