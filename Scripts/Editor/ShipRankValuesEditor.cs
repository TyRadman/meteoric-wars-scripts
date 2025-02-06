//using System.Collections;
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEngine;

//namespace SpaceWar
//{
//    [CustomEditor(typeof(ShipRankValues))]
//    public class ShipRankValuesEditor : Editor
//    {
//        private SerializedProperty _list;

//        // I know ... but that's your fault for naming th field List :P
//        private List<ShootingStyleValues> _listList;

//        // For Edito this is called when the object gains focus and the Inspector is loaded for this scriptableobject instance
//        private void OnEnable()
//        {
//            // Find and link the serialized field called "List"
//            _list = serializedObject.FindProperty(nameof(((ShipRankValues)target).NewStyleChances));

//            // Initialize and configure the ReorderableList to draw the referenced elements in the way we want
//            _listList = new ReorderableList(serializedObject, _list, true, true, true, true)
//            {
//                // How is te list header drawn?
//                drawHeaderCallback = rect => EditorGUI.LabelField(rect, _list.displayName),

//                // how should each element be drawn?
//                drawElementCallback = (rect, index, active, focused) =>
//                {
//                        // get the current element
//                        var element = _list.GetArrayElementAtIndex(index);

//                        // draw the default object reference field with the height of a single line without the label
//                        EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);

//                        // if no object is referenced do nothing more
//                        if (!element.objectReferenceValue) return;

//                        // move one line lower
//                        rect.y += EditorGUIUtility.singleLineHeight;

//                        // get a serializedObject of the reference
//                        var elementsSerializedObject = new SerializedObject(element.objectReferenceValue);

//                        // same as in our own OnInspectorGUI method below loads the current values
//                        elementsSerializedObject.Update();

//                        // for now assuming you only want the delay field
//                        var delay = elementsSerializedObject.FindProperty(nameof(ScriptableExpressionBuilder.delay));

//                        // draw the delay field
//                        EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), delay);

//                        // same as in our own OnInspectorGUI method below writes back changed values and handles undo/redo marking dirty etc
//                        elementsSerializedObject.ApplyModifiedProperties();
//                },

//                // how do we get the height of one element?
//                elementHeightCallback = index =>
//                {
//                        // get the current element
//                        var element = _list.GetArrayElementAtIndex(index);

//                        // if nothing is referenced we only need a single line
//                        if (!element.objectReferenceValue)
//                    {
//                        return EditorGUIUtility.singleLineHeight;
//                    }

//                        // otherwise we need two lines, one for the object field and one for the delay field
//                        return EditorGUIUtility.singleLineHeight * 2;
//                }
//            };
//        }

//        public override void OnInspectorGUI()
//        {
//            // draw the script field
//            DrawScriptField();

//            // loads current values into the serialized version
//            serializedObject.Update();

//            // draw the list according to the settings above
//            _listList.DoLayoutList();

//            // writes bac any changed values into the actual instance and handles undo/redo marking dirty etc
//            serializedObject.ApplyModifiedProperties();
//        }

//        // Draws the default script field on the top the Inspector
//        private void DrawScriptField()
//        {
//            // The script field is disabled since nobody is supposed to evr change it
//            EditorGUI.BeginDisabledGroup(true);
//            EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject((ScriptableExpressionCreator)target), typeof(ScriptableExpressionCreator), false);
//            EditorGUI.EndDisabledGroup();

//            // leave a little space between the script field and the actual inspector conten
//            EditorGUILayout.Space();
//        }
//    }
//}
