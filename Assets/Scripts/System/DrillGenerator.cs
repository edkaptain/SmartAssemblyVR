using UnityEngine;

public class DrillGenerator : MonoBehaviour
{
    [SerializeField] private GameObject currentDrill;
    [SerializeField] private GameObject drillPefab;

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

    public void DetachCurrentDrill()
    {
        currentDrill = null;
        currentDrill = InstantiateNewDrill();
    }

    private GameObject InstantiateNewDrill()
    {
        return Instantiate(drillPefab, transform.position, transform.rotation, transform);
    }
}
