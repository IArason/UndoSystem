using UnityEngine;

public class UndoInstantiate : UndoAction {

    // Reference to the stored object
    GameObject instantiatedObject;
    bool undone = false;

    public UndoInstantiate(GameObject obj)
    {
        instantiatedObject = obj;
        UndoManager.Instance.AddAction(this);
    }

    public override void Redo()
    {
        instantiatedObject.SetActive(true);
        undone = false;
    }

    public override void Undo()
    {
        instantiatedObject.SetActive(false);
        undone = true;
    }

    public override void Cull()
    {
        if (undone)
        {
            Object.Destroy(instantiatedObject);
        }
    }
}
