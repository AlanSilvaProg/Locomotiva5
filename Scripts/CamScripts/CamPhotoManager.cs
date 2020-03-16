using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


public enum CamState{ACTIVED, DISABLED}


[RequireComponent(typeof(AudioSource))]
public class CamPhotoManager : MonoBehaviour
{

    public static CamPhotoManager scriptRef;
    public static CamState camState = CamState.DISABLED;

    [Header("Layer Mask para o que pode bloquear uma interação")]
    [Tooltip("todas as layers aqui presentes, caso estejam na frente do player, o player não consegue interagir com o que está atrás")]
    public LayerMask rayMask;

    [Header("Raw referente ao que irá renderizar o preview da foto")]
    public RawImage preview_Picture;

    [Header("Camera Usada para pré visualização de fotos")]
    [Tooltip("Uma camera secundaria que tem sua imagem freezada na posição da camera fotografica para uma exibição instantanea, ela é filha da camera fotografica dentro do celular")]

    public GameObject camToExhibition;

    [Header("Camera Visualizada no celular padrão")]
    [Tooltip("A Camera padrão do celular, utilizada apenas para ver pelo aparelho durante a game play")]

    public GameObject camCelularNormal;

    [Header("Render Texture da camera que é responsavel pelas fotos")]
    [Tooltip("Linkar o render texture da camera secundaria que é responsavel por captar as fotos no jogo")]

    public RenderTexture renderCamPhotography;

    [Header("Botão para fechar o preview da foto recém tirada")]
    [Tooltip("quando esse botão for apertado o preview da foto recém retirada será fechada")]

    public string previewPicExit_Button;

    [Header("Flash e o ponto de origem da RayCast")]

    public GameObject flashs;
    public GameObject rayOriginOBJReference;


    [Header("Audios da camera")]
    [Tooltip("Audios referentes a cada ação feita com a camera, fotografando, salvando a foto, excluindo a foto")]

    public AudioClip picture_Photograph;
    [Range(0, 1)]
    public float vol_Photograph = 0.7f;

    [Header("Animator do canvas do celular")]
    [Space(2)]

    [Header("Informações de HUD do Celular")]
    [Tooltip("Animator responsavel por todas as transições da hud do celular durante as ações do jogador")]
    public Animator phoneCanvasAnimator;
    [Header("Preview da ultima foto tirada em miniatura")]
    [Tooltip("Image dentro do canvas do celular, responsável por dar um preview em uma miniatura presente na hud do celular")]
    public RawImage UI_LastPicPreview;
    [Header("Contador de fotos da galeria")]
    [Tooltip("Contador que exibe a quantidade de fotos existentes na camera do jogador, o total de fotos ja tiradas no game, linkar o text responsavel por isso")]
    public Text UI_PicturesCount;

    [Header("Zoom minimo e máximo para a camera do celular")]
    public float minZoom;
    public float maxZoom;
    [Header("Velocidade do Zoom in e out")]
    public float veloZoom;

    private float actual_Zoom;

    private int resWidth = 606;
    private int resHeight = 1280;

    public static bool flash_Permission;

    private AudioSource Source_Cam;

    private bool takeHiResShot = false;
    private Vector3 rayOrigin, destinyPosition;
    private bool photographed;

    [HideInInspector]
    public bool previewPicture;

    private byte[] bytes_ActualPicture;

    private Texture2D actualPicture;

    private Album albumRef;

    private Animator UI_AnimControll;

    private float contador = 0;

    private float alcanceRay;

    private void Awake()
    {
        scriptRef = this;
    }

    private void Start()
    {

        UI_AnimControll = GameObject.FindGameObjectWithTag("UICONTROLL").GetComponent<Animator>();
        Source_Cam = GetComponent<AudioSource>();
        albumRef = FindObjectOfType(typeof(Album)) as Album;

    }

    public void TakeHiResShot()
    {
        takeHiResShot = true;
    }

