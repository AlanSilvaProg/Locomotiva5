using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(AudioSource))]
public class CutSceneDialogo : MonoBehaviour
{

    public string Scene_Name;
    
    public string[] textos;
    public AudioClip[] audios;
    private GameObject canvas;
    private Text display;
    AudioSource source;
    private int n;
    public float tempoEntreFalas;
    private float cooldown;

    private Image fade;
    private float alphaValue;
    private bool end = false;

    private bool rodando;
    [SerializeField]
    private bool TestBoolDesabilitaLoad = false;

    [SerializeField]
    private GameObject loadingtxt;


    private void Awake()
    {

        CanvasRefFetch tempRefFetch = GameObject.Find("CanvasRefFetch").GetComponent<CanvasRefFetch>();
        canvas = tempRefFetch.SUBTITLES;
        display = tempRefFetch.SUBTITLES_TEXT;

    }


    void Start()
    {
        
        fade = GameObject.FindGameObjectWithTag("Fade").GetComponent<Image>();
        source = GetComponent<AudioSource>();
        

    }

    // Update is called once per frame
    void Update()
    {

        if (rodando)
        {

            cooldown -= Time.deltaTime;

            if (cooldown <= 0)
            {
                FalaSeguinte();
                cooldown = source.clip.length + 1;
            }

        }

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

            loadingtxt.SetActive(true);
            SceneManager.LoadScene(Scene_Name);
            
        }

    }

    //Controles de qual fala sera apresentada na tela
    public void FalaSeguinte()
    {
        if (!end)
        {
            n += 1;


            if (n >= textos.Length)
            {
                if (!TestBoolDesabilitaLoad)
                {
                    end = true;
                    StartCoroutine("FadeIn");
                }
                EndConversation();
            }
            else
            {
                display.text = textos[n];
                source.clip = audios[n];
                source.Play();
            }

            cooldown = source.clip.length + 1;
        }
    }

    /*Caso for ter a opção de voltar para a fala anterior
    public void FalaAnterior ()
    {
      n -= 1;
    }*/

    //Define se player esta próximo do NPC que ira falar
    void PlayConversation()
    {
        n = 0;
        display.text = textos[n];
        source.clip = audios[n];
        rodando = true;
        canvas.SetActive(true);
        source.Play();
        cooldown = source.clip.length + 1;

    }

    //Reseta fala quando jogador se afasta do NPC
    void EndConversation()
    {

        canvas.SetActive(false);
        n = 0;
        rodando = false;

    }
    public void Act()
    {

        PlayConversation();

    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
