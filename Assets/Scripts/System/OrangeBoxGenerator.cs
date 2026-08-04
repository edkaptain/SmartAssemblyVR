using UnityEngine;

public class OrangeBoxGenerator : MonoBehaviour
{
    public GameObject prefab;

    [ContextMenu("Generate New Box")]
    public void GenerateNewBox()
    {
        if(transform.childCount >= 1)
        {
            Debug.LogWarning("There is already a box placed");
        }
        else
        {
            Instantiate(prefab, transform.position, transform.rotation, transform);
        }
    }
}
