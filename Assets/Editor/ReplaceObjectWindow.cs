using UnityEditor;
using UnityEngine;

public class ReplaceSel : EditorWindow
{
    GameObject myObject;
    [MenuItem("Tools/ReplaceSelected %g")]
    public static void ReplaceObjects()
    {
        GetWindow(typeof(ReplaceSel));
    }

    void OnGUI()
    {
        GUILayout.Label("Use Object", EditorStyles.boldLabel);

        myObject = EditorGUILayout.ObjectField(myObject, typeof(GameObject), true) as GameObject;

        if (!GUILayout.Button("Replace Selected")) return;
        if (myObject == null) return;

        int undoIndex = Undo.GetCurrentGroup();
        foreach (Transform t in Selection.transforms)
        {
            GameObject o;
            o = PrefabUtility.GetPrefabParent(myObject) as GameObject;

            if (PrefabUtility.GetPrefabType(myObject).ToString() == "PrefabInstance")
            {
                o = (GameObject)PrefabUtility.InstantiatePrefab(o);
                PrefabUtility.SetPropertyModifications(o, PrefabUtility.GetPropertyModifications(myObject));
            }
            else if (PrefabUtility.GetPrefabType(myObject).ToString() == "Prefab")
            {
                o = (GameObject)PrefabUtility.InstantiatePrefab(myObject);
            }
            else
            {
                o = Instantiate(myObject);
            }

            Undo.RegisterCreatedObjectUndo(o, "created prefab");

            Transform newT = o.transform;

            if (t == null) continue;
            newT.position = t.position;
            newT.rotation = t.rotation;
            newT.localScale = t.localScale;
            newT.parent = t.parent;
        }

        foreach (GameObject go in Selection.gameObjects)
        {
            Undo.DestroyObjectImmediate(go);
        }
        Undo.CollapseUndoOperations(undoIndex);
    }
}
