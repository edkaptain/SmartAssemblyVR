using UnityEngine;
using UnityEngine.Events;

public class SnapTarget : MonoBehaviour
{
    #region ===== Inspector References =====

    [Header("Target Settings")]
    public SnapItemType acceptedType;
    public bool isOccupied;
    public SnapObject snappedObject;
    public bool lockSnap;

    [Header("Snap Point")]
    public Transform snapPoint;

    public UnityEvent onAction;

    #endregion

    private void Start()
    {
        // Testing
        snapPoint = transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        SnapObject snapObject = other.GetComponentInParent<SnapObject>();

        if (snapObject == null)
            return;

        if (!CanAccept(snapObject))
            return;

        SnapManager.Instance.TrySnap(snapObject, this);
    }

    private void OnTriggerExit(Collider other)
    {
        SnapObject snapObject = other.GetComponentInParent<SnapObject>();

        if (snapObject == null)
            return;

        if (lockSnap == true) return;

        // Si el objeto ya está snapped y NO lo está agarrando el usuario,
        // ignoramos el OnTriggerExit porque puede ser un falso exit.
        if (snapObject.isSnapped && snapObject.isSelected == false)
            return;

        // Si el objeto salió mientras el usuario lo está agarrando,
        // entonces sí puedes limpiar el target.
        if (snapObject == snappedObject)
        {
            ClearTarget();
            //UpdateMeshRenderer(true);
            // Try to unsnap the object
            SnapManager.Instance.TryUnsnap(snapObject, this);
        }
    }

    public bool CanAccept(SnapObject snapObject)
    {       
        if (isOccupied)
            return false;

        if (snapObject.objectType != acceptedType)
            return false;

        if (snapObject.isSnapped == true)
            return false;

        return true;
    }

    public void SetObject(SnapObject snapObject)
    {
        snappedObject = snapObject;
        isOccupied = true;
    }

    public void ClearTarget()
    {
        snappedObject = null;
        isOccupied = false;
    }

    public void UpdateMeshRenderer(bool status)
    {

        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer renderer in renderers)
        {
            renderer.enabled = status;
        }
    }
}