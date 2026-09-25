using Oculus.Interaction;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DrillObject : MonoBehaviour
{
    #region ===== Inspector References =====
    public bool isComplete;
    [SerializeField] private List<GameObject> drillComponents;
    [SerializeField] private List<GameObject> drillDefects;
    [SerializeField] private GameObject interactable;

    [Header("Testing")]
    public GameObject cables;

    public bool isSelected;

    public bool cableConnection;
    private bool gravityActivated;

    public List<GameObject> ghostlist = new List<GameObject>();

    #endregion

    #region ===== Unity Lifecycle =====

    private void Start()
    {
        drillComponents = new List<GameObject>();
        try
        {
            interactable.SetActive(false);

        }
        catch (System.Exception)
        {
            Debug.LogWarning("Intearctable was not found");
        }


    }

    private void Update()
    {
        //if (isSelected && gravityActivated == false) {
        //    UpdateGravity(true);
        //};

    }

    #endregion

    #region ===== Functions =====
    /// <summary>
    /// This function adds the attached drill component into a list to manage the information when it was attached correctly
    /// </summary>
    /// <param name="component"></param>
    public void AddToDrill(GameObject component)
    {
        if (drillComponents.Contains(component) == false)
        {
            drillComponents.Add(component);
        }

        if (component.GetComponent<SnapObject>().isDefective == true)
        {
            drillDefects.Add(component);
        }

        RenderCables();
        GrabCompleteDrill();
    }

    public void RemoveFromDrill(GameObject component)
    {
        if (drillComponents.Contains(component))
        {
            drillComponents.Remove(component);
        }
        RenderCables();

    }
    bool HasPart(params SnapItemType[] types)
    {
        return types.All(type => drillComponents.Any(obj =>
        {
            var part = obj.GetComponent<SnapObject>();
            return part != null && part.objectType == type;
        }));
    }

    private void RenderCables()
    {
        cableConnection = HasPart(SnapItemType.ElectricMotor, SnapItemType.Battery, SnapItemType.Trigger, SnapItemType.RearBody);
        cables.SetActive(cableConnection);
    }
    #endregion

    #region ===== Logic =====

    public void GrabCompleteDrill()
    {
        if (drillComponents == null)
        {
            Debug.LogWarning("drillComponents is null.");
            return;
        }

        if (drillComponents.Count != 8)
        {
            Debug.Log("The drill is not complete yet.");
            Debug.Log($"Current attached components: {drillComponents.Count}");
            return;
        }

        Debug.Log("The drill is complete");

        foreach (GameObject ghost in ghostlist)
        {
            if (ghost == null) continue;

            foreach (Collider col in ghost.GetComponentsInChildren<Collider>(true))
            {
                col.enabled = false;
            }
        }

        //foreach (GameObject child in drillComponents)
        //{
        //    if (child == null)
        //    {
        //        Debug.LogWarning("One component in drillComponents is null");
        //        continue;
        //    }

        //    SnapObject snapObj = child.GetComponent<SnapObject>();
        //    //snapObj.UpdateGravity(false);

        //    child.transform.localPosition = new Vector3(0, 0, 0);
        //    child.transform.localRotation = Quaternion.identity;


        //    // TESTING
        //    //snapObj.GetComponent<Collider>().isTrigger = true;

        //    if (child == null) continue;

        //    // Conserva los Rigidbody, pero deja las piezas sin física independiente.
        //    foreach (Rigidbody rb in child.GetComponentsInChildren<Rigidbody>(true))
        //    {
        //        if (!rb.isKinematic)
        //        {
        //            rb.linearVelocity = Vector3.zero;
        //            rb.angularVelocity = Vector3.zero;
        //        }

        //        rb.useGravity = false;
        //        rb.isKinematic = true;
        //    }

        //    // El collider del padre representará al taladro completo.
        //    foreach (Collider col in child.GetComponentsInChildren<Collider>(true))
        //    {
        //        col.enabled = false;
        //    }

        //    // Gives an error when its activated as explotes
        //    //testing
        //    //snapObj.enabled = false;
        //    if (child.GetComponent<Collider>())
        //    {
        //        child.GetComponent<Collider>().enabled = false;
        //    }

        //    if (child.transform.childCount > 0)
        //    {
        //        Transform firstChild = child.transform.GetChild(0);
        //        firstChild.gameObject.SetActive(false);
        //    }
        //    else
        //    {
        //        Debug.LogWarning($"{child.name} does not have a first child.");
        //    }
        //}

      

        for (int i = 0; i < drillComponents.Count; i++)
        {
            GameObject child = drillComponents[i];

            if (child == null)
            {
                Debug.LogWarning($"Component at index {i} is null.");
                continue;
            }

            // Desactiva la física independiente de cada pieza.
            Rigidbody[] rigidbodies = child.GetComponentsInChildren<Rigidbody>(true);

            for (int j = 0; j < rigidbodies.Length; j++)
            {
                Rigidbody rb = rigidbodies[j];

                if (!rb.isKinematic)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }

                rb.useGravity = false;
                rb.isKinematic = true;
            }

           

            // Desactiva los colliders de la pieza y sus hijos.
            Collider[] colliders = child.GetComponentsInChildren<Collider>(true);

            for (int j = 0; j < colliders.Length; j++)
            {
                colliders[j].enabled = false;
            }

            // Oculta el primer hijo.
            if (child.transform.childCount > 0)
            {
                child.transform.GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                Debug.LogWarning($"{child.name} does not have a first child.");
            }

            // Alinea la pieza con su padre.
            child.transform.localPosition = Vector3.zero;
            child.transform.localRotation = Quaternion.identity;


            Debug.Log(
    $"Índice: {i} | Objeto: {child.name} | " +
    $"Total: {drillComponents.Count} | " +
    $"Posición local: {child.transform.localPosition}" + $"Rotation local: {child.transform.localRotation}"
);
        }


        //// Testing
        drillComponents[drillComponents.Count - 1].transform.localPosition = new Vector3(0, 0, 0);

        drillComponents[drillComponents.Count - 1].transform.localRotation = Quaternion.identity;

        // Final testing
        DestroyDrillComponents();
        DestroyGhostComponents();


        if (interactable != null)
        {
            interactable.SetActive(true);

            GetComponent<Collider>().isTrigger = false;



            // Creates a new drill
            DrillGenerator.Instance.GenerateNewDrill(); // Testing

        }
        else
        {
            Debug.LogWarning("interactable is null");
        }


        isComplete = true;

    }

    public void ActivateGravity()
    {
        if (gravityActivated == false && isSelected)
        {
            UpdateGravity(true);
        }
    }

    //private void UpdateGravity(bool status)
    //{
    //    gravityActivated = status;

    //    Rigidbody rb = GetComponent<Rigidbody>();

    //    rb.linearVelocity = Vector3.zero;
    //    rb.angularVelocity = Vector3.zero;

    //    rb.useGravity = status;
    //    rb.isKinematic = !status;
    //}

    // Improved gravity

    private void UpdateGravity(bool status)
    {
        if (!TryGetComponent<Rigidbody>(out var rb))
        {
            Debug.LogError($"{name}: falta el Rigidbody.", this);
            return;
        }

        if (status)
        {
            // Primero permitir que la física lo controle.
            rb.isKinematic = false;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = true;
        }
        else
        {
            // Limpiar velocidades mientras todavía es dinámico.
            if (!rb.isKinematic)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            rb.useGravity = false;
            rb.isKinematic = true;
        }

        gravityActivated = status;
    }

    public void IsSelected(bool status)
    {
        isSelected = status;
        Vibration.Instance.Vibrate(0.2f);

    }
    //Testing
    public void DestroyDrillComponents()
    {
        if (drillComponents == null) return;

        foreach (GameObject obj in drillComponents.ToArray())
        {
            if (obj == null) continue;

            // 1. SnapObject
            foreach (var component in obj.GetComponents<SnapObject>())
                Destroy(component);

            // 2. Collider
            foreach (var component in obj.GetComponents<Collider>())
                Destroy(component);

            // 3. Grabbable
            foreach (var component in obj.GetComponents<Grabbable>())
                Destroy(component);

            // 4. InteractableUnityEventWrapper
            foreach (var component in obj.GetComponents<InteractableUnityEventWrapper>())
                Destroy(component);

            // 5. GrabFreeTransformer
            foreach (var component in obj.GetComponents<GrabFreeTransformer>())
                Destroy(component);

            // 6. RigidbodyKinematicLocker
            foreach (var component in obj.GetComponents<RigidbodyKinematicLocker>())
                Destroy(component);

            // 7. Rigidbody
            foreach (var component in obj.GetComponents<Rigidbody>())
                Destroy(component);
        }
    }

    public void DestroyGhostComponents()
    {
        if (ghostlist == null) return;

        foreach (GameObject ghost in ghostlist.ToArray())
        {
            if (ghost == null) continue;

            // Primero SnapTarget.
            foreach (var component in ghost.GetComponents<SnapTarget>())
                Destroy(component);

            // Después todos los colliders del objeto.
            foreach (var component in ghost.GetComponents<Collider>())
                Destroy(component);
        }
    }
    #endregion

}