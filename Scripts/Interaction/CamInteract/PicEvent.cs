using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class PicEvent : MonoBehaviour
{

    [Header("Será adicionado pontos para um final ruim?")]
    [Tooltip("Quando interagir com o objeto, quando verdadeiro, irá adicionar pontos para um final ruim")]
    public bool addBadPoint;

    [Header("Pontos que seram adicionados ao contador de final ruim?")]
    [Tooltip("Quando interagir com o objeto, irá adicionar esta quantidade de pontos para um final ruim")]
    public int badPoints;

    [Header("Será adicionado pontos para um final bom?")]
    [Tooltip("Quando interagir com o objeto, quando verdadeiro, irá adicionar pontos para um final bom")]
    public bool addGoodPoint;

    [Header("Pontos que seram adicionados ao contador de final bom?")]
    [Tooltip("Quando interagir com o objeto, irá adicionar esta quantidade de pontos para um final bom")]
    public int goodPoints;

    [Header("Foto necessita de Flash para disparar Eventos?")]
    [Tooltip("Ativar esse check box, caso seja necessario que esses eventos sejam disparados apenas quando for tirada uma foto com flash")]
    public bool need_Flash;

    [Header("Foto necessita de Flash para disparar Eventos?")]
    [Tooltip("Ativar esse check box, caso seja necessario que esses eventos sejam disparados apenas quando for tirada uma foto com flash")]
    public bool feedback_BadFlash;

    [Header("Uma foto irá ativar uma Chase Sequence?")]
    [Tooltip("Ativar, caso uma foto desse objeto de inicio a um Chase Sequence no trem")]
    public bool activate_Chase;

    [Header("Será carregada uma cena após a foto?")]
    [Tooltip("Após tirar uma foto será carregada uma nova cena")]
    public bool scene_Change;

    [Header("Controlador do chase que será ativo")]
    [Tooltip("Objeto controlador do chase que será ativado após esta foto")]
    public GameObject thisChaseControll;

    private ChaseSystem chaseRef;

    [Header("Tempo para carregar nova cena")]
    [Tooltip("Tempo que irá levar para carregar uma nova cena após a foto")]
    public float timeToLoad;

    [Header("Nome da Cena que será carregada")]
    [Tooltip("Nome da cena que irá carregar após tirar a foto do objeto")]
    public string scene_Name;

    [Header("Objetos que serão Ativados após a foto")]
    [Tooltip("Após tirar uma foto todos os objetos dentro do array serão ativados na cena do jogo")]
    public GameObject[] enable;

    [Header("Objetos que serão Desativados após a foto")]
    [Tooltip("Após tirar uma foto todos os objetos dentro do array serão desativados na cena do jogo")]
    public GameObject[] disable;

    [Header("Será disparado algum som?")]
    [Tooltip("Ativar caso seja necessario disparar um ou mais sons a partir dos eventos dessa foto")]
    public bool shoot_Sound;

    [Header("Clips de sons que serão disparados")]
    [Tooltip("Todos os clips de audio linkados aqui serão disparados ao mesmo tempo, caso a variavel shoot_Sound seja true")]
    public AudioClip[] clip_Sound;

    [Header("Volume dos sons que serão disparados")]
    [Tooltip("O volume em que cada som será emitido para o jogador tendo como ponto de origem do audio o audio source desse objeto")]
    public float clip_Vol;

    [Header("Destrancar uma porta após tirar a foto")]
    [Tooltip("Irá destrancar uma porta após disparar o evento da foto")]
    public bool unloock_Door;

    [Header("Porta que será destrancada")]
    [Tooltip("porta que irá ser destrancada após a foto")]
    public GameObject door;

    [Header("Algum Objeto irá receber interação após essa foto?")]
    [Tooltip("Todos os objetos linkados aqui, irão receber um SendMensage de Interaction ou Act, executando suas respectivas funções... a mensagem será enviada em base da tag... Tag 'PlayerInteract' irá receber mensage Interaction() e Tag 'PhotoInteracts' irá receber a mensagem Act()")]
    public GameObject[] send_Interaction_Mensage;

    private GameManager gmRef;

    private AudioSource source_Player;
    private AudioSource source_Pic;

    private Image fade;

    bool faded;

    private float alphaValue = 0;

    [Header("Vai disparar alguma reação da protagonista")]
    [SerializeField] ReactingSoundScript.ReactionType reactionTrigger = ReactingSoundScript.ReactionType.none ;
    private ReactingSoundScript reactions;


    private void Start()
    {

        fade = GameObject.FindGameObjectWithTag("Fade").GetComponent<Image>();

        if (activate_Chase)
        {
            chaseRef = thisChaseControll.GetComponent<ChaseSystem>().chaseSystem;
        }
        source_Pic = GetComponent<AudioSource>();
        source_Player = GameObject.FindGameObjectWithTag("Player").GetComponent<AudioSource>();
        reactions = GameObject.FindGameObjectWithTag("PlayerAudioFalas").GetComponent<ReactingSoundScript>();
        gmRef = GameManager.gmRef;

    }

    //Função chamada ao tirar uma foto desse objeto
    public void Act()
    {
        //Verificando se há a necessidade de usar flash para disparar os determinados eventos
        if (need_Flash)
        {

            if (CamPhotoManager.flash_Permission) //Verificando se o flash está ativo
            {

                Calls(); // Função em que dispara todos os eventos resultantes da foto

            }
            else if (!CamPhotoManager.flash_Permission) //Verificando se o flash está desativado
            {

                return; // encerrando a função

            }

        }
        else // Se não é necessário o flash, apenas será realizado os eventos sem nenhuma verificação adicional
        {

            Calls(); // Função em que dispara todos os eventos resultantes da foto

        }

    }

// Função em que dispara todos os eventos resultantes da foto
    void Calls()
    {

        if (addBadPoint)
        {
            gmRef.badEnding += badPoints;
        }

        if (addGoodPoint)
        {
            gmRef.goodEnding += goodPoints;
        }

        if (scene_Change && !faded) // Verificando se o evento de mudar de cena está marcado
        {
            faded = true;
            StartCoroutine("PreFade"); // Chamando a coroutine responsável pela troca de scene

        }
        else // Se não for necessário a troca de cena, será executado todos eventos adicionais
        {

            if (feedback_BadFlash)
            {
                GameObject.FindGameObjectWithTag("FeedBackFlash").GetComponent<Animator>().SetTrigger("FeedbackIn");
            }

            if (activate_Chase && chaseRef != null) // verificando se o Chase Sequence será ativado
            {
                // Procurando pelo ChaseSystem e chamando a respectiva função para ativar o chase sequence
                chaseRef.SendMessage("ChaseSetActive", true);

            }

            if (shoot_Sound) // Verificando se será disparado algum som após a foto ser tirada
            {

                for (int i = 0; i < clip_Sound.Length; i++)
                {

                    source_Pic.PlayOneShot(clip_Sound[i], clip_Vol); // Disparando som por som com um FOR

                }

            }

            if (unloock_Door)
            {
                door.SendMessage("UnloockDoor");
            }

            if (send_Interaction_Mensage.Length > 0) // Verificando se tem objetos para mandar um send mensage para triggar eventos
            {

                for (int i = 0; i < send_Interaction_Mensage.Length; i++)
                {

                    if (send_Interaction_Mensage[i].tag != "PhotoInteracts") // Verificando tag para identificar qual SendMensage enviar
                    {
                        send_Interaction_Mensage[i].SendMessage("Interaction"); // Triggando eventos do objeto
                    }
                    else
                    {
                        send_Interaction_Mensage[i].SendMessage("Act"); // Triggando Eventos do objeto
                    }

                }

            }

            if (enable.Length > 0) // Verificando se tem objetos setados para serem ativos após tirar a foto
            {
                for (int i = 0; i < enable.Length; i++)
                {

                    enable[i].SetActive(true); // Ativando objeto por objeto em base de um For

                }
            }

            if (disable.Length > 0) // Verificando se tem objetos setados para serem desativados após tirar a foto
            {
                for (int i = 0; i < disable.Length; i++)
                {

                    disable[i].SetActive(false); // Desativando objeto por objeto em base de um For

                }
            }

            if(reactionTrigger != ReactingSoundScript.ReactionType.none)
            {
                switch (reactionTrigger)
                {
                    case ReactingSoundScript.ReactionType.surpreso:
                        reactions.SendMessage("PlayReaction", "surpreso");
                    break;
                    case ReactingSoundScript.ReactionType.trancado:
                    reactions.SendMessage("PlayReaction", "trancado");
                    break;
                }
            }

        }

    }

    IEnumerator PreFade()
    {
        yield return new WaitForSeconds(timeToLoad);
        StartCoroutine("FadeIn");
    }

    IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(1);
        if (fade.color.a < 1)
        {
            alphaValue = fade.color.a;
            alphaValue += 0.1f;
            fade.color = new Color(fade.color.r, fade.color.g, fade.color.b, alphaValue);
            yield return new WaitForSeconds(0.1f);
            StartCoroutine("FadeIn");
            print("I am Fading");
        }
        else
        {
            StartCoroutine("SceneChange");
        }

    }

    /// Coroutine responsável de Carregar uma nova cena
    IEnumerator SceneChange()
    {
        print("I am Loading");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(scene_Name);
        print("I Should have loaded");

    }

}