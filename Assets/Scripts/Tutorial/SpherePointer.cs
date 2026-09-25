using UnityEngine;

public class SpherePointer : MonoBehaviour
{
    public Transform vrCamera;
    public float distance = 2f;
    float smoothSpeed = 5f;

    // Update is called once per frame
    private void LateUpdate()
    {
        Vector3 targetPosition = vrCamera.position + vrCamera.forward * distance;

        float smooting = 1f - Mathf.Exp(-smoothSpeed * Time.deltaTime);

        transform.position = Vector3.Lerp(transform.position, targetPosition, smooting);
    }
}
