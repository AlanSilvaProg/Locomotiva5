using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Signal
{
    POSITIVE,
    NEGATIVE
}

public enum OpenType
{
    ANGULAR,
    SLIDER,
    TELEPORT
}

public enum Axis
{
    X,
    Y,
    Z
}

public enum State
{
   
    MOVENDO,
    PARADO

}

[RequireComponent(typeof(AudioSource))]
public class Open_Close : MonoBehaviour
{
   
    private State objectState = State.PARADO;

    private bool resetando;

    private AudioSource Source_Openable;

    public Text loading;

    [Header("Tipo de Abertura do objeto")]
    [Tooltip("Angular para objetos como portas tradicionais e baus que abrem com uma rotação, Slider para aberturas laterais e deslizantes com movimentação e Teleport para portas com fade")]

    public OpenType openType;

    [Header("Ativar para quando é necessário uma chave para abrir")]
    [Tooltip("Quando ativado é necessário encontrar uma chave para abrir este objeto ou qualquer outro tipo de coisa que será usado para abri-lo")]
    public bool key_Need;

    [Header("Setar se está trancada ou não")]
    [Tooltip("Ativar caso a porta esteja trancada, precisando de chave ou não")]
    public bool isLocked = false;

    [Header("Para itens que precisa de Chave: Nome da Chave")]
    [Tooltip("Colocar aqui o nome da chave que será necessária para abrir este objeto ou porta")]
    public string key_Name;


    [Header("Tempo de inatividade da porta para interação")]
    [Tooltip("Toda e qualquer interação com portas ou objetos rotacionaveis ficaram inativos por esse tempo após uma interação... Exemplo: Ao abrir uma porta, ela não poderá ser fechada por Determinado tempo, esse tempo é referente a essa variavel")]
    public float interaction_TimeOff; // Tempo em que a porta não poderá abrir nem fechar ( tempo entre cada executação do ato )

    public float secondsToClose;

    [Header("Informações e valores especificos de portas Angulares")]
    [Tooltip("Todas as informações de valores e ações dentro do tipo de porta Angular")]
    public AngularDoor info_AngularDoor;


    [Header("Informações e valores especificos de portas Slider's")]
    [Tooltip("Todas as informações de valores e ações dentro do tipo de porta Slider")]
    public SliderDoor info_SliderDoor;


    [Header("Informações e valores especificos de portas de Teleport")]
    [Tooltip("Todas as informações de valores e ações dentro do tipo de porta de Teleport")]
    public TeleportDoor info_TeleportDoor;

    private Quaternion rotate;
    
    private float angle; // Angulo da próxima rotação

    private Transform posicao;

    private float positionOriginal; // Posição da porta no eixo X (Open Type : Slider)
    private float actualPosition; // Variavel que será alterada e manterá informações da atual posição da porta
    private float valueToMove; // Valor que ela se moverá para o lado quando aberta (Open Type : Slider)
    private Vector3 destiny; // Destino da porta


    private Image fade;
    private GameObject player;

    private bool atravessando;

    private float alphaValue;
    
    private Vector2 wagonAndEntry;

    private Transform posicaoChase;

    private bool ignoreLoock;
    private bool openWithIgnoreLoock;

    private float count;

    private GameManager gmRef;

    private ReactingSoundScript sn_scr;
    [SerializeField] [Range(0,1)] private float reactionFalaProb = 0.25f;
    
    //Eu odeio muito o Arion por ter falado pra tirar as pausas dos sons

    [Header("Variavel para fazer testes, ativar in-game para testar")]
    public bool abrir; // Variavel que controla quando a ação de abrir deve ser executada
    

