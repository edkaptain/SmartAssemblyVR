using OpenAI;
using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;


public class TTSGPT : MonoBehaviour
{
    [Header("OpenAI")]
    [SerializeField] private string apiKey;
    private OpenAIApi openai = new OpenAIApi();

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;

    public void Speak(string text, System.Action onFinished)
    {
        StartCoroutine(CreateSpeech(text, onFinished));
    }

    public void Speak(string text)
    {
        StartCoroutine(CreateSpeech(text, null));
    }

    private IEnumerator CreateSpeech(string text,System.Action onFinished)
    {
        string url = "https://api.openai.com/v1/audio/speech";

        string json = $@"
        {{
            ""model"": ""gpt-4o-mini-tts"",
            ""input"": ""{text}"",
            ""voice"": ""coral"",
            ""instructions"": ""Speak in a cheerful and positive tone as a robotic tone."",
            ""response_format"": ""wav""
        }}";

        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using UnityWebRequest request = new UnityWebRequest(url, "POST");

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("OpenAI TTS Error: " + request.error);
            Debug.LogError(request.downloadHandler.text);
            yield break;
        }

        byte[] audioBytes = request.downloadHandler.data;

        AudioClip clip = WavToAudioClip(audioBytes, "OpenAI_TTS");

        audioSource.clip = clip;
        audioSource.Play();
                
        // Activates the current event
        onFinished?.Invoke();

    }
    private int FindDataChunk(byte[] wavBytes)
    {
        for (int i = 12; i < wavBytes.Length - 8; i++)
        {
            if (wavBytes[i] == 'd' &&
                wavBytes[i + 1] == 'a' &&
                wavBytes[i + 2] == 't' &&
                wavBytes[i + 3] == 'a')
            {
                return i;
            }
        }

        return -1;
    }

    private AudioClip WavToAudioClip(byte[] wavBytes, string clipName)
    {
        if (wavBytes == null || wavBytes.Length < 44)
        {
            Debug.LogError("WAV vacío o inválido.");
            return null;
        }

        string riff = Encoding.ASCII.GetString(wavBytes, 0, 4);

        if (riff != "RIFF")
        {
            Debug.LogError("El archivo recibido no es WAV RIFF. Inicio: " + riff);
            return null;
        }

        int channels = BitConverter.ToInt16(wavBytes, 22);
        int sampleRate = BitConverter.ToInt32(wavBytes, 24);

        int dataIndex = FindDataChunk(wavBytes);

        if (dataIndex < 0)
        {
            Debug.LogError("No se encontró el chunk 'data' en el WAV.");
            return null;
        }

        int dataSize = BitConverter.ToInt32(wavBytes, dataIndex + 4);
        int audioDataStart = dataIndex + 8;

        // AQUÍ ESTÁ LA CORRECCIÓN IMPORTANTE
        if (dataSize <= 0 || dataSize > wavBytes.Length - audioDataStart)
        {
            Debug.LogWarning("DataSize inválido en header: " + dataSize + ". Usando tamaño restante del byte array.");
            dataSize = wavBytes.Length - audioDataStart;
        }

        int bitsPerSample = BitConverter.ToInt16(wavBytes, 34);

        if (bitsPerSample != 16)
        {
            Debug.LogError("Este parser solo soporta WAV de 16-bit PCM. Bits recibidos: " + bitsPerSample);
            return null;
        }

        if (channels <= 0)
        {
            Debug.LogError("Channels inválido: " + channels);
            return null;
        }

        if (sampleRate <= 0)
        {
            Debug.LogError("SampleRate inválido: " + sampleRate);
            return null;
        }

        int bytesPerSample = bitsPerSample / 8;
        int sampleCount = dataSize / bytesPerSample;

        if (sampleCount <= 0)
        {
            Debug.LogError("SampleCount inválido: " + sampleCount);
            return null;
        }

        float[] samples = new float[sampleCount];

        int byteIndex = audioDataStart;

        for (int i = 0; i < sampleCount; i++)
        {
            if (byteIndex + 1 >= wavBytes.Length)
            {
                Debug.LogWarning("Se llegó al final del WAV antes de terminar.");
                break;
            }

            short sample = BitConverter.ToInt16(wavBytes, byteIndex);
            samples[i] = sample / 32768f;
            byteIndex += bytesPerSample;
        }

        int totalSamples = sampleCount / channels;

        if (totalSamples <= 0)
        {
            Debug.LogError("TotalSamples inválido: " + totalSamples);
            return null;
        }

        AudioClip audioClip = AudioClip.Create(
            clipName,
            totalSamples,
            channels,
            sampleRate,
            false
        );

        audioClip.SetData(samples, 0);

        return audioClip;
    }

}
