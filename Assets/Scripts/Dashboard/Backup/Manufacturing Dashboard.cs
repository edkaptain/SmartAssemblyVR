using System;
using UnityEngine;
using UnityEngine.UI;

public class MFGDashboard : MonoBehaviour
{
    #region ===== Inspector References =====

    [System.Serializable]
    public class ProgressCard
    {
        public Image progressImage;

        [Range(0f, 1f)]
        public float value = 0;

        public int totalProducts = 1;

        public Text txtPercentage;

        public static class colors
        {
            public static readonly Color Green =
                new Color32(0x0D, 0x53, 0x0E, 255);

            public static readonly Color Yellow =
                new Color32(0xF5, 0xE0, 0x0E, 255);

            public static readonly Color Red =
                new Color32(0xF5, 0x22, 0x25, 255);
        }
        
        

        public virtual void UpdateValue(float newValue)
        {
            value = Mathf.Clamp01(newValue);

            txtPercentage.text = (value * 100).ToString("0") + "%";

            progressImage.fillAmount = value;

        }
    }

    [System.Serializable]
    public class ProductionCard : ProgressCard
    {
        public override void UpdateValue(float newValue)
        {
            value = Mathf.Clamp01(newValue);

            txtPercentage.text = (value * 100).ToString("0") + "%";

            progressImage.fillAmount = value;

            if (value >= 0.8f)
            {
                progressImage.color = colors.Green;
            }
            else if (value >= 0.5f)
            {

                progressImage.color = colors.Yellow;
            }
            else
            {

                progressImage.color = colors.Red;
            }

        }

    }

    [System.Serializable]
    public class RadialCard : ProgressCard
    {
        public Text statusText;
        public Image[] backgroundColor;

        public string[] phrases =
        {
        "Status: Good Performance",
        "Status: Average Performance",
        "Status: Low Performance"
    };

        public override void UpdateValue(float newValue)
        {
            base.UpdateValue(newValue);

            if (value >= 0.8f)
            {
                statusText.text = phrases[0];
                progressImage.color = colors.Green;
                foreach (Image img in backgroundColor)
                {
                    img.color = colors.Green;
                }
            }
            else if (value >= 0.5f)
            {

                statusText.text = phrases[1];
                progressImage.color = colors.Yellow;
                foreach (Image img in backgroundColor)
                {
                    img.color = colors.Yellow;
                }
            }
            else
            {

                statusText.text = phrases[2];
                progressImage.color = colors.Red;
                foreach (Image img in backgroundColor)
                {
                    img.color = colors.Red;
                }
            }
        }
    }
    [System.Serializable]
    public class SimpleCard
    {
        public int value;
        public Text statusTxt;

        public void UpdateValue(int newValue)
        {
            value = newValue;
            statusTxt.text = value.ToString();
        }
    }

    [System.Serializable]
    public class RemainingSteps : RadialCard
    {
        public int totalSteps = 24;
        public int actualStep = 0;

        public override void UpdateValue(float newValue = 0)
        {

            // Calcular progreso automáticamente
            value = Mathf.Clamp01((float)actualStep / totalSteps);

            // Actualiza círculo radial
            progressImage.fillAmount = value;

            // Muestra 16/24
            txtPercentage.text = $"{actualStep}/{totalSteps}";

            if (value >= 0.8f)
            {
                statusText.text = phrases[2];
                progressImage.color = colors.Green;
                foreach (Image img in backgroundColor)
                {
                    img.color = colors.Green;
                }
            }
            else if (value >= 0.5f)
            {

                statusText.text = phrases[1];
                progressImage.color = colors.Yellow;
                foreach (Image img in backgroundColor)
                {
                    img.color = colors.Yellow;
                }
            }
            else
            {

                statusText.text = phrases[0];
                progressImage.color = colors.Red;
                foreach (Image img in backgroundColor)
                {
                    img.color = colors.Red;
                }
            }
        }
    }
    [System.Serializable]
    public class CycleTime : RadialCard
    {
        public Text target;
        public float currentTime = 0f;

        public float targetCycleTime = 30f; // objetivo: 30 segundos

        public override void UpdateValue(float newValue)
        {
            float currentTime = newValue; // ejemplo: 50 segundos

            // Mostrar tiempo real
            TimeSpan current = TimeSpan.FromSeconds(currentTime);
            TimeSpan targetTime = TimeSpan.FromSeconds(targetCycleTime);

            if (currentTime < 60)
            {
                txtPercentage.text = currentTime.ToString() + "s";
            }
            else
            {
                txtPercentage.text = current.ToString(@"m\:ss");
            }

            if (targetCycleTime < 60)
            {
                target.text = $"Target: {targetCycleTime}s";
            }
            else
            {
                target.text = $"Target: {targetTime:m\\:ss}";
            }

            // Convertir segundos a porcentaje para el radial
            float ratio = currentTime / targetCycleTime;


            progressImage.fillAmount = ratio;

            // En Cycle Time, menor es mejor
            if (ratio <= 1f)
            {
                statusText.text = phrases[0]; // Good
                SetColor(colors.Green);
            }
            else if (ratio <= 1.3f)
            {
                statusText.text = phrases[1]; // Warning
                SetColor(colors.Yellow);
            }
            else
            {
                statusText.text = phrases[2]; // Bad
                SetColor(colors.Red);
            }
        }

        private void SetColor(Color color)
        {
            progressImage.color = color;

            if (backgroundColor != null)
            {
                foreach (Image img in backgroundColor)
                {
                    img.color = color;
                }
            }
        }
    }

    [Header("Cards")]
    [SerializeField] private ProductionCard productionCard;
    [SerializeField] private RadialCard qualityCard;
    [SerializeField] private RemainingSteps remainingSteps;
    [SerializeField] private CycleTime cycleTime;
    [SerializeField] private SimpleCard scrap;
    [SerializeField] public SimpleCard productsCompleted;

    #endregion

    #region ===== Unity Lifecycle =====

    private void OnValidate()
    {
        if (productionCard != null)
        {
            productionCard.UpdateValue(productionCard.value);
        }

        if (qualityCard != null)
        {
            qualityCard.UpdateValue(qualityCard.value);
        }

        if (remainingSteps != null)
        {
            remainingSteps.UpdateValue(remainingSteps.actualStep);
        }
        scrap.UpdateValue(scrap.value);
        productsCompleted.UpdateValue(productsCompleted.value);
        cycleTime.UpdateValue(cycleTime.currentTime);
    }

    // Singlenton interface

    public static MFGDashboard Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

    }
    #endregion

    #region ===== Functions =====

    public void UpdateScrap(int points)
    {
        scrap.UpdateValue(scrap.value + points);
    }
    public void UpdateProductsCompleted(int points)
    {
        productsCompleted.UpdateValue(productsCompleted.value + points);
    }


    #endregion

}