    private void Start()
    {

        gmRef = GameManager.gmRef;

        sn_scr = GameObject.FindGameObjectWithTag("PlayerAudioFalas").GetComponent<ReactingSoundScript>();

        if (info_TeleportDoor.thisChaseControll != null)
        {
            info_TeleportDoor.chaseRef = info_TeleportDoor.thisChaseControll.GetComponent<ChaseSystem>().chaseSystem;
        }

        fade = GameObject.FindGameObjectWithTag("Fade").GetComponent<Image>();
        player = GameObject.FindGameObjectWithTag("Player");

        Source_Openable = GetComponent<AudioSource>();

        //Guardando o valor do eixo do objeto para retornar depois
        if (openType == OpenType.SLIDER)
        {
            if (info_SliderDoor.axisToSlide == Axis.X)
            {
                positionOriginal = transform.localPosition.x;
            }
            else if (info_SliderDoor.axisToSlide == Axis.Y)
            {
                positionOriginal = transform.localPosition.y;
            }
            else if (info_SliderDoor.axisToSlide == Axis.Z)
            {
                positionOriginal = transform.localPosition.z;
            }
        }
        //Guardando o valor do eixo do objeto para retornar depois
        if (openType == OpenType.ANGULAR)
        {
            if (info_AngularDoor.axisToRotate == Axis.X)
            {
                positionOriginal = transform.eulerAngles.x;
            }
            else if (info_AngularDoor.axisToRotate == Axis.Y)
            {
                positionOriginal = transform.eulerAngles.y;
            }
            else if (info_AngularDoor.axisToRotate == Axis.Z)
            {
                positionOriginal = transform.eulerAngles.z;
            }
        }

        //Atualizando próxima posição
        valueToMove = positionOriginal;

        angle = positionOriginal;

    }

