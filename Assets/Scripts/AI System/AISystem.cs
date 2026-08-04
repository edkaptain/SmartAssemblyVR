using OpenAI;
using UnityEngine;
using UnityEngine.UI;

public class AISystem : MonoBehaviour
{
    [SerializeField] public MyWhisper whisper;
    [SerializeField] private MyChatGPT chatgptScreen;
    [SerializeField] public TTSGPT textToSpeech;
    [SerializeField] public Andon andon;

    #region ===== Singleton =====

    public static AISystem Instance;

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

    public void StartRecording()
    {
        if (whisper == null)
        {
            Debug.LogError("Whisper reference is missing.");
            return;
        }

        if (whisper.isRecording == false)
        {
            AudioManager.Instance.SystemNotification(true);
            whisper.StartRecording();
            chatgptScreen.MicrophoneScreen();
            andon.TurnColor(Andon.LightColor.green, false);
            andon.TurnColor(Andon.LightColor.yellow, true);
        }
    }

    public async void StopRecording()
    {
        if (whisper == null)
        {
            Debug.LogError("Whisper reference is missing.");
            return;
        }

        if (whisper.isRecording == false)
        {
            Debug.LogWarning("Whisper is not recording.");
            return;
        }


        AudioManager.Instance.SystemNotification(false);
        chatgptScreen.StopMicrophoneScreenRecording();

        string message = await whisper.EndRecording();        

        PublishToGPT(message);

        andon.TurnColor(Andon.LightColor.yellow, false);

        andon.TurnColor(Andon.LightColor.green, true);
    }

    private void PublishToGPT(string message)
    {
        if (chatgptScreen != null)
        {
            chatgptScreen.SendReply(message);
        }
        else
        {
            Debug.LogError("ChatGPT screen reference is missing.");
        }
    }

    

}