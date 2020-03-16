using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

/// State de permissão da visualização, quando for Permitted, o canvas exibe o objeto em questão ///
public enum Permission { PERMITTED, UNPERMITTED}

public enum SuitCase { ONPASSWORD, OUTPASSWORD}

[RequireComponent(typeof(AudioSource))]
public class InvestigationLook : MonoBehaviour
{

////Controle Global dos States da visualização de algum objeto///
    public static Permission permissionToInvestigation = Permission.UNPERMITTED;

    public static SuitCase suitCaseStates = SuitCase.OUTPASSWORD;

    public static InvestigationLook investigationRef;

    [Header("Camera responsável pela visualização 3D do objeto")]
    public GameObject camera3DVisu;

    [Header("Camera responsável pela foto do objeto que está sendo investigado")]
    public GameObject camera3DPhoto;

    [Header("Layer Mask para o que pode bloquear uma interação")]
    [Tooltip("todas as layers aqui presentes, caso estejam na frente do player, o player não consegue interagir com o que está atrás")]
    public LayerMask rayMask;

    [Header("Zoom minimo e máximo para a camera do celular")]
    public float minZoom;
    public float maxZoom;
    [Header("Velocidade do Zoom in e out")]
    public float veloZoom;

    private float actual_Zoom;

    [Header("Audios da Print")]
    [Tooltip("Audios referentes a cada ação feita de print")]

    public AudioClip picture_Print;
    [Range(0, 1)]
    public float vol_Print = 0.7f;
    private AudioSource audioSource;

    ///Informações do objeto visualizado///
    private GameObject objectToLook;

    private Vector3 oldPos;

    private Quaternion oldRot;

   ///Informações de acesso extra e variaveis de controle///
    private Animator Animator_Investigation; // Canvas de exibição do objeto

    public GameObject anchor; // Ancora para onde o objeto vai ser levado para que seja visualizado

    public GameObject anchorSuitCase; // ancora especial para o zoom na senha da maleta

    public float velo = 3; // Velocidade de giro do objeto

    private bool collectable;

    private bool printando;

    private byte[] bytes_ActualPicture;

    private Texture2D actualPicture;

    private Album albumRef;

    private int resWidth = 606;
    private int resHeight = 1280;

    private float inputH, inputV;

    private bool giroH, giroV;

    //Controles do jornal

    public int pageCtrl; // -1 0 1 ... TurnPage <<>> Open  de -1 até 1 é Open, e de 1 a -1 é TurnPage

    private int sinal = 1;

    private bool interagindo;

    public bool journal;

    private float count_InteractInterval;

    private CamPhotoManager camManagerRef;

    private Player_Interact playerIntRef;

    private float countToPhotage;

    // Controles Suit Case

    private Transform[] originalTransform = new Transform[5];

    private int[] ctlr_PassWordNum = new int[4];

    public int indiceButtom = 0;

    private bool suitCase;

    private string actualPassWord;

    private GameManager gmRef;


    private void Awake()
    {
        investigationRef = this;
    }