    private void Update()
    {

        if (isLocked)
        {
            if (gmRef.unloockAll)
            {
                isLocked = false;
            }
        }

        if(openWithIgnoreLoock && count < secondsToClose)
        {

            count += Time.deltaTime;

            if(count > secondsToClose)
            {
                if(openType == OpenType.ANGULAR)
                {
                    if (angle == 0)
                    {
                        Source_Openable.PlayOneShot(info_AngularDoor.opening, info_AngularDoor.vol_opening);
                    }
                    else
                    {
                        Source_Openable.PlayOneShot(info_AngularDoor.closing, info_AngularDoor.vol_closing);
                    }

                    abrir = true;
                    openWithIgnoreLoock = false;
                    
                }
                else if (openType == OpenType.SLIDER)
                {

                    if (valueToMove == positionOriginal)
                    {
                        Source_Openable.PlayOneShot(info_SliderDoor.sl_Closing, info_SliderDoor.vol_Sl_closing);
                    }
                    else
                    {
                        Source_Openable.PlayOneShot(info_SliderDoor.sl_Opening, info_SliderDoor.vol_Sl_opening);
                    }

                    abrir = true;
                    openWithIgnoreLoock = false;

                }
            }
        }

        if (abrir && objectState == State.PARADO)
        {

            if (openType == OpenType.ANGULAR)
            {

                RefreshInformations();
                abrir = false;
                if (ignoreLoock)
                {
                    ignoreLoock = false;
                    openWithIgnoreLoock = true;
                }

            }
            else
            {

                RefreshInformationsSlider();
                abrir = false;
                if (ignoreLoock)
                {
                    ignoreLoock = false;
                    openWithIgnoreLoock = true;
                }

            }

        }

       
        if(objectState == State.MOVENDO && openType == OpenType.ANGULAR)
        {

            if (info_AngularDoor.axisToRotate == Axis.X)
            {

                Quaternion rotate = Quaternion.Euler(angle, transform.eulerAngles.y, transform.eulerAngles.z);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotate, info_AngularDoor.open_Velocity);

                if(angle == transform.eulerAngles.x)
                {
                    objectState = State.PARADO;
                }

            }
            else if (info_AngularDoor.axisToRotate == Axis.Y)
            {

                Quaternion rotate = Quaternion.Euler(transform.eulerAngles.x, angle, transform.eulerAngles.z);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotate, info_AngularDoor.open_Velocity);
                if (angle == transform.eulerAngles.y)
                {
                    objectState = State.PARADO;
                }

            }
            else if (info_AngularDoor.axisToRotate == Axis.Z)
            {

                Quaternion rotate = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, angle);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotate, info_AngularDoor.open_Velocity);
                if (angle == transform.eulerAngles.z)
                {
                    objectState = State.PARADO;
                }

            }

        }

        if (objectState == State.MOVENDO && openType == OpenType.SLIDER)
        {

            transform.localPosition = Vector3.MoveTowards(transform.localPosition, destiny, Time.deltaTime * 2 * info_SliderDoor.multiplicadorDeAbertura);

        }

    }

    public  void Interaction()
    {
        
        ////////////////////////// OPEN TYPE : ANGULAR //////////////////////////////////////
        if (openType == OpenType.ANGULAR && objectState == State.PARADO || openType == OpenType.ANGULAR && ignoreLoock)
        {
            if (isLocked && !ignoreLoock)
            {

                if (info_AngularDoor.openTwoDoor)
                {
                    info_AngularDoor.Door.SendMessage("IgnoreLoockAndOpen", info_AngularDoor.timeToClose);
                    IgnoreLoockAndOpen(info_AngularDoor.timeToClose);
                    return;
                }

                if (key_Need)
                {
                    for (int i = 0; i < Player_Inventory.inventory.Count; i++)
                    {
                        if (key_Name == Player_Inventory.inventory[i])
                        {

                            UnloockDoor();

                            abrir = true;

                            if (info_AngularDoor.openTwoDoor)
                            {
                                info_AngularDoor.Door.SendMessage("IgnoreLoockAndOpen", info_AngularDoor.timeToClose);
                            }

                            Player_Inventory.inventory.Remove(Player_Inventory.inventory[i]);

                            return;

                        }
                    }
                }

                Source_Openable.PlayOneShot(info_AngularDoor.locked, info_AngularDoor.vol_locked);
                ReactionTriggerLocked();

            }
            else if(ignoreLoock || !isLocked)
            {

                if (angle == positionOriginal)
                {

                    Source_Openable.PlayOneShot(info_AngularDoor.opening, info_AngularDoor.vol_opening);

                }
                else
                {
                    Source_Openable.PlayOneShot(info_AngularDoor.closing, info_AngularDoor.vol_closing);
                }

                abrir = true;

            }
        }
        

        ///////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////// OPEN TYPE : SLIDER /////////////////////////////////////////

        if (openType == OpenType.SLIDER && objectState == State.PARADO || openType == OpenType.SLIDER && ignoreLoock)
        {
            if (isLocked && !ignoreLoock)
            {

                if (info_SliderDoor.openTwoDoor)
                {
                    info_SliderDoor.Door.SendMessage("IgnoreLoockAndOpen", info_SliderDoor.timeToClose);
                    IgnoreLoockAndOpen(info_SliderDoor.timeToClose);
                    return;
                }

                if (key_Need)
                {

                    for (int i = 0; i < Player_Inventory.inventory.Count; i++)
                    {
                        if (key_Name == Player_Inventory.inventory[i])
                        {

                            UnloockDoor();

                            abrir = true;

                            Player_Inventory.inventory.Remove(Player_Inventory.inventory[i]);

                            return;

                        }
                    }
                }

                Source_Openable.PlayOneShot(info_SliderDoor.sl_Locked, info_SliderDoor.vol_Sl_locked);
                ReactionTriggerLocked();
            }
            else if (ignoreLoock || !isLocked)
            {
                if (valueToMove == positionOriginal)
                {
                    Source_Openable.PlayOneShot(info_SliderDoor.sl_Closing, info_SliderDoor.vol_Sl_closing);
                }
                else
                {

                    Source_Openable.PlayOneShot(info_SliderDoor.sl_Opening, info_SliderDoor.vol_Sl_opening);

                }

                abrir = true;
            }
        }


        ////////////////////////// OPEN TYPE : TELEPORT //////////////////////////////////////
        if (openType == OpenType.TELEPORT)
        {
            if (info_TeleportDoor.chaseRef == null || info_TeleportDoor.chaseRef != null && info_TeleportDoor.chaseRef.chaseSystemState == ChaseStates.OFF)
            {
                if (isLocked)
                {
                    if (key_Need)
                    {
                        for (int i = 0; i < Player_Inventory.inventory.Count; i++)
                        {
                            if (key_Name == Player_Inventory.inventory[i])
                            {

                                UnloockDoor();

                                Player_Inventory.inventory.Remove(Player_Inventory.inventory[i]);

                                if (!atravessando)
                                {
                                    atravessando = true;
                                    Source_Openable.PlayOneShot(info_TeleportDoor.Tp_Opening, info_TeleportDoor.vol_Tp_opening);
                                    Player_Interact.playerStates = PlayerStates.INTERAGINDO;

                                    StartCoroutine("FadeIn");
                                }

                                return;

                            }
                        }
                    }

                    Source_Openable.PlayOneShot(info_TeleportDoor.Tp_Locked, info_TeleportDoor.vol_Tp_locked);
                    ReactionTriggerLocked();


                }
                else if (!atravessando)
                {

                    atravessando = true;
                    Source_Openable.PlayOneShot(info_TeleportDoor.Tp_Opening, info_TeleportDoor.vol_Tp_opening);
                    Player_Interact.playerStates = PlayerStates.INTERAGINDO;

                    StartCoroutine("FadeIn");

                }
            }
            else
            {
                if (!info_TeleportDoor.partOfChase && info_TeleportDoor.chaseRef.chaseSystemState == ChaseStates.ON || info_TeleportDoor.partOfChase && info_TeleportDoor.chaseRef.chaseSystemState == ChaseStates.ON)
                {

                    int randomWagon;
                    randomWagon = (int)Random.Range(0, info_TeleportDoor.chaseRef.chaseInfo.wagonsDestiny.Length);
                    int randomEntry = 0;

                    if (randomWagon == wagonAndEntry.x)
                    {
                        randomWagon++;
                        if (randomWagon >= info_TeleportDoor.chaseRef.chaseInfo.wagonsDestiny.Length)
                        {
                            randomWagon = 0;
                        }
                    }

                    wagonAndEntry = new Vector2(randomWagon, randomEntry);

                    posicaoChase = info_TeleportDoor.chaseRef.chaseInfo.wagonsDestiny[(int)wagonAndEntry.x].wagonDoors[(int)wagonAndEntry.y];

                    Vector2 toMensage;
                    toMensage = new Vector2(randomWagon, randomEntry);

                    info_TeleportDoor.chaseRef.SendMessage("NewWagon", toMensage);

                    if (!atravessando)
                    {
                        atravessando = true;
                        Source_Openable.PlayOneShot(info_TeleportDoor.Tp_Opening, info_TeleportDoor.vol_Tp_opening);
                        Player_Interact.playerStates = PlayerStates.INTERAGINDO;

                        StartCoroutine("FadeIn");
                    }
                }
            }
        }
        

        /////////////////////////////////////////////////////////////////////////////////////////////

    }

    public void Teleport()
    {

        PlayerPrefs.SetInt("SpawnOnTrain", 1);

#if UNITY_EDITOR
        PlayerPrefs.SetInt("SpawnOnTrain", 0);
#endif

        if (!info_TeleportDoor.partOfChase && info_TeleportDoor.chaseRef != null)
        {
            info_TeleportDoor.chaseRef.lastNonChaseWagon = info_TeleportDoor.posicao;
        }

        if (info_TeleportDoor.chaseRef == null || info_TeleportDoor.chaseRef != null && info_TeleportDoor.chaseRef.chaseSystemState != ChaseStates.ON)
        {

            if (info_TeleportDoor.chaseRef != null)
            {
                info_TeleportDoor.chaseRef.lastNonChaseWagon = info_TeleportDoor.posicao;
            }

            player.transform.rotation = info_TeleportDoor.posicao.rotation;
            player.transform.position = info_TeleportDoor.posicao.position;
            Player_Interact.playerStates = PlayerStates.NOTHING;

            return;

        }
        else if (info_TeleportDoor.chaseRef.chaseSystemState == ChaseStates.ON)
        {
            if (!info_TeleportDoor.partOfChase)
            {

                info_TeleportDoor.chaseRef.lastNonChaseWagon = info_TeleportDoor.posicao;
                player.transform.rotation = posicaoChase.rotation;
                player.transform.position = posicaoChase.position;
                Player_Interact.playerStates = PlayerStates.NOTHING;
                info_TeleportDoor.chaseRef.wagonCount = 0;
                return;

            }
            else if (info_TeleportDoor.partOfChase)
            {

                if (info_TeleportDoor.chaseRef.wagonCount < info_TeleportDoor.chaseRef.chaseInfo.amountOfWagon)
                {

                    info_TeleportDoor.chaseRef.wagonCount++;
                    player.transform.rotation = posicaoChase.rotation;
                    player.transform.position = posicaoChase.position;
                    Player_Interact.playerStates = PlayerStates.NOTHING;
                    return;

                }
                else
                {
                    if (info_TeleportDoor.chaseRef.chaseInfo.specificDestination_OnEndChase)
                    {

                        player.transform.rotation = info_TeleportDoor.chaseRef.chaseInfo.destinationOnEndChase.rotation;
                        player.transform.position = info_TeleportDoor.chaseRef.chaseInfo.destinationOnEndChase.position;
                        info_TeleportDoor.chaseRef.SendMessage("ChaseSetActive", false);
                        Player_Interact.playerStates = PlayerStates.NOTHING;
                        return;

                    }
                    else
                    {

                        player.transform.rotation = info_TeleportDoor.chaseRef.lastNonChaseWagon.rotation;
                        player.transform.position = info_TeleportDoor.chaseRef.lastNonChaseWagon.position;
                        info_TeleportDoor.chaseRef.SendMessage("ChaseSetActive", false);
                        Player_Interact.playerStates = PlayerStates.NOTHING;
                        return;

                    }
                    
                }

            }
        }

    }


    private void ReactionTriggerLocked()
    {
        if (Random.Range(0f, 1f) <= reactionFalaProb)
        {
            sn_scr.SendMessage("PlayReaction", "trancado");
            reactionFalaProb -= 0.3f;
            if (reactionFalaProb < 0) { reactionFalaProb = 0; }
        }

    }

    public void IgnoreLoockAndOpen(float timeToClose)
    {
        
        secondsToClose = timeToClose;
        openWithIgnoreLoock = false;
        ignoreLoock = true;
        count = 0;
        Interaction();

    }

    public void UnloockDoor()
    {

        ////////////////////////// OPEN TYPE : ANGULAR //////////////////////////////////////
        if (openType == OpenType.ANGULAR)
        {

            isLocked = false;
            Source_Openable.PlayOneShot(info_AngularDoor.unlocked, info_AngularDoor.vol_unlocked);

            return;

        }

        ////////////////////////// OPEN TYPE : SLIDER /////////////////////////////////////////

        if (openType == OpenType.SLIDER)
        {

            isLocked = false;
            Source_Openable.PlayOneShot(info_SliderDoor.sl_Unlocked, info_SliderDoor.vol_SL_unlocked);

            return;

        }


        ////////////////////////// OPEN TYPE : TELEPORT //////////////////////////////////////
        if (openType == OpenType.TELEPORT)
        {

            isLocked = false;
            Source_Openable.PlayOneShot(info_TeleportDoor.Tp_Unlocked, info_TeleportDoor.vol_Tp_unlocked);
            return;

        }

    }

    void RefreshInformationsSlider()
    {

        StartCoroutine("ResetProcess");

        //Atualizando próxima posição
        if (valueToMove == positionOriginal)
        {
            if (info_SliderDoor.reverse)
            {
                if (info_SliderDoor.axisToSlide == Axis.X)
                {
                    valueToMove = transform.localPosition.x + transform.lossyScale.z;
                }
                else if (info_SliderDoor.axisToSlide == Axis.Y)
                {
                    valueToMove = transform.localPosition.y + transform.lossyScale.y;
                }
                else if (info_SliderDoor.axisToSlide == Axis.Z)
                {
                    valueToMove = transform.localPosition.z + transform.lossyScale.z;
                }
            }
            else
            {
                if (info_SliderDoor.axisToSlide == Axis.X)
                {
                    valueToMove = transform.localPosition.x - transform.lossyScale.z;
                }
                else if (info_SliderDoor.axisToSlide == Axis.Y)
                {
                    valueToMove = transform.localPosition.y - transform.lossyScale.y;
                }
                else if(info_SliderDoor.axisToSlide == Axis.Z)
                {
                    valueToMove = transform.localPosition.z - transform.lossyScale.z;
                }
            }
        }
        else
        {
            valueToMove = positionOriginal;
        }

        if (info_SliderDoor.axisToSlide == Axis.X)
        {
            destiny = new Vector3(valueToMove, transform.localPosition.y, transform.localPosition.z);
        }
        else if (info_SliderDoor.axisToSlide == Axis.Y)
        {
            destiny = new Vector3(transform.localPosition.x, valueToMove, transform.localPosition.z);
        }
        else if (info_SliderDoor.axisToSlide == Axis.Z)
        {
            destiny = new Vector3(transform.localPosition.x, transform.localPosition.y, valueToMove);
        }
        //.

        objectState = State.MOVENDO;

    }

    void RefreshInformations()
    {

        StartCoroutine("ResetProcess");

        if (info_AngularDoor.valueSinal == Signal.NEGATIVE && info_AngularDoor.rotationValue > 0 || info_AngularDoor.valueSinal == Signal.POSITIVE && info_AngularDoor.rotationValue < 0)
        {
            info_AngularDoor.rotationValue *= -1;
        }

        if (angle == positionOriginal)
        {
            angle += info_AngularDoor.rotationValue;
        }
        else
        {
            angle = positionOriginal;
        }

        objectState = State.MOVENDO;

    }

    public IEnumerator ResetProcess()
    {
        
        resetando = true;
        yield return new WaitForSeconds(interaction_TimeOff);
        //Atualizando State
        resetando = false;

        if(openType != OpenType.ANGULAR)
        {
            objectState = State.PARADO;
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
            atravessando = false;
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
            Teleport();
            StartCoroutine("FadeOut");
        }

    }

}