    void Update()
    {
      
        UI_PicturesCount.text = "" + albumRef.photoAlbum.Count;

        if (camState == CamState.DISABLED)
        {
            
            camToExhibition.GetComponent<Camera>().focalLength = minZoom;
            camCelularNormal.GetComponent<Camera>().focalLength = minZoom;

        }

        if (Input.GetKeyDown(KeyCode.F) && camState == CamState.ACTIVED)
        {
            if (flash_Permission)
            {
                phoneCanvasAnimator.SetTrigger("FlashOff");
                flash_Permission = false;
            }
            else
            {
                phoneCanvasAnimator.SetTrigger("FlashOn");
                flash_Permission = true;
            }
        }

        if (camState == CamState.ACTIVED && Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            camToExhibition.GetComponent<Camera>().focalLength = Mathf.Clamp(camToExhibition.GetComponent<Camera>().focalLength + Input.GetAxis("Mouse ScrollWheel") * veloZoom * Time.deltaTime, minZoom, maxZoom);
            camCelularNormal.GetComponent<Camera>().focalLength = Mathf.Clamp(camToExhibition.GetComponent<Camera>().focalLength + Input.GetAxis("Mouse ScrollWheel") * veloZoom * Time.deltaTime, minZoom, maxZoom);
        }

        if (camState == CamState.ACTIVED && Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            camToExhibition.GetComponent<Camera>().focalLength = Mathf.Clamp(camToExhibition.GetComponent<Camera>().focalLength + Input.GetAxis("Mouse ScrollWheel") * veloZoom * Time.deltaTime, minZoom, maxZoom);
            camCelularNormal.GetComponent<Camera>().focalLength = Mathf.Clamp(camToExhibition.GetComponent<Camera>().focalLength + Input.GetAxis("Mouse ScrollWheel") * veloZoom * Time.deltaTime, minZoom, maxZoom);
        }

        if (camState == CamState.ACTIVED && !previewPicture)
        {

            if (Input.GetKeyDown(KeyCode.Mouse1) && contador > 1)
            {
                contador = 0;
                Preview();
            }

            takeHiResShot |= Input.GetKeyDown(KeyCode.Mouse0);

            if (takeHiResShot && !photographed)
            {

                photographed = true;

                phoneCanvasAnimator.SetTrigger("ShootPic");

                /////////////////////////////////////////////// SOM REFERENTE A AÇÃO DE FOTOGRAFAR //////////////////////////////////////////////

                Source_Cam.PlayOneShot(picture_Photograph, vol_Photograph);

                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                StartCoroutine("Flash");
                StartCoroutine("TakeFoto");

            }
        }
        else if (previewPicture && Input.GetKeyDown(KeyCode.Mouse1) && contador > 1)
        {
            PreviewExit();
        }

        contador += Time.deltaTime;

    }
    
    public IEnumerator TakeFoto()
    {
        yield return new WaitForSeconds(0.1f);
        RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
        camCelularNormal.GetComponent<Camera>().targetTexture = null;
        camToExhibition.GetComponent<Camera>().targetTexture = renderCamPhotography;
        camToExhibition.GetComponent<Camera>().Render();
        camToExhibition.GetComponent<Camera>().targetTexture = rt;
        Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
        camToExhibition.GetComponent<Camera>().Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
        camToExhibition.GetComponent<Camera>().targetTexture = null;
        camCelularNormal.GetComponent<Camera>().targetTexture = renderCamPhotography;
        RenderTexture.active = null; // JC: added to avoid errors
        Destroy(rt);
        bytes_ActualPicture = screenShot.EncodeToPNG();

        takeHiResShot = false;
    }

    public IEnumerator Flash()
    {
        
       if (flash_Permission)
        {
            flashs.SetActive(true);
        }

        yield return new WaitForSeconds(0.2f);
        if (flash_Permission)
        {
           flashs.SetActive(false);
        }
        yield return new WaitForSeconds(0.2f);
       SaveButton();

    }

    public void Preview()
    {

        UI_AnimControll.SetTrigger("LastPicIn");
        phoneCanvasAnimator.SetTrigger("LastPicIn");
        preview_Picture.texture = actualPicture;
        preview_Picture.enabled = true;
        previewPicture = true;

    }

    public void PreviewExit()
    {

        UI_AnimControll.SetTrigger("LastPicOut");
        phoneCanvasAnimator.SetTrigger("LastPicOut");
        preview_Picture.enabled = false;
        previewPicture = false;

    }

    public void SaveButton()
    {

        photographed = false;

        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(bytes_ActualPicture);
        actualPicture = tex;
        albumRef.photoAlbum.Add(tex);
        UI_LastPicPreview.texture = actualPicture;
        albumRef.NewPic();
        Preview();

    }

    public void SaveButton(byte[] bytes_actual)
    {

        photographed = false;

        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(bytes_actual);
        actualPicture = tex;
        albumRef.photoAlbum.Add(tex);
        UI_LastPicPreview.texture = actualPicture;
        albumRef.NewPic();
        Preview();

    }

    private void OnTriggerStay(Collider col)
    {
        
        if (photographed)
        {
          
            if (col.gameObject.tag == "PhotoInteracts")
            {

                alcanceRay = Vector3.Distance(Camera.main.transform.position, col.gameObject.transform.position);
                Interact_Trigger();
                 
            }

        }

    }

    private void Interact_Trigger()
    {

        if (isActiveAndEnabled)
        {
            Transform cam = Camera.main.transform;
            RaycastHit[] ray_Hit;
            ray_Hit = Physics.RaycastAll(gameObject.GetComponent<Camera>().transform.position, gameObject.GetComponent<Camera>().transform.forward, alcanceRay + 1, rayMask);
           

            if (ray_Hit.Length >= 1)
            {
                Transform interactTarget = null;
                float tempDist = 0;
               

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
                    interactTarget.SendMessage("Act");
                }

            }
        }

    }

}
