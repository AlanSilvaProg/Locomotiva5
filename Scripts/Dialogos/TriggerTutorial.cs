using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TriggerTutorial : MonoBehaviour
{
    
    private GameManager gmRef;

    public string[] textos;
    public AudioClip[] audios;
    private GameObject canvas;
    private Text display;
    AudioSource source;
    private int n;
    public float tempoEntreFalas;
    private float cooldown;

    private bool rodando = false;

    private int controll = 0;

    bool isTutorial = true;


    private void Awake()
    {

        CanvasRefFetch tempRefFetch = GameObject.Find("CanvasRefFetch").GetComponent<CanvasRefFetch>();
        canvas = tempRefFetch.TUTORIAL;
        display = tempRefFetch.TUTORIAL_TEXT;

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

        cooldown = tempoEntreFalas;

    }
    
    void PlayConversation()
    {

        n = 0;
        display.text = textos[n];
        source.clip = audios[n];
        rodando = true;
        canvas.SetActive(true);
        source.Play();
        cooldown = tempoEntreFalas;

    }

    //Reseta fala quando jogador se afasta do NPC
    void EndConversation()
    {

        canvas.SetActive(false);
        rodando = false;
        n = 0;

        Destroy(this);

    }

    private void OnTriggerEnter(Collider other)
    {

        if(other.gameObject.tag == "Player" && !rodando)
        {
            PlayConversation();
        }

    }

}