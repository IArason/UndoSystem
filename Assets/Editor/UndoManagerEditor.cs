using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UndoManager))]
public class UndoManagerEditor : Editor {
    
    UndoManager manager;
    bool toggle = false;

    void OnEnable()
    {
        manager = (UndoManager)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        toggle = EditorGUILayout.Foldout(toggle, "History View");
        if (toggle)
        {
            for (int i = 0; i < manager.history.Count; i++)
            {
                var label = i + ": " + manager.history[i].GetType().ToString();
                if (i == manager.currentIndex)
                    label += "  ◀";
                EditorGUILayout.LabelField(label);
            }
        }
    }
}
