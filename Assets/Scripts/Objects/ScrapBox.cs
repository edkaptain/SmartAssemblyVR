using Oculus.Interaction;
using OpenAI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrapBox : MonoBehaviour
{
    [SerializeField] private MyChatGPT gptScreen;
    private Queue<SnapObject> defectiveQueue = new Queue<SnapObject>();
    private bool isProcessingDefects = false;

    private void OnTriggerEnter(Collider other)
    {
        SnapObject snap = other.GetComponent<SnapObject>();

        if (other.CompareTag("Item"))
        {
            MFGDashboard.Instance.UpdateScrap(1);
        }

        if (snap != null && snap.isDefective)
        {
            defectiveQueue.Enqueue(snap);

            if (!isProcessingDefects)
            {
                StartCoroutine(ProcessDefectiveQueue(snap));
            }

            AISystem.Instance.andon.BlinkLight(Andon.LightColor.red, 2f, 0.5f, true);

            Debug.LogWarning("The defective: " + other.gameObject + " has been detected");

           
        }
    }

    private IEnumerator ProcessDefectiveQueue(SnapObject snap)
    {
        isProcessingDefects =true;

        while(defectiveQueue.Count > 0)
        {
            SnapObject current = defectiveQueue.Dequeue();

            gptScreen.ChangeImage(current);

            AISystem.Instance.textToSpeech.Speak(
               $"Defective {snap.objectType} has been detected"
           );

            yield return new WaitUntil(() => gptScreen.timerFinished == true);
        }

        isProcessingDefects = false;
    }
}
