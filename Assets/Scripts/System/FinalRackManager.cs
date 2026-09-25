using UnityEngine;

public class FinalRackManager : MonoBehaviour
{
    [SerializeField] private int quantity = 1;
    [SerializeField] private GameObject prefab;
    [SerializeField] private Vector3 offset = new Vector3();

    [ContextMenu("Generate instances")]
    public void GenerateRacks()
    {
        // Destroy each objects
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        // Instantiate objects
        for(int i = 0; i < quantity; i++)
        {
            GameObject instance = Instantiate(prefab, transform);

            instance.transform.localPosition = offset * i;
            instance.transform.localRotation = Quaternion.identity;
        }
    }
}
