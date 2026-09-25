using UnityEngine;

public class FinalBox : MonoBehaviour
{
    private void Start()
    {
        ManufacturingMonitor.Instance.AddBoxes();
    }

    public void OnAttached()
    {
        ManufacturingMonitor.Instance.UpdateSlider();
        // Generates a new box
        OrangeBoxGenerator.Instance.GenerateNewBox();

    }
}
