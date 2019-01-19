using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface ISceneSingleton { }
public interface ISingletonCreateIfNotFound { }
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
    private static T s_instance;
    private static object s_lock = new object ();
    private static bool s_isApplicationQuiting;

    public static T instance {
        get {
            if (s_instance == null) {
                lock (s_lock) {
                    if (s_instance == null) {
                        s_instance = FindObjectOfType<T> ();
                    }
                    if (s_instance == null && typeof (ISingletonCreateIfNotFound).IsAssignableFrom (typeof (T)) && !s_isApplicationQuiting) {
                        GameObject singleton = new GameObject ();
                        s_instance = singleton.AddComponent<T> ();
                        singleton.name = "[Singleton] " + typeof (T).ToString ();
                        Debug.Log ("[Singleton] '" + singleton + "' created implicitly.");
                    }
                }
            }
            return s_instance;
        }
    }

    protected void Awake () {
        if (s_instance != null && s_instance != this) {
            Destroy (this);
            return;
        }
        s_instance = this as T;
        if (!(this is ISceneSingleton)) {
            DontDestroyOnLoad (this);
        }
        OnAwake ();
    }

    protected virtual void OnAwake () { }

    protected virtual void OnApplicationQuit () {
        s_isApplicationQuiting = true;
    }
}