using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DrillAssembly : MonoBehaviour
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

        
        foreach (GameObject child in drillComponents)
        {
            if (child == null)
            {
                Debug.LogWarning("One component in drillComponents is null");
                continue;
            }

            SnapObject snapObj = child.GetComponent<SnapObject>();
            snapObj.UpdateGravity(false);
            snapObj.GetComponent<Collider>().isTrigger = true;
            // Gives an error when its activated as explotes
            //snapObj.enabled = false;

            if (child.transform.childCount > 0)
            {
                Transform firstChild = child.transform.GetChild(0);
                firstChild.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogWarning($"{child.name} does not have a first child.");
            }
        }

        if (interactable != null)
        {
            interactable.SetActive(true);

            // Creates a new drill
            DrillGenerator.Instance.DetachCurrentDrill(); // Testing

        }
        else
        {
            Debug.LogWarning("interactable is null");
        }


        isComplete = true;

    }

    public void ActivateGravity()
    {
        if(gravityActivated == false && isSelected)
        {
            UpdateGravity(true);
        }
    }

    private void UpdateGravity(bool status)
    {
        gravityActivated = status;

        Rigidbody rb = GetComponent<Rigidbody>();

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.useGravity = status;
        rb.isKinematic = !status;
    }

    public void IsSelected(bool status)
    {
        isSelected = status;
        Vibration.Instance.Vibrate(0.2f);
        
    }


    #endregion

}
