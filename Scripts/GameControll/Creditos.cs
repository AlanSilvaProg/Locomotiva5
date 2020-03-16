using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Creditos : MonoBehaviour
{

    public float timeToCredit;
    public float timeCreditDuring;

    public bool cenaCreditos;

    private Image fade;

    public float velo;

    bool faded;

    public Text credit;
    bool iniciar;
    float count;

    private float alphaValue = 0;

    // Start is called before the first frame update
    void Start()
    {

        fade = GameObject.FindGameObjectWithTag("Fade").GetComponent<Image>();
        if (cenaCreditos)
        {
            faded = iniciar = true;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
        if (!iniciar || iniciar && faded)
        {
            count += Time.deltaTime;
        }

        if (!iniciar)
        {
           if(count > timeToCredit)
            {
                iniciar = true;
                StartCoroutine("FadeIn");
                count = 0;
            }
        }

        if(iniciar && faded)
        {
            Move();
        }

        if(count > timeCreditDuring)
        {
            SceneManager.LoadScene(0);
        }



    }

    void Move()
    {
        Vector3 rect;
        rect = credit.transform.position;
        rect.y += Time.deltaTime * velo;

        credit.transform.position = rect;
    }

    IEnumerator FadeIn()
    {
        if (fade.color.a < 1)
        {
            alphaValue = fade.color.a;
            alphaValue += 0.1f;
            fade.color = new Color(fade.color.r, fade.color.g, fade.color.b, alphaValue);
            yield return new WaitForSeconds(0.1f);
            StartCoroutine("FadeIn");
        }
        else
        {
            faded = true;
        }

    }

}
