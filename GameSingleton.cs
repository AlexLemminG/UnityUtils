using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSingleton<T> : MonoBehaviour where T : MonoBehaviour {
    private static T s_instance;
    private static object s_lock = new object ();

    public static T instance {
        get {
            if (s_instance == null) {
                lock (s_lock) {
                    if (s_instance == null) {
                        s_instance = FindObjectOfType<T> ();

                        if (s_instance == null) {
                            GameObject singleton = new GameObject ();
                            s_instance = singleton.AddComponent<T> ();
                            singleton.name = "(singleton) " + typeof (T).ToString ();

                            DontDestroyOnLoad (singleton);
                            Debug.Log ("[Singleton] '" + singleton + "' created implicitly.");
                        }
                    }
                }
            }
            return s_instance;
        }
    }
}