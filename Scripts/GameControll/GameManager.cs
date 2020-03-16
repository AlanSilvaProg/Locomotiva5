using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum GameState { INGAME, PAUSE}

public enum PauseState { GALERIA}

public class GameManager : MonoBehaviour
{

    public static bool firstChaseAct;

    public static GameManager gmRef;

    public static GameObject actualChaseControll;

    public Image death_FadeImage;

    [HideInInspector]
    public bool neverDie;
    [HideInInspector]
    public bool unloockAll;
    [HideInInspector]
    public bool goToEnd;
    [HideInInspector]
    public int goodEnding;
    [HideInInspector]
    public int badEnding;

    public bool enableCheats;

    public string goodSceneName;
    public string badSceneName;

    public static GameState gameStates;
    private PauseState pauseState;

    private bool die;

    private GameObject player;

    [HideInInspector]
    public Transform positionToLoad;
    [HideInInspector]
    public List<GameObject> enableOnLoad;
    [HideInInspector]
    public List<GameObject> disableOnLoad;

    private void Awake()
    {
        gmRef = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        GameObject.FindGameObjectWithTag("Loading").GetComponent<Text>().enabled = false;
        death_FadeImage = GameObject.FindGameObjectWithTag("DeathFade").GetComponent<Image>();
        player = GameObject.FindGameObjectWithTag("Player");
        enableOnLoad = new List<GameObject>();
        disableOnLoad = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {

        if (firstChaseAct)
        {
            PlayerPrefs.SetInt("Load", 1);
        }

        if (enableCheats)
        {

            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                neverDie = true;
            }

            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                unloockAll = true;
            }

            if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                goodEnding = 100;
                badEnding = 0;
            }

            if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                goodEnding = 0;
                badEnding = 100;
            }

            if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                EndGame();
            }

        }

        if (die)
        {
            if(death_FadeImage.color.a == 1)
            {
                LoadGame();
            }
        }

    }

    public void Die()
    {

        Player_Interact.playerStates = PlayerStates.INTERAGINDO;
        die = true;

    }

    public void LoadGame()
    {

        die = false;

        player.transform.rotation = positionToLoad.rotation;
        player.transform.position = positionToLoad.position;

        Player_Interact.playerStates = PlayerStates.NOTHING;

        foreach (GameObject obj in enableOnLoad)
        {
            obj.SetActive(true);
        }

        foreach (GameObject obj in disableOnLoad)
        {
            obj.SetActive(false);
        }

    }

    public void EndGame()
    {

        if(goodEnding > badEnding)
        {
            SceneManager.LoadScene(goodSceneName);
        }
        else
        {
            SceneManager.LoadScene(badSceneName);
        }

    }

}
