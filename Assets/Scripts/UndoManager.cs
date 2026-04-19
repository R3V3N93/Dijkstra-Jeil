using System.Collections.Generic;
using UnityEngine;

public class UndoManager : MonoBehaviour
{
    Queue<Operation> undoBuffer;
}

public abstract class Operation
{
    public void Undo()
    {
        
    }
}