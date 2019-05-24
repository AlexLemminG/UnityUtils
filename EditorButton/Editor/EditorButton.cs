// Initial Concept by http://www.reddit.com/user/zaikman
// Revised by http://www.reddit.com/user/quarkism

using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (UnityEngine.Object), true)]
[CanEditMultipleObjects]
public class EditorButton : Editor {
	public override void OnInspectorGUI () {
		base.OnInspectorGUI ();

		var mono = target as UnityEngine.Object;

		if (mono == null)
			return;

		var methods = mono.GetType ()
			.GetMembers (BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
			.Where (o => Attribute.IsDefined (o, typeof (EditorButtonAttribute)));

		bool guiEnabled = GUI.enabled;
		foreach (var memberInfo in methods) {
			var editorButtonAttribute = memberInfo.GetCustomAttribute<EditorButtonAttribute> ();
			GUI.enabled = editorButtonAttribute.enabledInEditMode || Application.isPlaying;
			if (GUILayout.Button (memberInfo.Name)) {
				var method = memberInfo as MethodInfo;
				foreach (var target in targets) {
					method.Invoke (target, null);
				}
			}
		}
		GUI.enabled = guiEnabled;
	}
}