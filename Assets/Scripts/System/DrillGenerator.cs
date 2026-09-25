using System;
using UnityEngine;

/// <summary>
/// This class enables the generation of new drills attached to the main support
/// </summary>
public class DrillGenerator : MonoBehaviour
{
    [SerializeField] private GameObject currentDrill;
    [SerializeField] private GameObject drillPrefab;
    // Singlenton
    public static DrillGenerator Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    [ContextMenu("Generate New Drill")]
    public void GenerateNewDrill()
    {
        currentDrill = null;
        currentDrill = Instantiate(drillPrefab, transform.position, transform.rotation, transform);
    }

}
