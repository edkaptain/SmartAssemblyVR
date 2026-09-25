using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gestiona una colección de objetos hijos dentro de un contenedor (parentObj),
/// reemplazando algunos de ellos por versiones "defectuosas" según una
/// probabilidad configurable (defectChance).
///
/// Flujo general:
/// 1. FillListFromParent(): recorre los hijos de parentObj, normaliza los que
///    ya venían marcados como defectuosos (los reemplaza por el prefab normal)
///    y construye la lista interna listObjects.
/// 2. DefectProbability(): recorre listObjects y, según la probabilidad
///    generada por GenerateNormalProbability(), reemplaza algunos objetos
///    por una instancia aleatoria de defectiveObjects.
/// </summary>
public class Bins : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private GameObject objects; // Prefab normal
    [SerializeField] private List<GameObject> defectiveObjects; // Prefab defectuoso
    [SerializeField] private GameObject parentObj;
    [SerializeField] private List<GameObject> listObjects = new List<GameObject>();

    [Header("Probability")]
    [SerializeField, UnityEngine.Range(0f, 1f)] private float defectChance = 0.25f;

    [Header("Interactable")]
    [SerializeField] private GameObject interactable;
    private Rigidbody rb;
    private bool isActivated = false;

    /// <summary>
    /// Se ejecuta automáticamente al agregar el componente o al presionar
    /// "Reset" en el inspector. Llena la lista a partir del padre.
    /// </summary>
    private void Reset()
    {
        FillListFromParent();
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        FillListFromParent();
    }
    private void Update()
    {
        if(isActivated) return;

        if (parentObj.transform.childCount == 0)
        {
            ActivateObject();
            enabled = false;
        }
    }

    private void ActivateObject()
    {
        isActivated = true;
        interactable.SetActive (true);
        rb.isKinematic = false;
        rb.useGravity = true;
    }

    /// <summary>
    /// Recorre los hijos de parentObj, normaliza los que estén marcados como
    /// defectuosos (reemplazándolos por una instancia del prefab normal) y
    /// llena listObjects con el resultado. Al finalizar, aplica
    /// DefectProbability() sobre la lista resultante.
    /// </summary>
    [ContextMenu("Fill List From Parent")]
    private void FillListFromParent()
    {
        listObjects.Clear();

        if (parentObj == null) return;

        // FIX: Antes se usaba "parentObj.GetComponentInChildren<Transform>()"
        // dentro del foreach, lo cual es redundante (GetComponentInChildren
        // sobre el propio objeto regresa su propio Transform) y, peor aún,
        // se modificaba la jerarquía (Destroy/Instantiate) MIENTRAS se
        // iteraba sobre ella. Esto puede saltarse hijos o iterar sobre
        // objetos ya destruidos.
        //
        // Solución: copiamos los hijos a una lista aparte ANTES de tocar
        // la jerarquía, y luego iteramos sobre esa copia de forma segura.
        List<Transform> children = new List<Transform>();
        foreach (Transform child in parentObj.transform)
        {
            children.Add(child);
        }

        foreach (Transform child in children)
        {
            SnapObject snapObject = child.gameObject.GetComponent<SnapObject>();

            if (snapObject != null && snapObject.isDefective)
            {
                Vector3 position = child.position;
                Quaternion rotation = child.rotation;
                Transform parent = child.parent;

                DestroyObject(child.gameObject);

                GameObject newObj = Instantiate(
                    objects,
                    position,
                    rotation,
                    parent
                );

                listObjects.Add(newObj);
            }
            else
            {
                listObjects.Add(child.gameObject);
            }
        }

        DefectProbability();
    }

    /// <summary>
    /// Recorre listObjects y, para cada elemento, genera un valor de
    /// probabilidad mediante GenerateNormalProbability(). Si defectChance
    /// es menor o igual a ese valor, el objeto es reemplazado por una
    /// instancia aleatoria de defectiveObjects.
    /// </summary>
    private void DefectProbability()
    {
        // FIX: Si defectiveObjects está vacía, Random.Range(0, 0) devuelve 0
        // y defectiveObjects[0] lanzaría un IndexOutOfRangeException.
        // Validamos antes de entrar al bucle.
        if (defectiveObjects == null || defectiveObjects.Count == 0)
        {
            Debug.LogWarning("[Bins] defectiveObjects está vacía. No se pueden generar objetos defectuosos.");
            return;
        }

        for (int i = 0; i < listObjects.Count; i++)
        {
            GameObject obj = listObjects[i];

            // FIX: Si por alguna razón el elemento ya es null
            // (por ejemplo, fue destruido externamente), lo saltamos
            // para evitar referencias nulas más abajo.
            if (obj == null) continue;

            bool isDefective;
            if(defectChance != 0f){

                isDefective = Random.value <= defectChance;
            }
            else
            {
                isDefective = false;
            }


            Debug.LogWarning("Is defective: " + isDefective);

            if (isDefective == true)
            {
                int randomIndex;

                randomIndex = Random.Range(0, defectiveObjects.Count);

                Vector3 position = obj.transform.position;
                Quaternion rotation = obj.transform.rotation;
                Transform parent = obj.transform.parent;

                DestroyObject(obj);

                GameObject newDefectiveObject = Instantiate(
                    defectiveObjects[randomIndex],
                    position,
                    rotation,
                    parent
                );

                listObjects[i] = newDefectiveObject;
            }
        }
    }


    /// <summary>
    /// Destruye un GameObject de forma segura tanto en Play mode
    /// (Destroy) como en el Editor (DestroyImmediate).
    /// </summary>
    private void DestroyObject(GameObject obj)
    {
        if (obj == null) return;

        if (Application.isPlaying)
            Destroy(obj);
        else
            DestroyImmediate(obj);
    }
    [ContextMenu("Reset list")]
    public void ResetList()
    {
        listObjects.Clear();

        if (parentObj == null) return;

        List<Transform> children = new List<Transform>();
        foreach (Transform child in parentObj.transform)
        {
            children.Add(child);
        }

        foreach (Transform child in children)
        {
            SnapObject snapObject = child.gameObject.GetComponent<SnapObject>();

            if (snapObject != null)
            {
                Vector3 position = child.position;
                Quaternion rotation = child.rotation;
                Transform parent = child.parent;

                DestroyObject(child.gameObject);

                GameObject newObj = Instantiate(
                    objects,
                    position,
                    rotation,
                    parent
                );

                listObjects.Add(newObj);
            }
        }
    }


    // Testing, to move bins by hand

}