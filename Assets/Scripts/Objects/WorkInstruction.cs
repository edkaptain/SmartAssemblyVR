using System.Collections.Generic;
using UnityEngine;
public class WorkInstruction : MonoBehaviour
{
    public GameObject canvas;
    public List<GameObject> slides = new List<GameObject>();
    public int current;

    private void OnValidate()
    {
        if (canvas != null)
        {
            SlidesFinder(canvas);
        }
    }


    private void Start()
    {
        ResetSlides();
    }

    // Adds all the slides into the list<Gameobject>
    private void SlidesFinder(GameObject gameObject)
    {
        slides.Clear();

        foreach(Transform child in gameObject.transform)
        {
            slides.Add(child.gameObject);
        }
        slides.RemoveAt(slides.Count - 1);
    }

    [ContextMenu("Reset all sildes")]
    public void ResetSlides()
    {
        SlidesFinder(canvas);
        foreach (GameObject child in slides)
        {
            child.SetActive(false);
        }
        current = 0;

        slides[current].SetActive(true);
    }

    [ContextMenu("Go to previous")]
    public void ChangePreviousSlide()
    {
        if (current != 0)
        {
            slides[current].SetActive(false);
            slides[current - 1].SetActive(true);

            current--;
        }

    }

    [ContextMenu("Change the next slide")]
    public void ChangeNextSlide()
    {
        if (current != slides.Count - 1) {
            // actual slide
            slides[current].SetActive(false);

            // next slide
            slides[current + 1].SetActive(true);
            current++;

            // add voices
        }
        else
        {
            ResetSlides() ;
        }
    }

   
}
