using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public enum PlayerStates { INTERAGINDO, FALANDO, NOTHING}

public class Player_Interact : MonoBehaviour
{

    public static Transform lastDoor;

    public static PlayerStates playerStates;

    public static Player_Interact playerIntRef;

    [Header("Layer Mask para o que pode bloquear uma interação")]
    [Tooltip("todas as layers aqui presentes, caso estejam na frente do player, o player não consegue interagir com o que está atrás")]
    public LayerMask rayMask;

    [Header("GameObject do celular")]
    [Tooltip("Linkar o celular filho do fps controller, para que ele seja habilitado e desabilitado da cena")]
    public GameObject celular;

    [Header("Animator do Celular")]
    [Tooltip("Animator responsável pelos movimentos do celular e toda a movimentação do mesmo por animação")]
    public Animator phoneAnimator;
    [Header("Animator da UI do Celular")]
    [Tooltip("Animator responsável pelas animações responsivas do celular em resposta das ações do jogador, como tirar fotos, transição de menus, etc..")]
    public Animator phoneCanvasAnimator;
    [Header("UI do celular")]
    [Tooltip("Linkar o object responsavel por carregar toda a estrutura de UI do celular")]
    public Transform camCanvas;

    private CharacterController character_Ctrl;
    private FirstPersonController fps_Controller;
    
    private bool camModeOn = false;

    private Animator UI_AnimControll;

    private bool firstTime = true;

    private float contador;
    private CamPhotoManager camRef;
    private GameManager gmRef;

    private ChaseSystem refChaseSystem;

    private ChaseMonster monsterRef;

    private float alcanceRay;

    private void Awake()
    {
        playerIntRef = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
        gmRef = GameManager.gmRef;
        camRef = CamPhotoManager.scriptRef;
        UI_AnimControll = GameObject.FindGameObjectWithTag("UICONTROLL").GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
        fps_Controller = GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonController>();
        playerStates = PlayerStates.NOTHING;
        character_Ctrl = FindObjectOfType(typeof(CharacterController)) as CharacterController;
        fps_Controller.permission = true;
        firstTime = true;
        camModeOn = false;

        contador = 0;

    }

    // Update is called once per frame
    void Update()
    {


        if (CamPhotoManager.camState == CamState.ACTIVED)
        {
            fps_Controller.on_Cam = true;
        }
        else
        {
            fps_Controller.on_Cam = false;
        }

        if (playerStates != PlayerStates.INTERAGINDO && PauseGame.pauseState == PauseStates.OUTPAUSE)
        {
            if (!fps_Controller.permission)
            {
                fps_Controller.ReInitMouseLook();
                fps_Controller.permission = true;
            }

            if (!character_Ctrl.enabled)
            {
                character_Ctrl.enabled = true;
            }


            if (Input.GetKeyDown(KeyCode.Tab) && InvestigationLook.permissionToInvestigation != Permission.PERMITTED && contador > 1)
            {

                CelularEnable_Trigger();

                if (firstTime)
                {
                    if (GameObject.FindGameObjectWithTag("FirstTime") != null)
                    {
                        GameObject.FindGameObjectWithTag("FirstTime").gameObject.SendMessage("Interaction");
                    }
                }

            }

            if (InvestigationLook.permissionToInvestigation != Permission.UNPERMITTED)
            {
                if (celular.activeSelf == true)
                {
                    if (camRef.previewPicture)
                    {
                        camRef.PreviewExit();
                    }
                    celular.SetActive(false);
                }
            }


        }
        else if(playerStates == PlayerStates.INTERAGINDO)
        {
            if (character_Ctrl.enabled == true)
            {
                character_Ctrl.enabled = false;
            }
            if (fps_Controller.permission != false)
            {
                fps_Controller.permission = false;
            }
        }

        contador += Time.deltaTime;


    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Enemy")
        {
            refChaseSystem = other.gameObject.GetComponent<WallEnemy>().thisChaseSystemControll.GetComponent<ChaseSystem>();
            if (!gmRef.neverDie)
            {
                Die();
                UI_AnimControll.SetTrigger("DeathIn");
            }
        }

    }

    private void OnTriggerStay(Collider col)
    {
        
        if (Input.GetKeyUp(KeyCode.Mouse0) && playerStates == PlayerStates.NOTHING && CamPhotoManager.camState == CamState.DISABLED && InvestigationLook.permissionToInvestigation == Permission.UNPERMITTED)
        {
            if (col.gameObject.tag == ("PlayerInteract"))
            {
                alcanceRay = Vector3.Distance(Camera.main.transform.position, col.gameObject.transform.position);
                Interact_Trigger();
            }
        }

        if(col.gameObject.tag == ("TriggerFala") && playerStates == PlayerStates.NOTHING && CamPhotoManager.camState == CamState.DISABLED)
        {
            playerStates = PlayerStates.FALANDO;
            col.gameObject.SendMessage("PlayConversation");
        }

    }

    private void Interact_Trigger()
    {
        if (isActiveAndEnabled)
        {
            Transform cam = Camera.main.transform;
            RaycastHit[] ray_Hit;
            ray_Hit = Physics.RaycastAll(gameObject.GetComponent<Camera>().transform.position, gameObject.GetComponent<Camera>().transform.forward, alcanceRay + 1, rayMask);

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

            }

            if (interactTarget != null)
            {
                if (interactTarget.tag == "PlayerInteract")
                {
                    interactTarget.SendMessage("Interaction");
                }
            }

        }
        
    }

    public void CelularEnable_Trigger()
    {
        if (!camModeOn)
        {
            UI_AnimControll.SetTrigger("CameraIn");
            camCanvas.gameObject.SetActive(true);
            phoneAnimator.SetTrigger("PhoneIn");
            phoneCanvasAnimator.SetTrigger("CamIn");
            CamPhotoManager.camState = CamState.ACTIVED;
            camModeOn = true;
        }
        else if (camModeOn)
        {
            if (camRef.previewPicture)
            {
                camRef.PreviewExit();
            }
            UI_AnimControll.SetTrigger("CameraOut");
            camCanvas.gameObject.SetActive(false);
            phoneAnimator.SetTrigger("PhoneOut");
            phoneCanvasAnimator.SetTrigger("CamOut");
            CamPhotoManager.camState = CamState.DISABLED;
            camModeOn = false;
        }

        contador = 0;

    }

    public void Die()
    {

        refChaseSystem.ChaseSetActive(false);
        gmRef.Die();

    }

}
