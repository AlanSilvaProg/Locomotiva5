using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class DialogSceneStart : MonoBehaviour
{
    public bool isTutorial;

    public string[] textos;
    public AudioClip[] audios;
    private GameObject canvas;
    private Text display;
    AudioSource source;
    private CharacterController character_Ctrl;
    private FirstPersonController fps_Controller;
    [SerializeField] private Player_Interact player_interact;
    private int n;
    private float tempoParaFalas;
    private float cooldown;
    private bool set;

    private Image fade;
    private float alphaValue;
    private bool end = false;

    private void Awake()
    {
        CanvasRefFetch tempRefFetch = GameObject.Find("CanvasRefFetch").GetComponent<CanvasRefFetch>();
        canvas = tempRefFetch.SUBTITLES;
        display = tempRefFetch.SUBTITLES_TEXT;
    }

    // Start is called before the first frame update
    void Start()
    {
        
        source = GameObject.FindGameObjectWithTag("PlayerAudioFalas").GetComponent<AudioSource>();
        n = 0;
        display.text = textos[n];
        source.clip = audios[n];
        set = false;
        fps_Controller = GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonController>();
        character_Ctrl = FindObjectOfType(typeof(CharacterController)) as CharacterController;
        fade = GameObject.FindGameObjectWithTag("Fade").GetComponent<Image>();
        canvas.SetActive(true);
        tempoParaFalas = 0;

    }

    // Update is called once per frame
    void Update()
    {
        if (!set)
        {
            tempoParaFalas -= Time.deltaTime;
        }
        else
        {
            cooldown -= Time.deltaTime;
        }

        if (tempoParaFalas <= 0 && set == false && !end)
        {
            
            if (n <= 0)
            {

                display.text = textos[n];
                source.clip = audios[n];
                source.Play();
                cooldown = source.clip.length + 1;
                character_Ctrl.enabled = false;
                fps_Controller.enabled = false;
                player_interact.enabled = false;
                
            }

            set = true;
        }

        if (set == true && cooldown <= 0)
        {
            
            n += 1;

            if (n >= audios.Length)
            {
                if (isTutorial)
                {
                    CamBoxTutorial.iniciarTutorial = true;
                    canvas.SetActive(false);
                    set = false;
                    character_Ctrl.enabled = true;
                    fps_Controller.enabled = true;
                  player_interact.enabled = true;
                    end = true;
                    StartCoroutine("FadeOut");
                    tempoParaFalas = 0.2f;
                }
                else
                {
                    canvas.SetActive(false);
                    set = false;
                    character_Ctrl.enabled = true;
                    fps_Controller.enabled = true;
                    player_interact.enabled = true;
                    end = true;
                    StartCoroutine("FadeOut");
                    tempoParaFalas = 0.2f;
                }

            }
            else if (n < audios.Length)
            {
                display.text = textos[n];
                source.clip = audios[n];
                source.Play();

            }

            cooldown = source.clip.length + 1;

        }
    }

    IEnumerator FadeOut()
    {
        if (fade.color.a > 0)
        {
            alphaValue = fade.color.a;
            alphaValue -= 0.1f;
            fade.color = new Color(fade.color.r, fade.color.g, fade.color.b, alphaValue);
            yield return new WaitForSeconds(0.1f);
            StartCoroutine("FadeOut");
        }
        else
        {
            Destroy(this);
        }

    }

}
