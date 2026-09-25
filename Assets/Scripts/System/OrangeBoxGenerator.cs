using UnityEngine;

/// <summary>
/// This script is used to generate new instances from an object
/// </summary>
public class OrangeBoxGenerator : MonoBehaviour
{
    public GameObject prefab;
    public GameObject currentObject;
    public Quaternion rotation;

    public static OrangeBoxGenerator Instance;

    private void Awake()
    {
        Instance = this;
    }

    [ContextMenu("Generate New Box")]
    public void GenerateNewBox()
    {
        if (prefab == null)
        {
            Debug.LogError("There isn't an object attached to prefab");
            return;
        }

        // Checar
        //if (currentObject.GetComponent<SnapObject>().isSnapped)
        //{
        //currentObject.GetComponent<OrangeBox>().ShowComponents(true, false);
        //}


        // Si el generador tiene hijos, no crear otra caja.
        //if (transform.childCount > 0) return;

        currentObject = Instantiate(prefab, transform.position, rotation, transform);

    }


}