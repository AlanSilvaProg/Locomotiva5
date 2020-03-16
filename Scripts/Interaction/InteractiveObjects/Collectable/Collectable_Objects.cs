using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SuitCaseInfo
{

    [Header("Errar a senha ativará uma chase?")]
    [Tooltip("Ativar, caso uma senha incorreta desse objeto seja aplicada e de inicio a um Chase Sequence no trem")]
    public bool activate_Chase;

    [Header("Audios da interação")]
    [Tooltip("Audios referentes a cada ação feita com o objeto, abrindo, fechando, trancada e destrancando")]
    public AudioClip clip_RightPassword;

    public float vol_Right_Password;

    public AudioClip clip_WrongPassword;

    public float vol_WrongPassword;

    [Header("Controlador do chase que será ativo")]
    [Tooltip("Objeto controlador do chase que será ativado após esta foto")]
    public GameObject thisChaseControll;

    [Header("Senha correta da maleta")]
    [Tooltip("A senha colocada aqui será comparada com a senha colocada na maleta e afetará o destino")]
    public float open_Angle;

    [Header("Angulo de abertura da maleta")]
    [Tooltip("Quando a senha for correta, irá abrir a maleta nesse angulo")]
    public string password_Right;

    [Header("Senha incorreta da maleta")]
    [Tooltip("A senha colocada aqui será comparada com a senha colocada na maleta e afetará o destino")]
    public string password_Wrong;

    [Header("isSuitCase: Objetos referentes aos botões de senha e confirmação")]
    [Tooltip("esses objetos serão usados como controles no modo investigação")]
    public GameObject[] password_ConfirmButtons;

    [Header("isSuitCase: Setas que indicaram em qual slot está")]
    [Tooltip("a seta indicara em qual slot o player está para que altere a senha do mesmo")]
    public GameObject[] arrow_Indication;

    [Header("Rotação para cada alteração de número")]
    [Tooltip("Essa rotação será aplicada na roletinha de senha para que altere o número")]
    public float rotationPassWordButtom;

    [Header("isSuitCase: Parte superior da mala")]
    [Tooltip("Parte superior da mala, que irá abrir ao descobrir a senha")]
    public GameObject suitCase_Up;
}

    [RequireComponent(typeof (AudioSource))]
public class Collectable_Objects : MonoBehaviour
{

    [Header("Nome do Objeto que será interagido")]
    [Tooltip("Necessário apenas quando for um item coletável, pois será por esse nome que ele será guardado no inventario")]
    public string obj_Name;

    [Header("Esse Objeto é a maleta com senha?")]
    [Tooltip("Quando marcado como true, o sistema de investigação irá tratar esse objeto como um unico, diferente, com um sistema de advinhação de senha")]
    public bool isSuitCase;

    [Header("Informações extras para maletas")]
    [Tooltip("Informações extras para que sejam possiveis executar todas as funções da maleta")]
    public SuitCaseInfo suitCaseInfo;

    [Header("Esse Objeto é um jornal?")]
    [Tooltip("Quando marcado como true, o sistema de investigação irá tratar esse objeto como um unico, diferente, com um sistema de Páginas")]
    public bool isJournal;

    [Header("Objeto que armazena os colliders do jornal")]
    [Tooltip("Objeto que tem como filho emptys que recebem a colisão do ray cast para verificar se está do lado certo para o efeito da foto")]
    public GameObject colliders;

    [Header("Será armazenado no inventorio?")]
    [Tooltip("Quando coletável é destruido após ser visualizado, e quando visualizavel, ele retorna para a cena após ser visualizado")]
    public bool isCollectable = true;

    [Header("Essa interação encerrará o jogo?")]
    [Tooltip("Quando Interagir com esse objeto, irá carregar a cena de fim de jogo")]
    public bool callEndGame;

    [Header("Quanto tempo a interação com esse objeto ficará desativada")]
    [Tooltip("Tempo em que esse objeto ficará com sua interação ficará desativada em caso de não ser um item coletável ou auto destruivel")]
    public float timeOff;

    [Header("Visualizar 3D ao Interagir?")]
    [Tooltip("Quando interagir com o objeto, quando verdadeiro, ele habitará a visualização 3D do objeto dentro do sistema de investigação")]
    public bool ThreeD_View;

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

    [HideInInspector]
    public bool foiAberta;

    private GameManager gmRef;

    [Header("Audio de coleta")]
    [Tooltip("Audio que tocará quando o objeto for coletado, linkar aqui")]
    public AudioClip collecting;
    [Range(0, 1)]
    public float vol_collecting = 0.7f;

    [HideInInspector]
    public AudioSource player_AudioSource;

    private bool off;
    private float count;

    [HideInInspector]
    public ChaseSystem chaseRef;

    [Header("Vai disparar alguma reação da protagonista")]
    [SerializeField] private bool IsWrongThink;
    private ReactingSoundScript reactions;

    private float wrongthinkmeter = 0;

    private void Start()
    {

        if (isSuitCase)
        {
            if (suitCaseInfo.activate_Chase)
            {
                chaseRef = suitCaseInfo.thisChaseControll.GetComponent<ChaseSystem>().chaseSystem;
            }
        }

        player_AudioSource = GameObject.FindGameObjectWithTag("Player").GetComponent<AudioSource>();
        reactions = GameObject.FindGameObjectWithTag("PlayerAudioFalas").GetComponent<ReactingSoundScript>();
        gmRef = GameManager.gmRef;

    }

    private void Update()
    {

        if (off)
        {

            gameObject.GetComponent<BoxCollider>().enabled = false;

            if (count < timeOff)
            {
                count += Time.deltaTime;
            }
            else
            {
                off = false;
                gameObject.GetComponent<BoxCollider>().enabled = true;
            }
        }
        
    }

    public void TurnOff()
    {
        off = true;
        count = 0;
    }

    public void Interaction()
    {

        if (callEndGame)
        {
            gmRef.EndGame();
            Destroy(this);
        }

        if (!isSuitCase)
        {
            if (addBadPoint)
            {
                gmRef.badEnding += badPoints;
            }

            if (addGoodPoint)
            {
                gmRef.goodEnding += goodPoints;
            }
        }


        player_AudioSource.PlayOneShot(collecting, vol_collecting);

        if (IsWrongThink)
        {
            if (wrongthinkmeter <= Random.Range(0f, 1f))
            {
                reactions.SendMessage("PlayReaction", "wrongthink");
            }
            else
            {
                wrongthinkmeter += 0.25f;
            }
        }


        if (isCollectable)
        {
            Player_Inventory.inventory.Add(obj_Name);
            if (ThreeD_View)
            {
                GameObject.Find("AnchorToLook").GetComponent<InvestigationLook>().SendMessage("SendPermission", this.gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

        }
        else if (ThreeD_View && !off)
        {
                GameObject.Find("AnchorToLook").GetComponent<InvestigationLook>().SendMessage("SendPermission", this.gameObject);
        }

    }

}