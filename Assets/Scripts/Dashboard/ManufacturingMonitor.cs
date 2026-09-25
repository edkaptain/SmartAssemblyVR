using UnityEngine;
using UnityEngine.UI;

public class ManufacturingMonitor : MonoBehaviour
{
    public GameObject canvas;

    public int boxTotal = 0;
    public int boxCompleted;
    public Slider slider;
    public Text sliderTxt;
    [Header("Cards")]
    public CycleTime cycleTime;
    public AssemblyStep step;
    public RemainingSteps remainingSteps;
    public ProductsCompleted products;

    #region Singlenton
    public static ManufacturingMonitor Instance;

    private void Awake()
    {
        Instance = this;
    }
    #endregion

    //private void Start()
    //{
    //    canvas.SetActive(false);
    //}


    public void AddBoxes()
    {
        boxTotal += 1;
        slider.maxValue = boxTotal;
    }

    public void UpdateSlider()
    {
        boxCompleted += 1;
        slider.value = boxCompleted;
        sliderTxt.text = $"{((float)boxCompleted / boxTotal * 100):F0}%";
    }

    #region ===== Common propoerties =====
    public static class colors
    {
        public static readonly Color Green = new Color32(0x0D, 0x53, 0x0E, 255);

        public static readonly Color Yellow = new Color32(0xF5, 0xE0, 0x0E, 255);

        public static readonly Color Red = new Color32(0xF5, 0x22, 0x25, 255);
    }

    #endregion
}
