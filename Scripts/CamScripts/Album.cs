using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Resources;
using UnityEngine.UI;

public enum AlbumStates { INALBUM, ONPREVIEW, OUTALBUM}

public class Album : MonoBehaviour
{

    public static int photo_Quantity ;

    public static Album albumRef;

    [HideInInspector]
    public AlbumStates albumState;

    [HideInInspector]
    public List<Texture2D> photoAlbum;
    
    [Header("Grid que armazena os slots de imagens do album")]
    [Tooltip("Raw Imagens referentes ao painel de exibição do album, onde ficaram todas as fotos salvas")]
    public GameObject albumImagesGroup;

    [Header("Scroll responsável pelo album")]
    [Tooltip("Scroll que será usado para scrollar o album conforme ir selecionando as fotos")]
    public Scrollbar scrollAlbum;

    public RawImage preview_Image;

    public Animator buttons_Yes, buttons_No;

    private List<RawImage> albumSlots;

    private int indice = 0;
    private float value = 0.025f; // Deminuir essa quantidade do scroll para descer o equivalente a uma linha de fotos, e acrescentar para subir

    private bool colorControll; // true para somar e false para subtrair do alfa da imagem selecionada 

    private int lastCatcher = 0;

    private PauseGame pauseRef;

    private float count;

    private int indice_Two = 0;

    private bool lowPic;

    private void Awake()
    {
        albumRef = this;
    }

    // Start is called before the first frame update
    void Start()
    {

        indice_Two = 0;
        buttons_Yes.SetTrigger("Highlighted");
        preview_Image.enabled = false;
        albumSlots = new List<RawImage>();
        pauseRef = PauseGame.pauseRef;
        albumState = AlbumStates.OUTALBUM;

    }

    // Update is called once per frame
    void Update()
    {

        count += Time.deltaTime;

        if (photo_Quantity != lastCatcher)
        {
            AtualizarCatcher();
        }

        if (albumState == AlbumStates.INALBUM && PauseGame.pauseState == PauseStates.SUBMENU)
        {

            if (Input.GetKeyDown(KeyCode.Return) && count > 0.2f)
            {

                count = 0;
                preview_Image.texture = photoAlbum[indice];
                EnablePreview();

            }

            if (lastCatcher != 0)
            {

                if (Input.GetKeyDown(KeyCode.DownArrow) && indice < photo_Quantity - 1 && count > 0.2f)
                {
                    albumSlots[indice].color = Color.white;
                    colorControll = false;
                    indice++;
                    count = 0;
                    lowPic = true;

                    if (indice > 1 && indice + 1 < photo_Quantity)
                    {
                        scrollAlbum.value -= value;
                    }
                    else
                    {
                        lowPic = true;
                    }

                }

                if (Input.GetKeyDown(KeyCode.UpArrow) && indice > 0 && count > 0.2f)
                {
                    albumSlots[indice].color = Color.white;
                    colorControll = false;
                    indice--;
                    count = 0;

                    if (indice != 0 && !lowPic)
                    {
                        scrollAlbum.value += value;
                    }else if (lowPic)
                    {
                        lowPic = false;
                    }

                }

//Controle do efeito de Fade Selection

                if (!colorControll && albumSlots[indice].color.a > 0)
                {

                    Color color = albumSlots[indice].color;
                    color.a -= 0.2f;
                    albumSlots[indice].color = color;
                    if (!(albumSlots[indice].color.a > 0f))
                    {
                        colorControll = true;
                    }

                }
                else if (colorControll && albumSlots[indice].color.a < 1)
                {

                    Color color = albumSlots[indice].color;
                    color.a += 0.2f;
                    albumSlots[indice].color = color;
                    if (!(albumSlots[indice].color.a < 1f))
                    {
                        colorControll = false;
                    }

                }

////////////////////

            }

        }
        else if (PauseGame.pauseState != PauseStates.SUBMENU || albumState == AlbumStates.OUTALBUM)
        {
            if (lastCatcher != 0)
            {
                albumSlots[indice].color = Color.white;
            }
            indice = 0;
            scrollAlbum.value = 1;
            albumState = AlbumStates.OUTALBUM;

        }

        if (Input.GetKeyDown(KeyCode.Escape) && albumState == AlbumStates.INALBUM && count > 0.2f)
        {
            ExitAlbum();
            count = 0;
        }

        if (Input.GetKeyDown(KeyCode.Escape) && albumState == AlbumStates.ONPREVIEW && count > 0.2f)
        {
            ExitAlbumPreview();
            count = 0;
        }

    }

    void AtualizarCatcher()
    {
        
        albumSlots.Clear();

        foreach (Transform child in albumImagesGroup.transform)
        {
            albumSlots.Add(child.GetComponent<RawImage>());
        }

        for (int i = 0; i < photo_Quantity; i++)
        {
            albumSlots[i].texture = photoAlbum[i];
        }

        lastCatcher = photo_Quantity;

    }

    public void NewPic()
    {
        photo_Quantity++;
        Instantiate(new GameObject(("Foto ")).AddComponent(typeof(RawImage)), albumImagesGroup.transform);
    }

    public void ExitAlbum()
    {

        StartCoroutine("Exit");
        
    }

    IEnumerator Exit()
    {
        yield return new WaitForSeconds(0.1f);
        albumSlots[indice].color = Color.white;
        albumState = AlbumStates.OUTALBUM;
        pauseRef.Gallery();
    }

    public void ExitAlbumPreview()
    {

        StartCoroutine("Disable_Preview");
        albumState = AlbumStates.INALBUM;

    }

    IEnumerator Disable_Preview()
    {
        yield return new WaitForSeconds(0.1f);
        preview_Image.enabled = false;
    }

    public void EnablePreview()
    {
        StartCoroutine("Enable_Preview");
        albumState = AlbumStates.ONPREVIEW;
    }

    IEnumerator Enable_Preview()
    {
        yield return new WaitForSeconds(0.1f);
        preview_Image.enabled = true;
    }

    public void ActivateAlbum()
    {

        StartCoroutine("Enable") ;

    }

    IEnumerator Enable()
    {
        yield return new WaitForSeconds(0.1f);
        albumState = AlbumStates.INALBUM;
    }

}
