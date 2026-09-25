using System.Collections.Generic;
using UnityEngine;

public class AssemblySteps : MonoBehaviour
{
    [Header("Assembly Sequence")]
    [Tooltip("Enables the sequence in the assembly")]
    public bool isEnabled;
    [Tooltip("This list shows the currently sequence for the assembly production")]
   public List<SnapItemType> steps = new List<SnapItemType>();


   


}
