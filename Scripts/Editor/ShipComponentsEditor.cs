using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SpaceWar
{
    [CustomEditor(typeof(ShipComponenets), true)]
    public class ShipComponentsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            ShipComponenets components = (ShipComponenets)target;

            if(GUILayout.Button("Get Components"))
            {
                Undo.RecordObject(components, "Get Components"); 
                components.GetComponents();
                EditorUtility.SetDirty(components);
            }
        }
    }
}
