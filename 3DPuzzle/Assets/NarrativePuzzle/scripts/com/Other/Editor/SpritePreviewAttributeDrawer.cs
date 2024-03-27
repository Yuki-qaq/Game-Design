// SearchableAttributeDrawer.cs >>> IN AN 'Editor' FOLDER <<<
using System;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(SpritePreviewAttribute))]
public class SpritePreviewAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var ident = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        EditorGUI.LabelField(position, label);
        var spriteRect = new Rect(position.x + 32, position.y + 8, 64, 64);

        GUI.changed = false;//! do this, else if you multi select items, the data will be overwritten!!!!
        var value = EditorGUI.ObjectField(spriteRect, property.objectReferenceValue, typeof(Sprite), false);
        if (GUI.changed)
            property.objectReferenceValue = value;

        EditorGUI.indentLevel = ident;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) + 72f;
    }
}