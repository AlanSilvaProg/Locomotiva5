using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cam_Interactive : MonoBehaviour
{
    [Header("Tocar apenas uma vez")]
    public bool oneShot;

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
    private AudioSource source;
    private int n;
    public float tempoEntreFalas;
    private float cooldown;

    private bool rodando = false;

    private int controll = 0;


   
    private void Awake()
    {
        
        CanvasRefFetch tempRefFetch = GameObject.Find("CanvasRefFetch").GetComponent<CanvasRefFetch>();
        canvas = tempRefFetch.SUBTITLES;
        display = tempRefFetch.SUBTITLES_TEXT;

    }

    void Start()
    {

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

        if (oneShot)
        {
            controll ++;
        }

        if (addBadPoint)
        {
            gmRef.badEnding++;
        }

        if (addGoodPoint)
        {
            gmRef.goodEnding++;
        }

    }
    public void Act()
    {

        if (oneShot && controll == 0 && !rodando)
        {
            PlayConversation();
        }
        else if(!oneShot && !rodando)
        {
            PlayConversation();
        }

    }

}
