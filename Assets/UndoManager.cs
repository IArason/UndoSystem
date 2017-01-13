using System.Collections.Generic;
using UnityEngine;

public class UndoManager : MonoBehaviour
{
    private static UndoManager _instance;
    public static UndoManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UndoManager>();

                if (_instance == null)
                {
                    GameObject singleton = new GameObject();
                    _instance = singleton.AddComponent<UndoManager>();
                    singleton.name = "UndoManager";
                }
            }
            return _instance;
        }
    }

    [SerializeField]
    int maxHistoryLength = 5;

    public List<UndoAction> history = new List<UndoAction>();
    public int currentIndex = -1;

    public void Undo()
    {
        // -1 is our "parked" index, meaning we are at the
        // end of the history, so we can't undo.
        if (currentIndex < 0) return;

        // Undo the action and move down the history list
        history[currentIndex].Undo();
        currentIndex--;
    }

    public void Redo()
    {
        if (currentIndex >= history.Count - 1) return;

        // Move up the history list and redo the action.
        currentIndex++;
        history[currentIndex].Redo();
    }


    public void AddAction(UndoAction action)
    {
        // If we have undone part way back into the history, we
        // want to erase everything we have undone, creating
        // a new history thread starting at our current point.
        if (currentIndex != history.Count - 1)
        {
            Cull(currentIndex + 1, history.Count - currentIndex - 1);
        }

        history.Add(action);
        currentIndex = history.Count - 1;
        Trim();
    }

    // Removes the oldest object in the history if the history
    // has reached its maximum length.
    // Called every time history is modified
    void Trim()
    {
        if (history.Count > maxHistoryLength)
        {
            Cull(0, 1);
            currentIndex--;
        }
    }

    // Culls a given range of the history
    void Cull(int start, int count)
    {
        for (int i = start; i < start + count; i++)
        {
            history[i].Cull();
        }
        history.RemoveRange(start, count);
    }
}