[System.Serializable]
public class TeleportDoor
{
    [Header("Destino ao interagir com a porta")]


    [Header("OpenType Thing's: Teleport")]
    [Tooltip("Destino para onde o player será movido ao interagir com a porta... (''O AXIS AZUL INDICA A DIREÇÃO PARA ONDE O PLAYER IRÁ OLHAR QUANDO FOR TELETRANSPORTADO'')")]
    public Transform posicao;

    [Header("O vagão dessa porta pertence ao sistema de Chase?")]
    [Tooltip("deixar essa variavel ativa, caso esse vagão correspondente a essa porta seja a de um vagão do chase System")]
    public bool partOfChase;

    [Header("Controlador do chase que envolverá esta porta")]
    [Tooltip("Objeto controlador do chase referente a está porta na cena do jogo")]
    public GameObject thisChaseControll;
    [HideInInspector]
    public ChaseSystem chaseRef;

    [Header("Audios da interação")]
    [Tooltip("Audios referentes a cada ação feita com o objeto, abrindo, fechando, trancada e destrancando")]

    public AudioClip Tp_Locked;
    [Range(0, 1)]
    public float vol_Tp_locked = 0.7f;
    public AudioClip Tp_Unlocked;
    [Range(0, 1)]
    public float vol_Tp_unlocked = 0.7f;
    public AudioClip Tp_Opening;
    [Range(0, 1)]
    public float vol_Tp_opening = 0.7f;
    public AudioClip Tp_Closing;
    [Range(0, 1)]
    public float vol_Tp_closing = 0.7f;




}

