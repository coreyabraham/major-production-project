using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomEditor(typeof(PlayerSystem))]
public class PlayerSystem_Inspector : Editor
{
    //public VisualTreeAsset m_InspectorXML;
    //public override VisualElement CreateInspectorGUI()
    //{
    //    // Create a new VisualElement to be the root of the Inspector UI.
    //    VisualElement myInspector = new VisualElement();

    //    // Load from default reference.
    //    m_InspectorXML.CloneTree(myInspector);

    //    // Get a reference to the default Inspector Foldout control.
    //    VisualElement InspectorFoldout = myInspector.Q("Default_Inspector");

    //    // Attach a default Inspector to the Foldout.
    //    InspectorElement.FillDefaultInspector(InspectorFoldout, serializedObject, this);

    //    // Return the finished Inspector UI.
    //    return myInspector;
    //}
}
