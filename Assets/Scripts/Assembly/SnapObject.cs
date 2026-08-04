using UnityEngine;
using UnityEngine.UI;

public class SnapObject : MonoBehaviour
{
    #region ===== Inspector References ======
    
    [Header("Properties")]
    public bool isDefective;
    public Sprite img;
    public bool isSnapped;
    public bool isSelected;

    [Header("ObjectSettings")]
    public SnapItemType objectType;    
    public SnapTarget currentTarget;

    [Range(0f, 1f)]
    public float vibrationTime = 0.2f;


    private void Update()
    {
        if (isSnapped && isSelected == false)
        {
            Transform point = currentTarget.snapPoint != null ? currentTarget.snapPoint : currentTarget.transform;
            transform.SetPositionAndRotation(point.position, point.rotation);

           
        }
    }
    public void SnapToTarget(SnapTarget target)
    {
        Transform point = target.snapPoint != null ? target.snapPoint : target.transform;

        transform.SetPositionAndRotation(point.position, point.rotation);

        currentTarget = target;
        isSnapped = true;
        UpdateGravity(false);
       
    }

    public void Unsnap()
    {
        if (currentTarget != null)
        {
            currentTarget.ClearTarget();
        }
        Debug.LogWarning($"The {gameObject.name} has been desactived");
        currentTarget = null;
        isSnapped = false;
        UpdateGravity(true);
    }

    public void UpdateGravity(bool status)
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = status;
        rb.isKinematic = !status;
        //rb.detectCollisions = status;
    }

    public void IsSelected(bool status)
    {
        isSelected = status;
        Vibration.Instance.Vibrate(0.2f);
        if (isSelected && isSnapped)
        {
            SnapManager.Instance.TryUnsnap(this, currentTarget);
        }
    }

    #endregion
}
