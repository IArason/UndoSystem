using UnityEngine;
using System.Collections;

public abstract class UndoAction
{
    public abstract void Undo();
    public abstract void Redo();

    /* * *
    * Implement this if something special needs to be done before
    * the object is removed from the history, such as it needing
    * to be properly removed if it is only hidden when it is deleted
    * and placed in the history.
    * * */
    public virtual void Cull() { }
}
