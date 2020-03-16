using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ChaseStates { ON, OFF}

[System.Serializable]
public class ChaseInfo
{

    [Header("1 Elemento por vagão")]
    public WagonDoors[] wagonsDestiny;

    [Header("Quantidade de vagões que irão passar no ChaseSequence")]
    [Tooltip("Quantidade respectiva ao total de vagões em que o player irá ter que passar dentro do chase")]
    public int amountOfWagon;

    [Header("O jogador deve ir para um lugar especifico no fim da chase?")]
    [Tooltip("Marcar apenas nas portas de chase, onde será verificado se o jogador terá um destino especifico no final de uma chase sequence... Quando acabar, ele será movido para esse lugar")]
    public bool specificDestination_OnEndChase;

    [Header("Local para onde o jogador será movido no final da chase")]
    [Tooltip("Se o specificDestination_OnEndChase for true, o jogador será movido para está posição, quando acabar a chase sequence")]
    public Transform destinationOnEndChase;

    public GameObject[] enable;
    public GameObject[] disable;
    public GameObject[] enable_Disable;
    public GameObject[] disable_Enable;
    public GameObject[] enable_End;
    public GameObject[] disable_End;

}

[System.Serializable]
public class WagonDoors
{
    [Header("2 Destinos de Portas por Elemento")]
    public Transform[] wagonDoors = new Transform[2];
}

public class ChaseSystem : MonoBehaviour
{

    public static ChaseSystem chaseRef;
    
    [Header("Primeiro lugar em que o monstro será spawnado")]
    [Tooltip("Quando a chase for ativada o monstro será spawnado neste ponto")]
    public Transform firstWallSpawn;

    [Space(2)]

    [HideInInspector]
    public ChaseSystem chaseSystem;
    [HideInInspector]
    public Transform lastNonChaseWagon;
    [HideInInspector]
    public Vector2 actualChaseWagonAndEntry;
    [HideInInspector]
    public int wagonCount;
    [HideInInspector]
    public Transform newWagonDestiny;
    [HideInInspector]
    public ChaseStates chaseSystemState;

    public ChaseInfo chaseInfo;

    private ChaseMonster chaseMRef;

    public bool triggerActiveTest;

    private ReactingSoundScript reactions;

    // Start is called before the first frame update
    private void Awake()
    {
        chaseSystemState = ChaseStates.OFF;
        chaseSystem = this;
    }

    void Start()
    {
        chaseMRef = transform.GetComponent<ChaseMonster>().chaseMonster;
        reactions = GameObject.FindGameObjectWithTag("PlayerAudioFalas").GetComponent<ReactingSoundScript>();
        
    }

    // Update is called once per frame
    void Update()
    {

        if (triggerActiveTest)
        {
            ChaseSetActive(true);
            triggerActiveTest = false;
        }
        
    }

    public void NewWagon(Vector2 actual)
    {

        actualChaseWagonAndEntry = actual;
        chaseMRef.SendMessage("ChangePositionAndDestiny", actualChaseWagonAndEntry);

    }

    public void ChaseSetActive(bool setActive)
    {

        GameManager.firstChaseAct = true;

        reactions.SendMessage("PlayReaction","desespero");
        if(firstWallSpawn == null)
        {
            chaseMRef.SendMessage("onEnableChase");
        }
        else
        {
            chaseMRef.SendMessage("onEnableChase", firstWallSpawn);
        }
        

        if (setActive)
        {
            if (chaseInfo.enable.Length > 0)
            {
                for (int i = 0; i < chaseInfo.enable.Length; i++)
                {

                    chaseInfo.enable[i].SetActive(setActive);
                    
                }
            }

            if (chaseInfo.enable_Disable.Length > 0)
            {
                for (int i = 0; i < chaseInfo.enable_Disable.Length; i++)
                {

                    chaseInfo.enable_Disable[i].SetActive(setActive);

                }
            }

            if (chaseInfo.disable.Length > 0)
            {
                for (int i = 0; i < chaseInfo.disable.Length; i++)
                {
                    
                    chaseInfo.disable[i].SetActive(!setActive);

                }
            }

            if (chaseInfo.disable_Enable.Length > 0)
            {
                for (int i = 0; i < chaseInfo.disable.Length; i++)
                {

                    chaseInfo.disable_Enable[i].SetActive(!setActive);

                }
            }

            wagonCount = 0;
            chaseSystemState = ChaseStates.ON;

            GameManager.actualChaseControll = this.gameObject;

        }
        else if (!setActive)
        {

            if (chaseInfo.enable_Disable.Length > 0)
            {
                for (int i = 0; i < chaseInfo.enable_Disable.Length; i++)
                {

                    chaseInfo.enable_Disable[i].SetActive(setActive);

                }
            }

            if (chaseInfo.disable_Enable.Length > 0)
            {
                for (int i = 0; i < chaseInfo.disable_Enable.Length; i++)
                {

                    chaseInfo.disable_Enable[i].SetActive(!setActive);

                }
            }

            if (chaseInfo.enable_End.Length > 0)
            {
                for (int i = 0; i < chaseInfo.enable_End.Length; i++)
                {

                    chaseInfo.enable_End[i].SetActive(!setActive);

                }
            }

            if (chaseInfo.disable_End.Length > 0)
            {
                for (int i = 0; i < chaseInfo.disable_End.Length; i++)
                {

                    chaseInfo.disable_End[i].SetActive(setActive);

                }
            }

            chaseSystemState = ChaseStates.OFF;

        }

    }

}