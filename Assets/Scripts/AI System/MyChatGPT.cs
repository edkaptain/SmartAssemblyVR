using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace OpenAI
{
    public class MyChatGPT : MonoBehaviour
    {

        [Header("ChatGPT Screen")]
        public GameObject chatScreen;
       [SerializeField] private ScrollRect scroll;

        [SerializeField] private RectTransform sent;
        [SerializeField] private RectTransform received;

        [Header("Microphone Screen")]
        public GameObject microScreen;
        public Text secondsTxt;
        public Animator anim;

        private float height;
        private OpenAIApi openai = new OpenAIApi();

        private List<ChatMessage> messages = new List<ChatMessage>();
        private string prompt = "Act as a random stranger in a chat room and reply to the questions. Don't break character. Don't ever mention that you are an AI model.";

        [Header("Defective Product")]
        [SerializeField] private GameObject defectiveScreen;
        [SerializeField] private Text txt;
        [SerializeField] private Image img;
        public bool timerFinished { get; private set; }
        // Delete
        public bool status;

        
        private void Update()
        {
            if (AISystem.Instance.whisper.isRecording)
            {
                var time = (int)AISystem.Instance.whisper.time;

                secondsTxt.text = time.ToString();
            }

        }

        private void AppendMessage(ChatMessage message)
        {
            scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);

            var item = Instantiate(message.Role == "user" ? sent : received, scroll.content);
            item.GetChild(0).GetChild(0).GetComponent<Text>().text = message.Content;
            item.anchoredPosition = new Vector2(0, -height);
            LayoutRebuilder.ForceRebuildLayoutImmediate(item);
            height += item.sizeDelta.y;
            scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            scroll.verticalNormalizedPosition = 0;
        }

        public async void SendReply(string messageTXT)
        {
            var newMessage = new ChatMessage()
            {
                Role = "user",
                Content = messageTXT
            };

            AppendMessage(newMessage);

            if (messages.Count == 0) newMessage.Content = prompt + "\n";

            messages.Add(newMessage);


            // Complete the instruction
            var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
            {
                Model = "gpt-5.4-mini",
                Messages = messages
            });
            // Modify the 0 output
            if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
            {
                var message = completionResponse.Choices[0].Message;
                message.Content = message.Content.Trim();

                
                // Plays the voice output, then displays the text on the screen when the audio finishes.
                AISystem.Instance.textToSpeech.Speak(message.Content, () => {

                    messages.Add(message);
                    AppendMessage(message);

                });

                Debug.Log("ChatGPT:" + message.Content);
            }
            else
            {
                Debug.Log("No text was generated from this prompt.");
            }

        }

        public void MicrophoneScreen()
        {
            chatScreen.SetActive(false);
            microScreen.SetActive(true);

            anim.SetBool("Bool", true);

        }

        public void StopMicrophoneScreenRecording()
        {
            anim.SetBool("Bool", false);

            chatScreen.SetActive(true);
            microScreen.SetActive(false);
        }

        public void ChangeImage(SnapObject component)
        {
            Debug.LogWarning("Changing screen");
            timerFinished = false;

            chatScreen.SetActive(false);

            if (component.img != null)
            {
                img.sprite = component.img;
            }
            else
            {
                img.sprite = Resources.Load<Sprite>("No Image");
            }

            ScreenTimer(defectiveScreen, 5);
        }

        public void ScreenTimer(GameObject screen, int time)
        {
            StartCoroutine(ScreenTimerCoroutine(screen, time));
        }

        private IEnumerator ScreenTimerCoroutine(GameObject screen, int time)
        {
            screen.SetActive(true);

            yield return new WaitForSeconds(time);

            screen.SetActive(false);
            chatScreen.SetActive(true);
            timerFinished = true;
        }

    }
}
