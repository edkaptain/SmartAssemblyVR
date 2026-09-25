using UnityEngine;
/// <summary>
/// Crea una lista de objetos ya definidos:)
/// </summary>

public class SnapManager : MonoBehaviour
{
    public static SnapManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void TrySnap(SnapObject snapObject, SnapTarget target)
    {
        if (snapObject == null || target == null)
            return;

        if (!target.CanAccept(snapObject))
            return;

        if (snapObject.isSnapped == true)
            return;

        Debug.LogWarning($"The object {snapObject} has entered in {target}");

        // Effects Vibration, Sounds 
        Vibration.Instance.Vibrate(0.1f);
        AudioManager.Instance.Click();
        target.onAction?.Invoke(); // Testing
        snapObject.SnapToTarget(target);
        target.UpdateMeshRenderer(false);
        target.SetObject(snapObject);

        // Testing if the object is the drill object
        Transform parentTransform = target.transform.parent;

        if (parentTransform != null)
        {
            DrillObject parent = parentTransform.GetComponent<DrillObject>();

            if (parent != null)
            {
                parent.AddToDrill(snapObject.gameObject);
            }
        }

        UpdateChild(true, snapObject, target);
    }



    public void TryUnsnap(SnapObject snapObject, SnapTarget target)
    {
        if (snapObject == null || target == null)
            return;

        Debug.LogWarning($"The object {snapObject} has exit from {target}");

        target.UpdateMeshRenderer(true);
        snapObject.Unsnap();

        // Testing if the object is the drill object
        Transform parentTransform = target.transform.parent;

        if (parentTransform != null)
        {
            DrillObject parent = parentTransform.GetComponent<DrillObject>();

            if (parent != null)
            {
                parent.RemoveFromDrill(snapObject.gameObject);
            }
        }

        UpdateChild(false, snapObject, target);
    }

    public void UpdateChild(bool status, SnapObject snapObject, SnapTarget snapTarget)
    {
        if (status == true)
        {
            snapObject.transform.SetParent(snapTarget.transform, true);
        }
        else
        {
            snapObject.transform.SetParent(null, true);
        }
    }

}