    // Start is called before the first frame update
    void Start()
    {

        gmRef = GameManager.gmRef;
        playerIntRef = Player_Interact.playerIntRef;
        camManagerRef = CamPhotoManager.scriptRef;
        Animator_Investigation = GameObject.FindGameObjectWithTag("UICONTROLL").GetComponent<Animator>();
        albumRef = FindObjectOfType(typeof(Album)) as Album;
        audioSource = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {

        inputH = Input.GetAxisRaw("Horizontal");
        inputV = Input.GetAxisRaw("Vertical");

        if(inputH == 0)
        {
            giroH = false;
        }

        if(inputV == 0)
        {
            giroV = false;
        }

        if (!giroH && inputV != 0)
        {
            giroV = true;
        }
        else if (!giroV && inputH != 0)
        {
            giroH = true;
        }
        

        countToPhotage += Time.deltaTime;

        if (permissionToInvestigation == Permission.PERMITTED)
        {

            ////////////////////////////////Interações com o objeto/////////////////////////////////////////////////

            if (suitCaseStates == SuitCase.ONPASSWORD)
            {

                if(objectToLook != null && !objectToLook.GetComponent<Collectable_Objects>().foiAberta)
                {
                    switch (indiceButtom)
                    {
                        case 0:
                            
                            objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.arrow_Indication[0].SetActive(true);
                            objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.arrow_Indication[1].SetActive(false);
                            objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.arrow_Indication[2].SetActive(false);
                            objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.arrow_Indication[3].SetActive(false);
                            objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.arrow_Indication[4].SetActive(false);

                            break;
                        case 1:
                            
                            objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.arrow_Indication[0].SetActive(false);
                            objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.arrow_Indication[1].SetActive(true);
                            objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.arrow_Indication[2].SetActive(false);
                            objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.arrow_Indication[3].SetActive(false);
                            objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.arrow_Indication[4].SetActive(false);

                            break;
                        case 2:

                            objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.arrow_Indication[0].SetActive(false);
                            objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.arrow_Indication[1].SetActive(false);
                            objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.arrow_Indication[2].SetActive(true);
                            objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.arrow_Indication[3].SetActive(false);
                            objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.arrow_Indication[4].SetActive(false);

                            break;
                        case 3:

                            objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.arrow_Indication[0].SetActive(false);
                            objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.arrow_Indication[1].SetActive(false);
                            objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.arrow_Indication[2].SetActive(false);
                            objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.arrow_Indication[3].SetActive(true);
                            objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.arrow_Indication[4].SetActive(false);

                            break;
                        case 4:

                            objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.arrow_Indication[0].SetActive(false);
                            objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.arrow_Indication[1].SetActive(false);
                            objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.arrow_Indication[2].SetActive(false);
                            objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.arrow_Indication[3].SetActive(false);
                            objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.arrow_Indication[4].SetActive(true);

                            break;

                    }
                }

                if (objectToLook != null)
                {
                    objectToLook.transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                if (Input.GetKeyDown(KeyCode.Mouse1) && !interagindo)
                {

                    NewPosition();
                    suitCaseStates = SuitCase.OUTPASSWORD;

                }

                if (Input.GetKeyDown(KeyCode.Mouse0) && !objectToLook.GetComponent<Collectable_Objects>().foiAberta)
                {
                    if (indiceButtom == 0)
                    {
                        if (objectToLook != null)
                        {
                            if (actualPassWord == objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.password_Right)
                            {
                                TruePassWord();
                            }
                            else if (actualPassWord == objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.password_Wrong)
                            {
                                FalsePassword();
                            }
                        }
                    }
                }


                if (Input.GetKeyDown(KeyCode.A))
                {
                    if(indiceButtom > 0) { indiceButtom--; }
                }

                if (Input.GetKeyDown(KeyCode.D))
                {
                    if (indiceButtom < 4) { indiceButtom++; }
                }

                if (indiceButtom != 0)
                {

                    if (Input.GetKeyDown(KeyCode.S))
                    {
                        float value;
                        value = 0;
                        switch (indiceButtom)
                        {
                            case 1:

                                value = objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.rotationPassWordButtom * -1;
                                objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.password_ConfirmButtons[indiceButtom].transform.Rotate(0, value, 0);

                                ctlr_PassWordNum[indiceButtom - 1] -= 1;

                                if (ctlr_PassWordNum[indiceButtom - 1] < 0)
                                {
                                    ctlr_PassWordNum[indiceButtom - 1] = 9;
                                }

                                break;
                            case 2:

                                value = objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.rotationPassWordButtom * -1;
                                objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.password_ConfirmButtons[indiceButtom].transform.Rotate(0, value, 0);

                                ctlr_PassWordNum[indiceButtom - 1] -= 1;

                                if (ctlr_PassWordNum[indiceButtom - 1] < 0)
                                {
                                    ctlr_PassWordNum[indiceButtom - 1] = 9;
                                }
                                break;
                            case 3:

                                value = objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.rotationPassWordButtom * -1;
                                objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.password_ConfirmButtons[indiceButtom].transform.Rotate(0, value, 0);

                                ctlr_PassWordNum[indiceButtom - 1] -= 1;

                                if (ctlr_PassWordNum[indiceButtom - 1] < 0)
                                {
                                    ctlr_PassWordNum[indiceButtom - 1] = 9;
                                }
                                break;
                            case 4:

                                value = objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.rotationPassWordButtom * -1;
                                objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.password_ConfirmButtons[indiceButtom].transform.Rotate(0, value, 0);

                                ctlr_PassWordNum[indiceButtom - 1] -= 1;

                                if (ctlr_PassWordNum[indiceButtom - 1] < 0)
                                {
                                    ctlr_PassWordNum[indiceButtom - 1] = 9;
                                }
                                break;
                        }
                    }

                    if (Input.GetKeyDown(KeyCode.W))
                    {
                        float value;
                        value = 0;
                        switch (indiceButtom)
                        {
                            case 1:

                                value = objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.rotationPassWordButtom;
                                objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.password_ConfirmButtons[indiceButtom].transform.Rotate(0, value, 0);

                                ctlr_PassWordNum[indiceButtom - 1] += 1;
                                if (ctlr_PassWordNum[indiceButtom - 1] > 9)
                                {
                                    ctlr_PassWordNum[indiceButtom - 1] = 0;
                                }
                                break;
                            case 2:

                                value = objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.rotationPassWordButtom;
                                objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.password_ConfirmButtons[indiceButtom].transform.Rotate(0, value, 0);

                                ctlr_PassWordNum[indiceButtom - 1] += 1;
                                if (ctlr_PassWordNum[indiceButtom - 1] > 9)
                                {
                                    ctlr_PassWordNum[indiceButtom - 1] = 0;
                                }
                                break;
                            case 3:

                                value = objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.rotationPassWordButtom;
                                objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.password_ConfirmButtons[indiceButtom].transform.Rotate(0, value, 0);

                                ctlr_PassWordNum[indiceButtom - 1] += 1;
                                if (ctlr_PassWordNum[indiceButtom - 1] > 9)
                                {
                                    ctlr_PassWordNum[indiceButtom - 1] = 0;
                                }
                                break;
                            case 4:

                                value = objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.rotationPassWordButtom;
                                objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.password_ConfirmButtons[indiceButtom].transform.Rotate(0, value, 0);

                                ctlr_PassWordNum[indiceButtom - 1] += 1;
                                if (ctlr_PassWordNum[indiceButtom - 1] > 9)
                                {
                                    ctlr_PassWordNum[indiceButtom - 1] = 0;
                                }
                                break;
                        }
                    }

                }

            }
            else if(suitCaseStates == SuitCase.OUTPASSWORD)
            {

                if(objectToLook.GetComponent<Collectable_Objects>().isSuitCase && objectToLook.GetComponent <Collectable_Objects>().foiAberta)
                {
                    objectToLook.GetComponent<BoxCollider>().enabled = false;
                }
                else if (objectToLook.GetComponent<Collectable_Objects>().isSuitCase && !objectToLook.GetComponent<Collectable_Objects>().foiAberta)
                {
                    objectToLook.GetComponent<BoxCollider>().enabled = true;
                }

                if (Input.GetAxis("Mouse ScrollWheel") > 0f)
                {
                    camera3DVisu.GetComponent<Camera>().focalLength = Mathf.Clamp(camera3DVisu.GetComponent<Camera>().focalLength + Input.GetAxis("Mouse ScrollWheel") * veloZoom * Time.deltaTime, minZoom, maxZoom);
                    camera3DPhoto.GetComponent<Camera>().focalLength = Mathf.Clamp(camera3DPhoto.GetComponent<Camera>().focalLength + Input.GetAxis("Mouse ScrollWheel") * veloZoom * Time.deltaTime, minZoom, maxZoom);
                }

                if (Input.GetAxis("Mouse ScrollWheel") < 0f)
                {
                    camera3DVisu.GetComponent<Camera>().focalLength = Mathf.Clamp(camera3DVisu.GetComponent<Camera>().focalLength + Input.GetAxis("Mouse ScrollWheel") * veloZoom * Time.deltaTime, minZoom, maxZoom);
                    camera3DPhoto.GetComponent<Camera>().focalLength = Mathf.Clamp(camera3DPhoto.GetComponent<Camera>().focalLength + Input.GetAxis("Mouse ScrollWheel") * veloZoom * Time.deltaTime, minZoom, maxZoom);
                }

                if (Input.GetKeyDown(KeyCode.Mouse1) && !interagindo)
                {

                    if (suitCase && !objectToLook.GetComponent<Collectable_Objects>().foiAberta)
                    {

                        suitCaseStates = SuitCase.ONPASSWORD;
                        NewPositionSuitCase();

                    }

                    if (journal)
                    {

                        pageCtrl += 1 * sinal;

                        if (sinal > 0)
                        {
                            objectToLook.gameObject.GetComponent<Animator>().SetTrigger("Open");
                        }
                        else if (sinal < 0)
                        {
                            objectToLook.gameObject.GetComponent<Animator>().SetTrigger("TurnPage");
                        }



                        if (pageCtrl == 1)
                        {
                            sinal = -1;
                        }
                        else if (pageCtrl == -1)
                        {
                            sinal = 1;
                        }

                        if (pageCtrl != 0)
                        {
                            objectToLook.GetComponent<Collectable_Objects>().colliders.SetActive(true);
                        }
                        else
                        {
                            objectToLook.GetComponent<Collectable_Objects>().colliders.SetActive(false);
                        }

                        interagindo = true;
                        count_InteractInterval = 0;

                    }

                }
                else if (interagindo)
                {

                    if (count_InteractInterval > 0.5)
                    {
                        interagindo = false;
                    }
                    else
                    {
                        count_InteractInterval += Time.deltaTime;
                    }

                }

                if (Input.GetKeyDown(KeyCode.Mouse1) && objectToLook.GetComponent<Collectable_Objects>().foiAberta)
                {
                    CheckTrigger();
                }

            }

            ////////////////////////////////Movimentação e rotação////////////////////////////////

            if (suitCaseStates == SuitCase.OUTPASSWORD)
            {
                if (giroH)
                {
                    if (inputH > 0 && giroH) // Comparando atual posição do mouse com a antiga e efetuando as ações necessárias
                    {
                        anchor.transform.Rotate(0, -1 * velo * Time.deltaTime, 0, Space.World);
                    }

                    if (inputH < 0 && giroH) // Comparando atual posição do mouse com a antiga e efetuando as ações necessárias
                    {
                        anchor.transform.Rotate(0, 1 * velo * Time.deltaTime, 0, Space.World);
                    }
                }

                if (giroV)
                {
                    if (inputV > 0 && giroV) // Comparando atual posição do mouse com a antiga e efetuando as ações necessárias
                    {
                        anchor.transform.Rotate(0, 0, -1 * velo * Time.deltaTime, Space.World);
                    }

                    if (inputV < 0 && giroV) // Comparando atual posição do mouse com a antiga e efetuando as ações necessárias
                    {
                        anchor.transform.Rotate(0, 0, 1 * velo * Time.deltaTime, Space.World);
                    }
                }


                if (inputH == 0 && inputV == 0)
                {
                    anchor.transform.Rotate(0, 0, 0, Space.World);
                }

            }
            
        }
        else
        {

            anchor.transform.rotation = new Quaternion(0, 0, 0, 0);

            if (camera3DVisu.GetComponent<Camera>().focalLength != minZoom)
            {
                camera3DVisu.GetComponent<Camera>().focalLength = minZoom;
            }

            if (camera3DPhoto.GetComponent<Camera>().focalLength != minZoom)
            {
                camera3DPhoto.GetComponent<Camera>().focalLength = minZoom;
            }

        }
        
    }

    private void LateUpdate()
    {

        if (suitCaseStates == SuitCase.OUTPASSWORD && permissionToInvestigation == Permission.PERMITTED)
        {

            if (Input.GetKeyDown(KeyCode.Mouse0) && !printando && countToPhotage > 2)
            {

                TakeFoto();
                countToPhotage = 0;

            }
            else if (printando)
            {

                if (countToPhotage > 2)
                {
                    printando = false;
                }

            }


            if (Input.GetKeyDown(KeyCode.Escape))
            {
                NonPermission();
            }

        }


        if (suitCaseStates == SuitCase.ONPASSWORD)
        {
            objectToLook.transform.position = anchorSuitCase.transform.position;
            objectToLook.transform.rotation = anchorSuitCase.transform.rotation;

            actualPassWord = "" + ctlr_PassWordNum[0] + "" + ctlr_PassWordNum[1] + "" + ctlr_PassWordNum[2] + "" + ctlr_PassWordNum[3];
        }

    }

    void TruePassWord()
    {

        if(objectToLook != null && !objectToLook.GetComponent<Collectable_Objects>().foiAberta)
        {
            objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.suitCase_Up.transform.Rotate( 0, 0, objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.open_Angle, Space.Self);
            objectToLook.GetComponent<Collectable_Objects>().foiAberta = true;

            if (objectToLook.GetComponent<Collectable_Objects>().addGoodPoint)
            {
                gmRef.goodEnding += objectToLook.GetComponent<Collectable_Objects>().goodPoints;
            }

        }

        if (objectToLook != null)
        {
            objectToLook.GetComponent<Collectable_Objects>().player_AudioSource.PlayOneShot(objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.clip_RightPassword, objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.vol_Right_Password);
        }

    }

    void FalsePassword()
    {

        NonPermission();

        if (objectToLook.GetComponent<Collectable_Objects>().addBadPoint)
        {
            gmRef.badEnding += objectToLook.GetComponent<Collectable_Objects>().badPoints;
        }

        if (objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.activate_Chase && objectToLook.GetComponent<Collectable_Objects>().chaseRef != null) // verificando se o Chase Sequence será ativado
        {
            // Procurando pelo ChaseSystem e chamando a respectiva função para ativar o chase sequence
            objectToLook.GetComponent<Collectable_Objects>().chaseRef.SendMessage("ChaseSetActive", true);

        }

        if (objectToLook != null)
        {
            objectToLook.GetComponent<Collectable_Objects>().player_AudioSource.PlayOneShot(objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.clip_WrongPassword, objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.vol_WrongPassword);
        }
        

    }

    void TakeFoto()
    {

        printando = true;

        RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
        camera3DPhoto.GetComponent<Camera>().targetTexture = rt;
        Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
        camera3DPhoto.GetComponent<Camera>().Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
        camera3DPhoto.GetComponent<Camera>().targetTexture = null;
        RenderTexture.active = null; // JC: added to avoid errors
        Destroy(rt);
        bytes_ActualPicture = screenShot.EncodeToPNG();

        CheckTrigger();

    }


    public IEnumerator ShowPreview()
    {
        
        yield return new WaitForSeconds(1f);
        playerIntRef.CelularEnable_Trigger();
        camManagerRef.SaveButton(bytes_ActualPicture);

    }
    

    public void CheckTrigger()
    {

        if (isActiveAndEnabled)
        {
            
            Transform cam = camera3DVisu.GetComponent<Camera>().transform;
            RaycastHit[] ray_Hit;
            ray_Hit = Physics.RaycastAll(cam.position, cam.transform.forward, 100, rayMask);


            Transform interactTarget = null;
            float tempDist = 0;

            if (ray_Hit.Length >= 1)
            {

                if (interactTarget == null)
                {
                    interactTarget = ray_Hit[0].transform;
                    tempDist = ray_Hit[0].distance;
                }

                for (int i = 0; i < ray_Hit.Length; i++)
                {

                    if (ray_Hit[i].distance < tempDist)
                    {
                        interactTarget = ray_Hit[i].transform;
                        tempDist = ray_Hit[i].distance;
                    }

                }

                if (interactTarget.tag == "PhotoInteracts")
                {
                    if (objectToLook.GetComponent<Collectable_Objects>().isJournal)
                    {
                        if (interactTarget.GetComponent<Journal>().side == 1 && pageCtrl == 1 || interactTarget.GetComponent<Journal>().side == -1 && pageCtrl == -1)
                        {
                            interactTarget.SendMessage("Act");
                        }
                    }

                }
                else if(interactTarget.tag == "PlayerInteract")
                {

                    if (suitCaseStates == SuitCase.ONPASSWORD && objectToLook.GetComponent<Collectable_Objects>().foiAberta)
                    {
                        interactTarget.SendMessage("Interaction");
                    }

                }
            }
            
            if (printando)
            {
                StartCoroutine("ShowPreview");
            }

            NonPermission();

        }

    }

    ////////////////////Recebendo e atualizando informações sobre o objeto que será visualizado////////////////////////////////
    public void SendPermission(GameObject objToLook)
    {
        
        StopAllCoroutines();

        countToPhotage = 0;

        Animator_Investigation.SetTrigger("InvestigationIn");
        collectable = objToLook.GetComponent<Collectable_Objects>().isCollectable;
        permissionToInvestigation = Permission.PERMITTED;
        Player_Interact.playerStates = PlayerStates.INTERAGINDO;
        objectToLook = objToLook;
        oldPos = objectToLook.transform.position;
        oldRot = objectToLook.transform.rotation;

        if (objectToLook.GetComponent<Collectable_Objects>().isSuitCase)
        {

            suitCase = true;

            for (int i = 0; i < originalTransform.Length; i++)
            {
                originalTransform[i] = objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.password_ConfirmButtons[i].transform;
            }

            for (int i = 0; i < ctlr_PassWordNum.Length; i++)
            {

                ctlr_PassWordNum[i] = 0;
                indiceButtom = 0;

            }

        }

        journal = objectToLook.gameObject.GetComponent<Collectable_Objects>().isJournal;

        if (objectToLook.gameObject.GetComponent<Collectable_Objects>().isJournal)
        {

            objectToLook.gameObject.GetComponent<BoxCollider>().enabled = false;
            objectToLook.gameObject.GetComponent<Collectable_Objects>().colliders.SetActive(true);

        }

        NewPosition();

    }

///////////////////Determinando a posição nova do objeto para que seja visualizado////////////////////////////////
    public void NewPosition()
    {

        objectToLook.transform.parent = anchor.transform;
        objectToLook.transform.localPosition =Vector3.zero;
        objectToLook.transform.rotation = new Quaternion(0, 0, 0, 0);
        
    }

    public void NewPositionSuitCase()
    {

        objectToLook.transform.parent = anchorSuitCase.transform;
        objectToLook.transform.localPosition = Vector3.zero;
        objectToLook.transform.rotation = new Quaternion(0, 0, 0, 0);

    }

    ///////////////////Finalizando a visualização do objeto///////////////////////////////
    public void NonPermission()
    {

        Animator_Investigation.SetTrigger("InvestigationOut");
        permissionToInvestigation = Permission.UNPERMITTED;
        Player_Interact.playerStates = PlayerStates.NOTHING;
        interagindo = false;
        printando = false;

        if (suitCase)
        {

            suitCaseStates = SuitCase.OUTPASSWORD;

            suitCase = false;

            for (int i = 0; i < originalTransform.Length; i++)
            {
                objectToLook.GetComponent<Collectable_Objects>().suitCaseInfo.password_ConfirmButtons[i].transform.rotation = originalTransform[i].rotation;
            }

            for (int i = 0; i < ctlr_PassWordNum.Length; i++)
            {

                ctlr_PassWordNum[i] = 0;
                indiceButtom = 0;

            }

        }

        if (journal)
        {

            if (pageCtrl != 0)
            {
                if (sinal > 0)
                {
                    objectToLook.gameObject.GetComponent<Animator>().SetTrigger("Open");
                }
                else if (sinal < 0)
                {
                    objectToLook.gameObject.GetComponent<Animator>().SetTrigger("TurnPage");
                }
                pageCtrl = 0;
                sinal = 1;
            }

            journal = false;
            objectToLook.GetComponent<BoxCollider>().enabled = true;
            objectToLook.GetComponent<Collectable_Objects>().colliders.SetActive(false);

        }

        if (collectable)
        {
            Destroy(objectToLook);
        }
        else
        {
            ReturnToOldPosition();
        }

    }

    ///////////////////Colocando o objeto de volta para sua posição original da cena após visualização////////////////////////////////
    public void ReturnToOldPosition()
    {

        objectToLook.SendMessage("TurnOff");
        objectToLook.transform.SetParent(null);
        objectToLook.transform.position = oldPos;
        objectToLook.transform.rotation = oldRot;
        objectToLook = null;

    }

}
