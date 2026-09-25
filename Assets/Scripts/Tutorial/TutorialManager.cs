using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;
    [Header("Testing")]
    public Light environmentLight;
    public TutorialVoiceManager voiceManager;
    public GameObject[] activities;

    public GameObject canvas;
    public List<GameObject> slides = new List<GameObject>();
    public int current = 0;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ResetSlides();
        ColorUtility.TryParseHtmlString("#FFF4D6", out Color warmColor);
        environmentLight.color = warmColor;       
    }

  

    private void OnValidate()
    {
        if (canvas != null)
        {
            SlidesFinder(canvas);
        }
    }

    private void SlidesFinder(GameObject gameObject)
    {
        slides.Clear();

        foreach (Transform child in gameObject.transform)
        {
            slides.Add(child.gameObject);
        }

        slides.RemoveAt(slides.Count - 1);
    }

    [ContextMenu("Reset all sildes")]
    public void ResetSlides()
    {
        foreach (GameObject child in slides)
        {
            child.SetActive(false);
        }

        ChangeLight();
        slides[0].SetActive(true);
        current = 0;

        foreach (var activities in activities)
        {
            activities.SetActive(false);
        }
    }

    [ContextMenu("Change the next slide")]
    public void ChangeNextSlide()
    {
        if (current != slides.Count - 1) {
            slides[current].SetActive(false);

            slides[current + 1].SetActive(true);
            current++;

            if (voiceManager.audiosource.isPlaying)
            {
                voiceManager.audiosource.Stop();
            }
            
            ChangeLight();
        }
        else
        {
            ResetSlides();
        }     
        

        // Scene 6

        
        environmentLight.intensity = current == 5 ? 0.25f : 1f;

    }


    public void ChangeLight()
    {
        int[] arr = { 2,3,5};

        ColorUtility.TryParseHtmlString("#FFF4D6", out Color warmColor);
        ColorUtility.TryParseHtmlString("#D6FBFF", out Color coldColor);

        bool isInArray = false;

        for (int i = 0; i < arr.Length; i++)
        {
            if (current == arr[i])
            {
                isInArray = true;
                break;
            }
        }

        environmentLight.color = isInArray ? coldColor : warmColor;
    }
}
