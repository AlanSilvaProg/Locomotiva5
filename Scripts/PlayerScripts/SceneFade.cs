using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneFade : MonoBehaviour
{

    public float fadeTime;
    private Image fade;
    private float alphaValue;

    // Start is called before the first frame update
    void Start()
    {
        fade = GameObject.FindGameObjectWithTag("Fade").GetComponent<Image>();
        StartCoroutine(FadeCanvas(fade, fade.color.a, 0, fadeTime));
    }

    public IEnumerator FadeCanvas(Image cg, float start, float end, float lerpTime)
    {
        float timeStartedLerping = Time.time;
        float timeScinceStarted = Time.time - timeStartedLerping;
        float percentageComplete = timeScinceStarted / lerpTime;

        while (true)
        {
            timeScinceStarted = Time.time - timeStartedLerping;
            percentageComplete = timeScinceStarted / lerpTime;

            float currentValue = Mathf.Lerp(start, end, percentageComplete);

            Color color;
            color = cg.color;

            color.a = currentValue;

            cg.color = color;

            if (percentageComplete >= 1) break;

            yield return new WaitForEndOfFrame();
        }
    }

}
