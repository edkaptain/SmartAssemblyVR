using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controlls the cycle time card
/// </summary>
public class CycleTime : MonoBehaviour
{
    [Header("Cycle Time")]
    [Range(0f, 1f)]
    [SerializeField] private float value;

    [SerializeField] private float elapsedTime;
    [SerializeField] private float averageTime;

    /// <summary>
    /// Target time is define by user as the goal
    /// </summary>
    [SerializeField] private float targetTime;
    private float totalTime;
    private int cycles;

    private bool isActive;

    [Header("UI")]
    [SerializeField] private Image percentageImage;
    [SerializeField] private Image[] backgroundImages;

    [Header("UI Text")]
    [SerializeField] private Text currentTimeTxt;
    [SerializeField] private Text targetTimeTxt;
    [SerializeField] private Text statusText;

    [Header("Status Messages")]
    [SerializeField] private string belowTargetText = "Status: Below the Target";
    [SerializeField] private string onTargetText = "Status: On Target";
    [SerializeField] private string excellentText = "Status: Excellent Performance";


    // Notes: Mean controlls the bottom card, elasedtime controlls the value circle range, the total is the max value for the fircle value 

    private void Start()
    {
        RefreshUI();
        StartStopWatch();
    }

    private void Update()
    {
        if (!isActive) return;

        elapsedTime += Time.deltaTime;
        totalTime += Time.deltaTime;
        RefreshUI();

    }

    private void StartStopWatch()
    {
        isActive = true;
    }

    private void StopStopWatch()
    {
        isActive = false;
    }

    private void ResetStopWatch()
    {
        StopStopWatch();

        cycles = 0;
        elapsedTime = 0f;
        averageTime = 0f;
        totalTime = 0f;
    }

    private void LapStopWatch()
    {
        cycles++;
        averageTime = totalTime / cycles;
        elapsedTime = 0f;

        // Normalization for mean to [0,1]
        float value = (averageTime - 0) / (targetTime - 0);

        UpdateValue(value);
        UpdateStopwatchUI();
    }


    /// <summary>
    /// Updates the cycle time value and refreshes the UI.
    /// </summary>
    public void UpdateValue(float newValue)
    {
        value = Mathf.Clamp01(newValue);

        RefreshUI();
    }

    public void UpdateStopwatchUI()
    {
        if (currentTimeTxt != null)
            currentTimeTxt.text = FormatTime(elapsedTime);

        if (targetTimeTxt != null)
            targetTimeTxt.text = $"Target\n{FormatTime(targetTime)}";
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        return $"{minutes:00}:{seconds:00}";
    }


    /// <summary>
    /// Updates all UI elements according to the current value.
    /// </summary>
    private void RefreshUI()
    {
        Color statusColor;
        string status;

        // Updates the stopwatch
        UpdateStopwatchUI();

      


        if (value >= 0.8f)
        {
            status = excellentText;
            statusColor = ManufacturingMonitor.colors.Green;
        }
        else if (value >= 0.5f)
        {
            status = onTargetText;
            statusColor = ManufacturingMonitor.colors.Yellow;
        }
        else
        {
            status = belowTargetText;
            statusColor = ManufacturingMonitor.colors.Red;
        }

        // Percentage / progress bar
        if (percentageImage != null)
        {
            percentageImage.fillAmount = elapsedTime;
            percentageImage.color = statusColor;
        }

        // Status text
        if (statusText != null)
        {
            statusText.text = status;
        }

        // Background images
        if (backgroundImages != null)
        {
            foreach (Image image in backgroundImages)
            {
                if (image != null)
                    image.color = statusColor;
            }
        }
    }


    /// <summary>
    /// Automatically updates the UI when values change in the Inspector.
    /// </summary>
    private void OnValidate()
    {
        RefreshUI();
    }
}