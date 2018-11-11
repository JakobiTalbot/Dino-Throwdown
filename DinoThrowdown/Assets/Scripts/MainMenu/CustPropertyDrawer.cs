using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// used to display the 4x4 array as a grid in the inspector
[CustomPropertyDrawer(typeof(ArrayLayout))]
public class CustPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PrefixLabel(position, label);
        // stores the initial position
        Rect newPosition = position;
        // moves down a line in the inspector
        newPosition.y += 18.0f;
        // gets the rows from the class
        SerializedProperty data = property.FindPropertyRelative("rows");

        // iterates through each of the rows
        for (int i = 0; i < 4; i++)
        {
            // gets the object at the index in the row
            SerializedProperty row = data.GetArrayElementAtIndex(i).FindPropertyRelative("row");
            // sets the height of the object in the inspector
            newPosition.height = 18.0f;
            // sets the amount of columns to equal the amount of rows
            if (row.arraySize != 4)
            {
                row.arraySize = 4;
            }
            // sets the width of the object in the inspector
            newPosition.width = position.width / 4;

            // creates the fields in the row
            for (int j = 0; j < 4; j++)
            {
                EditorGUI.PropertyField(newPosition, row.GetArrayElementAtIndex(j), GUIContent.none);
                newPosition.x += newPosition.width;
            }

            // moves to the next line
            newPosition.x = position.x;
            newPosition.y += 18.0f;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // sets the height of the property
        return (18.0f * (4 + 1));
    }
}