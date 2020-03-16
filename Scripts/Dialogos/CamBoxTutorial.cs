using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CamBoxTutorial : MonoBehaviour
{

    public static bool iniciarTutorial;

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

    public string[] tutorialText;
    
    public GameObject nextTutorialObject;

    private GameObject ui_Tutorial;

    private Text ui_TutorialText;

    private bool ativo = false;

    private int current_Tuto = 0;

    private int controll = 0;

    private void Awake()
    {
        CanvasRefFetch tempRefFetch = GameObject.Find("CanvasRefFetch").GetComponent<CanvasRefFetch>();
        ui_Tutorial = tempRefFetch.TUTORIAL;
        ui_TutorialText = tempRefFetch.TUTORIAL_TEXT;
    }

    private void Start()
    {

        gmRef = GameManager.gmRef;

    }

    private void Update()
    {

        if (iniciarTutorial && !ativo && current_Tuto == 0)
        {
            ativo = true;
            EnableTutorial();
        }

        if (iniciarTutorial && !ativo && current_Tuto == 1 )
        {
            ativo = true;
            EnableTutorial();
        }

        if (ativo && current_Tuto == 1 && Input.GetKeyDown(KeyCode.Tab) && CamPhotoManager.camState == CamState.DISABLED && Player_Interact.playerStates == PlayerStates.NOTHING && controll == 0)
        {
            controll++;
            current_Tuto++;
            EnableTutorial();
        }

        if (ativo && current_Tuto == 2 && Input.GetKeyDown(KeyCode.Mouse0) && CamPhotoManager.camState == CamState.ACTIVED && controll == 1)
        {
            controll++;
            current_Tuto++;
            EnableTutorial();
        }

        if (ativo && current_Tuto == 3 && Input.GetKeyDown(KeyCode.Tab) && CamPhotoManager.camState == CamState.ACTIVED && controll == 2)
        {
            EndTutorial();
        }


    }

    public void Interaction()
    {
        if (current_Tuto == 0)
        {
            ui_Tutorial.SetActive(false);
            current_Tuto++;
            iniciarTutorial = false;
            ativo = false;

            nextTutorialObject.SendMessage("Interaction");
        }

    }

    void EnableTutorial()
    {

        ui_Tutorial.SetActive(true);
        ui_TutorialText.text = "" + tutorialText[current_Tuto];

    }

    void EndTutorial()
    {
        ui_TutorialText.text = "";
        ui_Tutorial.SetActive(false);
        iniciarTutorial = false;
        ativo = false;

        if (addBadPoint)
        {
            gmRef.badEnding++;
        }

        if (addGoodPoint)
        {
            gmRef.goodEnding++;
        }

        Destroy(this);
    }

}
