using UnityEngine;

public class UndoTranslate : UndoAction
{
    Transform target;
    Vector3 startPos;
    Vector3 endPos;

    public UndoTranslate(Transform target, Vector3 startPos, Vector3 endPos)
    {
        this.target = target;
        this.startPos = startPos;
        this.endPos = endPos;

        UndoManager.Instance.AddAction(this);
    }

    public override void Redo()
    {
        target.transform.position = endPos;
    }

    public override void Undo()
    {
        target.transform.position = startPos;
    }
}
