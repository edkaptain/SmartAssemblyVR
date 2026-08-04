using UnityEngine;
using static Oculus.Interaction.Context;

public class FinishedProducts : MonoBehaviour
{
    [SerializeField] private GameObject defaultPrefab;
    [SerializeField] private int rows = 3;
    [SerializeField] private int columns = 3;
    [SerializeField] private int layers = 2;

    

    public int TotalPieces => rows * columns * layers;

    public static FinishedProducts Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    [SerializeField] private float gap = 0.05f;

    [ContextMenu("Generate Boxes")]
    private void GenerateBoxes()
    {
        ClearBoxes();

        if (defaultPrefab == null) return;

        Vector3 boxSize = GetPrefabBoundsSize(defaultPrefab);

        for (int y = 0; y < layers; y++)
        {
            for (int z = 0; z < rows; z++)
            {
                for (int x = 0; x < columns; x++)
                {
                    Vector3 position = transform.position + new Vector3(
                        x * (boxSize.x + gap),
                        y * (boxSize.y + gap),
                        z * (boxSize.z + gap)
                    );

                    Instantiate(defaultPrefab, position, transform.rotation, transform);
                }
            }
        }
    }

    private void Reset()
    {
        GenerateBoxes();
    }

    private void ClearBoxes()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    private Vector3 GetPrefabBoundsSize(GameObject prefab)
    {
        GameObject temp = Instantiate(prefab, Vector3.zero, Quaternion.identity);

        Renderer[] renderers = temp.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
        {
            DestroyImmediate(temp);
            return Vector3.one;
        }

        Bounds bounds = renderers[0].bounds;

        foreach (Renderer renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }

        Vector3 size = bounds.size;

        DestroyImmediate(temp);

        return size;
    }
}