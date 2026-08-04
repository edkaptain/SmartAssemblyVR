using System;
using System.Collections;
using UnityEngine;

public class Andon : MonoBehaviour
{
    public enum LightColor
    {
        green, yellow, red
    }

    [System.Serializable]
    public class AndonLight
    {
        public Light light;
        public Material material;
        public LightColor color;
    }

    

    [ContextMenu("Turn Red On")]
    public void TurnRedOn()
    {
        TurnColor(LightColor.red, true);
    }
    [ContextMenu("Turn Green On")]
    public void TurnGreenOn()
    {
        TurnColor(LightColor.green, true);
    }
    [ContextMenu("Turn Yellow On")]
    public void TurnYellowOn()
    {
        TurnColor(LightColor.yellow, true);
    }

    [ContextMenu("Reset Andon Lights")]
    public void ResetAll()
    {
        foreach(AndonLight andonLight in m_Light)
        {
            TurnColor(andonLight.color, false);
        }
    }

    [Header("Andon Settings")]
    [SerializeField] private AndonLight[] m_Light;

    private void Reset()
    {
        TurnColor(LightColor.red, true);
    }

    public void TurnColor(LightColor light, bool status)
    {
        int index = (int)light;
        Color color = m_Light[index].material.color;

        m_Light[index].light.gameObject.SetActive(status);

        float value = status ? 1f : 120f / 255f;

        if (light == LightColor.green)
        {
            color.r = 0f;
            color.g = value;
            color.b = 0f;
        }
        else if (light == LightColor.red)
        {
            color.r = value;
            color.g = 0f;
            color.b = 0f;
        }
        else if(light == LightColor.yellow)
        {
            value = status ? 1f : 60f / 255f;
            color.r = 255f;
            color.g = 255f;
            color.b = value;

        }

        m_Light[index].material.color = color;

    }

   public void BlinkLight(LightColor light, float duration, float blink, bool buzzer)
    {
        StartCoroutine(Timer(light,duration,blink, buzzer));
    }

    private IEnumerator Timer(LightColor light, float duration, float blink, bool buzzer)
    {
        float stampTime = 0f;
        int index = (int)light;
        bool flag = false;

        while(stampTime < duration)
        {
            flag = !flag;

            Buzzer(buzzer);
            m_Light[index].light.gameObject.SetActive(flag);

            yield return new WaitForSeconds(blink);
            stampTime += blink;
        }

        m_Light[index].light.gameObject.SetActive(false);
    }


    private void Buzzer(bool status)
    {

        if (status) AudioManager.Instance.Buzzer();
    }

}
