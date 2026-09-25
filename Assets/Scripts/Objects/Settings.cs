using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public GameObject vrPlayer;
    public Text txt;
    public Vector3 offset;
    public bool status;
    private float time;

    private void Update()
    {
        time += Time.deltaTime;

        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        txt.text = $"Time Elapsed: {minutes:00}:{seconds:00}";
    }

    public void ShowSettings()
    {
        gameObject.transform.position = vrPlayer.transform.position + offset;
        gameObject.SetActive(status);
        status = !status;
    }
}