[System.Serializable]
public class AngularDoor
{
    [Header("Angulo de rotação")]

    [Header("OpenType Thing's: Angular")]
    [Range(0, 90)]
    [Tooltip("Valor de 0 a 360 que o objeto irá rotacionar")]

    public float rotationValue;


    [Header("Eixo de rotação")]
    [Tooltip("Eixo em que o objeto irá rotacionar baseado no valor setado acima")]

    public Axis axisToRotate;


    [Header("Velocidade de abertura")]
    [Tooltip("Velocidade com que a porta irá abrir")]
    [Range(0, 10)]

    public float open_Velocity;


    [Header("Rotacionar no angulo negativo ou positivo")]
    [Tooltip("Sinal de positivo ou negativo que pode ser usado para abrir ou fechar algo, dependendo de onde virá a interação")]

    public Signal valueSinal;

    [Header("Abrir uma segunda porta junto com essa porta")]
    [Tooltip("A segunda porta irá abrir e fechar depois de um determinado tempo, junto da mesma")]
    public bool openTwoDoor;

    [Header("Porta que será aberta ao abrir essa")]
    [Tooltip("Segunda porta que será aberta depois que abrir essa")]
    public GameObject Door;

    [Header("Tempo para que a segunda porta feche depois que for aberta")]
    [Tooltip("esse tempo será contado para que a segunda porta se feche ao finalizar seu ciclo")]
    public float timeToClose;

