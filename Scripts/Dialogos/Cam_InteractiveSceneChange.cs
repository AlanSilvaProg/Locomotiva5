using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Cam_InteractiveSceneChange : MonoBehaviour
{

    public string Scene_Name;

    [Header("Será adicionado pontos para um final ruim?")]
    [Header("Quando interagir com o objeto, quando verdadeiro, irá adicionar pontos para um final ruim")]
    public bool addBadPoint;

    [Header("Pontos que seram adicionados ao contador de final ruim?")]
    [Header("Quando interagir com o objeto, irá adicionar esta quantidade de pontos para um final ruim")]
    public int badPoints;

    [Header("Será adicionado pontos para um final bom?")]
    [Header("Quando interagir com o objeto, quando verdadeiro, irá adicionar pontos para um final bom")]
    public bool addGoodPoint;

    [Header("Pontos que seram adicionados ao contador de final bom?")]
    [Header("Quando interagir com o objeto, irá adicionar esta quantidade de pontos para um final bom")]
    public int goodPoints;

    private GameManager gmRef;

    public string[] textos;
    public AudioClip[] audios;
    private GameObject canvas;
    private Text display;
    AudioSource source;
    private int n;
    public float tempoEntreFalas;
    private float cooldown;

    private bool rodando;

    private Image fade;

    bool faded;

    private float alphaValue = 0;


    private void Awake()
    {
        
        CanvasRefFetch tempRefFetch = GameObject.Find("CanvasRefFetch").GetComponent<CanvasRefFetch>();
        canvas = tempRefFetch.SUBTITLES;
        display = tempRefFetch.SUBTITLES_TEXT;

    }

    void Start()
    {

        fade = GameObject.FindGameObjectWithTag("Fade").GetComponent<Image>();
        source = GameObject.FindGameObjectWithTag("PlayerAudioFalas").GetComponent<AudioSource>();
        gmRef = GameManager.gmRef;

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
            }

        }

        

    }

    //Controles de qual fala sera apresentada na tela
    public void FalaSeguinte()
    {

        n += 1;


        if (n >= textos.Length)
        {
            if (!faded)
            {
                StartCoroutine("FadeIn");
                faded = true;
            }
        }
        else
        {
            display.text = textos[n];
            source.clip = audios[n];
            source.Play();
        }

        cooldown = source.clip.length + 1;

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
        rodando = false;
        n = 0;

        if (addBadPoint)
        {
            gmRef.badEnding++;
        }

        if (addGoodPoint)
        {
            gmRef.goodEnding++;
        }

        SceneManager.LoadScene(Scene_Name);

    }
    public void Act()
    {

        PlayConversation();

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
            EndConversation();
        }

    }


}