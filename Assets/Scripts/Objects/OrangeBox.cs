using Oculus.Interaction;
using UnityEngine;

public class OrangeBox : MonoBehaviour
{

    [Header("Lid")]
    [SerializeField] private GameObject lid;
    [SerializeField] private bool isClosed;
    [SerializeField] private GameObject lidInteractable;

    [Header("Box")]
    [SerializeField] private GameObject boxInteractable;
    private bool closeBoxFull;
    [SerializeField] private SnapObject snapObject;
    [SerializeField] private Grabbable grabbable;

    [Header("Testing")]
    [SerializeField] private GameObject[] delete;

    private void Update()
    {
        UpdateClosedState();

        if (isClosed && closeBoxFull == false)
        {
            ActivateInteractable(true, boxInteractable);
            snapObject.enabled = true;
            GetComponent<Collider>().enabled = true;
            gameObject.GetComponent<Rigidbody>().useGravity = true;
            gameObject.GetComponent<Rigidbody>().isKinematic = false;

            grabbable.enabled = true;
            ActivateInteractable(false, lidInteractable);

            lid.GetComponent<Collider>().enabled = false;
            AudioManager.Instance.CloseBox();

            DesactivateAllComponents();
            closeBoxFull = true;

            
        }

        if (snapObject.isSnapped && delete.Length >= 0)
        {
            for (int i = delete.Length - 1; i >= 0; i--)
            {
                Destroy(delete[i]);
            }
        }
    }

    private void UpdateClosedState()
    {
        float zRotation = lid.transform.localEulerAngles.z;
        isClosed = zRotation < 1f;
    }

    public void ActivateInteractable(bool status, GameObject obj)
    {
        obj.SetActive(status);
    }

    private void DesactivateAllComponents()
    {
        foreach( Transform child in transform)
        {
            SnapTarget snapTarget = child.GetComponent<SnapTarget>();

            if(snapTarget != null)
            {
                child.GetComponent<SnapTarget>().lockSnap = true;
            }
        }
    }

}

