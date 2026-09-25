using System.Collections.Generic;
using UnityEngine;

public class TutorialTurnAround : MonoBehaviour
{
    public List<GameObject> vectors = new List<GameObject>();
    public int points;
    public GameObject btn;
    public GameObject player;
    public GameObject pointer;

    private void OnValidate()
    {
        CollectVectors();
    }

    private void Start()
    {
        pointer.SetActive(true);
    }

    [ContextMenu("Collect vectors")]
    private void CollectVectors()
    {
        vectors.Clear();

        foreach (Transform child in transform)
        {
            vectors.Add(child.gameObject);
            child.gameObject.SetActive(false);
        }

        vectors[0].gameObject.SetActive(true);
        vectors[0].GetComponent<TurnAroundItem>().TurnText(player);
    }
    [ContextMenu("Add points")]
    public void AddPoints()
    {
        points++;

        if (points >= vectors.Count)
        {
            pointer.SetActive(false);
            btn.SetActive(true);
            AudioManager.Instance.Sucess();
            Debug.LogWarning("This part was completed");
            return;
        }

        vectors[points].SetActive(true);
        vectors[points].GetComponent<TurnAroundItem>().TurnText(player);

    }
}
