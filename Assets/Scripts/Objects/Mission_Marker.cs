using UnityEngine;
using UnityEngine.Events;

public class Mission_Marker : MonoBehaviour
{
    public UnityEvent events;

    private void Start()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("VRController"))
        {
            gameObject.SetActive(false);
            AudioManager.Instance.Sucess();
            events!.Invoke();
        }
    }
}
