using UnityEngine;
using System.Collections;
using UnityEditor;

/// <summary>
/// Class06
/// </summary>
[CustomPropertyDrawer(typeof(MyTestAttribute))]
public class MyTestDrawer : PropertyDrawer
{

    public override void OnGUI(UnityEngine.Rect position, SerializedProperty property, UnityEngine.GUIContent label)
    {
        MyTestAttribute attribute = (MyTestAttribute)base.attribute;

        property.intValue = Mathf.Min(Mathf.Max(EditorGUI.IntField(position, label.text, property.intValue), attribute.min), attribute.max);
    }
}