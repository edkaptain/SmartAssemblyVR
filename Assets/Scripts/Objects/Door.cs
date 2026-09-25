using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject door;
    
    public void OpenDoor(bool isOpened)
    {
        if (door == null)
        {
            Debug.LogWarning("There isn't a door attached.");
            return;
        }

        if (isOpened)
        {
            door.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
        }
        else
        {
            door.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }
}