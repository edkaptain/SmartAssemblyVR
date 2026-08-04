using OpenAI;
using Samples.Whisper;
using System.Threading.Tasks;
using UnityEngine;

public class MyWhisper : MonoBehaviour
{
    private readonly string fileName = "output.wav";

    private AudioClip clip;
    public bool isRecording;
    public float time;
    public int maxDuration = 25;
    private OpenAIApi openai = new OpenAIApi();

    private void Update()
    {
        if (isRecording)
        {
            time += Time.deltaTime;
        }
        else
        {
            time = 0;
        }
    }

    public void StartRecording()
    {
        isRecording = true;
        clip = Microphone.Start(null,false, maxDuration, 44100);
    }

    public async Task<string> EndRecording()
    {
        isRecording = false;
        Microphone.End(null);

        // Converts the audo to byte file
        byte[] data = SaveWav.Save(fileName, clip);

        var req = new CreateAudioTranscriptionsRequest
        {
            FileData = new FileData() { Data = data, Name = "audio.wav" },
            Model = "whisper-1",
            Language = "en",
        };
        
        var res = await openai.CreateAudioTranscription(req);
        Debug.LogError("My Responde:" + res.Text.ToString());
        return res.Text;
    }

    
}