    [Header("Audios da interação")]
    [Tooltip("Audios referentes a cada ação feita com o objeto, abrindo, fechando, trancada e destrancando")]

    public AudioClip locked;
    [Range(0, 1)]
    public float vol_locked = 0.7f;
    public AudioClip unlocked;
    [Range(0, 1)]
    public float vol_unlocked = 0.7f;
    public AudioClip opening;
    [Range(0, 1)]
    public float vol_opening = 0.7f;
    public AudioClip closing;
    [Range(0, 1)]
    public float vol_closing = 0.7f;

}


[System.Serializable]
public class SliderDoor
{

    [Header("Inverter o lado em que a porta abre")]

    [Header("OpenType Thing's: Slider")]
    [Tooltip("Variavel util para quando haver duas portas Sliders, essa variavel faz com que a porta inverta o lado de abertura, no caso de uma porta dupla, ativar em apenas 1 delas essa variavel")]

    public bool reverse;


    [Header("Eixo de movimentação")]
    [Tooltip("Eixo em que o objeto irá ser movimentado baseado no valor setado acima")]

    public Axis axisToSlide;

    [Header("Abrir uma segunda porta junto com essa porta")]
    [Tooltip("A segunda porta irá abrir e fechar depois de um determinado tempo, junto da mesma")]
    public bool openTwoDoor;

    [Header("Abrir uma segunda porta junto com essa porta")]
    [Tooltip("A segunda porta irá abrir e fechar depois de um determinado tempo, junto da mesma")]
    public float multiplicadorDeAbertura;

    [Header("Porta que será aberta ao abrir essa")]
    [Tooltip("Segunda porta que será aberta depois que abrir essa")]
    public GameObject Door;

    [Header("Tempo para que a segunda porta feche depois que for aberta")]
    [Tooltip("esse tempo será contado para que a segunda porta se feche ao finalizar seu ciclo")]
    public float timeToClose;

    [Header("Audios da interação")]
    [Tooltip("Audios referentes a cada ação feita com o objeto, abrindo, fechando, trancada e destrancando")]

    public AudioClip sl_Locked;
    [Range(0, 1)]
    public float vol_Sl_locked = 0.7f;
    public AudioClip sl_Unlocked;
    [Range(0, 1)]
    public float vol_SL_unlocked = 0.7f;
    public AudioClip sl_Opening;
    [Range(0, 1)]
    public float vol_Sl_opening = 0.7f;
    public AudioClip sl_Closing;
    [Range(0, 1)]
    public float vol_Sl_closing = 0.7f;

}

