using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer (typeof (Enum), true)]
public class EnumFlagsDrawer : PropertyDrawer {
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
		EditorGUI.BeginProperty (position, label, property);

		Type type = fieldInfo.FieldType;
		if (type.GetCustomAttribute<System.FlagsAttribute> () == null) {
			EditorGUI.PropertyField (position, property, label);
		} else {
			Enum enumValue = (Enum) Enum.ToObject (fieldInfo.FieldType, property.intValue);
			enumValue = EditorGUI.EnumFlagsField (position, label, enumValue);
			property.intValue = (int) Convert.ToInt32 (enumValue);
		}

		EditorGUI.EndProperty ();
	}
}