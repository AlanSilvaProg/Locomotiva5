using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{

    public Button continueButtom;

    private Image fade;

    bool faded;
    
    float count;
    private float alphaValue = 0;

    private void Start()
    {
        fade = GameObject.FindGameObjectWithTag("Fade").GetComponent<Image>();
    }

    private void Update()
    {

        if (PlayerPrefs.GetInt("Load") == 1)
        {
            continueButtom.interactable = true;
        }
        else
        {
            continueButtom.interactable = false;
        }

        if (faded)
        {
            faded = false;
            LoadScene();
        }

    }

    void LoadScene()
    {
        SceneManager.LoadScene(2);
    }

    public void ResetSaves()
    {
        PlayerPrefs.SetInt("Load", 0);
    }

    public void Continue()
    {

        StartCoroutine("FadeIn");
        
    }

    public void Creditos()
    {
        SceneManager.LoadScene("Creditos");
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
