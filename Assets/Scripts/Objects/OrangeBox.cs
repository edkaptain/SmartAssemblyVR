using Oculus.Interaction;
using UnityEngine;

public class OrangeBox : MonoBehaviour
{

    [Header("Lid")]
    [SerializeField] private bool isClosed;
    [SerializeField] private GameObject lid;
    [SerializeField] private GameObject lidInteractable;

    [Header("Box")]
    [SerializeField] private GameObject boxInteractable;
    [SerializeField] private SnapObject snapObject;
    [SerializeField] private Grabbable grabbable;
    [SerializeField] private GameObject backLidInteractable;

    [Header("Back lid")]

    [Header("Components")]
    [SerializeField] private GameObject[] ghostComponents;

    public bool showedComponents;

    private bool fullClosed;

    private void Update()
    {
        UpdateClosedState();

        if (isClosed)
        {

            //Activates the entire box to move
            ActivateInteractable(true, boxInteractable);
            snapObject.enabled = true;
            grabbable.enabled = true;
            GetComponent<Collider>().enabled = true;
            gameObject.GetComponent<Rigidbody>().useGravity = true;
            gameObject.GetComponent<Rigidbody>().isKinematic = false;

            // Desactivates the lid and back lid
            ActivateInteractable(false, lidInteractable);
            ActivateInteractable(false, backLidInteractable);
            lid.GetComponent<Collider>().enabled = false;
            

            AudioManager.Instance.CloseBox();

    
            

            // Desactivates orange box
            enabled = false;

            // Destroy Internal Components
            //ShowComponents(false, true); 
        }

    }

    private void UpdateClosedState()
    {
        float zRotation = lid.transform.localEulerAngles.z;
        if (zRotation <= 0.1f)
        {
            isClosed = true;
            gameObject.GetComponent<SnapObject>().objectType = SnapItemType.OrangeBoxFinal;
        }
        //isClosed = zRotation < 1f;
    }

    public void ActivateInteractable(bool status, GameObject obj)
    {
        obj.SetActive(status);
    }


    /// <summary>
    /// Shows the components from the inspector
    /// </summary>
    public void ShowComponents()
    {
        if (showedComponents == false)
        {
            ShowComponents(true, false);
            showedComponents = true;
        }
    }
  

    /// <summary>
    /// Muestra, oculta o elimina los hijos con el tag "Item".
    /// </summary>
    public void ShowComponents(bool status, bool delete)
    {
        if (ghostComponents == null) return;

        foreach (GameObject ghost in ghostComponents)
        {
            if (ghost == null) continue;

            if (delete)
            {
                Destroy(ghost);
            }
            else
            {
                ghost.SetActive(status);
            }
        }
    }

}