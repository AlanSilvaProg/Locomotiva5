using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public enum PauseStates { ONPAUSE, SUBMENU, OUTPAUSE}

public class PauseGame : MonoBehaviour {

    public static PauseStates pauseState;

    public static PauseGame pauseRef;

    public float unpauseDelay = 1.5f;
    public GameObject player;
	public Transform canvas;
	private bool paused;
    public Animator phoneAnimator;
    public Animator phoneCanvasAnimator;
    public GameObject buttons;
    public Animator buttons_Yes, buttons_No;
    private FirstPersonController fpsController;

    private int indice = 0;
    private GameObject[] buttons_Pause;
    private Animator[] buttons_Anim;

    private float count;
    private Album albumRef;

    private void Awake()
    {

        pauseRef = this;

        count = 0;
        buttons_Pause = new GameObject[5];
        buttons_Anim = new Animator[5];
        int i = 0;
        foreach (Transform child in buttons.transform)
        {
            buttons_Pause[i] = child.gameObject;
            buttons_Anim[i] = buttons_Pause[i].GetComponent<Animator>();
            i++;
        }

    }

    void Start ()
    {

        albumRef = Album.albumRef;

        pauseState = PauseStates.OUTPAUSE;

		paused = false;
        
		fpsController = player.GetComponent<FirstPersonController>();
	}


	void Update ()
    {

        count += Time.deltaTime;

        if (pauseState == PauseStates.ONPAUSE)
        {

            if (Input.GetKeyDown(KeyCode.Return))
            {

                switch (indice)
                {

                    case 0:
                        buttons_Anim[indice].SetTrigger("Pressed");
                        Continue();
                        buttons_Anim[indice].SetTrigger("Normal");
                        indice = 0;
                        break;
                    case 1:
                        buttons_Anim[indice].SetTrigger("Pressed");
                        Camera();
                        buttons_Anim[indice].SetTrigger("Normal");
                        indice = 0;
                        break;
                    case 2:
                        if (Album.photo_Quantity != 0)
                        {
                            buttons_Anim[indice].SetTrigger("Pressed");
                            Gallery();
                            buttons_Anim[indice].SetTrigger("Normal");
                            indice = 0;
                        }
                        break;
                    case 3:
                        buttons_Anim[indice].SetTrigger("Pressed");
                        Options();
                        buttons_Anim[indice].SetTrigger("Normal");
                        indice = 0;
                        break;
                    case 4:
                        buttons_Anim[indice].SetTrigger("Pressed");
                        Quit();
                        buttons_Anim[indice].SetTrigger("Normal");
                        indice = 0;
                        break;

                }

            }

            if (Input.GetKeyDown(KeyCode.S) && indice < buttons_Pause.Length - 1 && count > 0.2f)
            {
                buttons_Anim[indice].SetTrigger("Normal");
                indice++;
                buttons_Anim[indice].SetTrigger("Highlighted");
                count = 0;
            }

            if (Input.GetKeyDown(KeyCode.W) && indice > 0 && count > 0.2f)
            {
                buttons_Anim[indice].SetTrigger("Normal");
                indice--;
                buttons_Anim[indice].SetTrigger("Highlighted");
                count = 0;
            }

        }

        if (pauseState == PauseStates.OUTPAUSE && Input.GetKeyDown(KeyCode.Escape) && Player_Interact.playerStates == PlayerStates.NOTHING && InvestigationLook.permissionToInvestigation == Permission.UNPERMITTED && CamPhotoManager.camState == CamState.DISABLED && count > 0.2f)
        {

            pauseState = PauseStates.ONPAUSE;

            StartCoroutine(Pause(unpauseDelay));

            count = 0;

            for (int i = 0; i < 5; i++)
            {
                if (i == 0)
                {
                    buttons_Anim[i].SetTrigger("Highlighted");
                }
                else
                {
                    buttons_Anim[i].SetTrigger("Normal");
                }
            }

        }


    }

    private void LateUpdate()
    {

        if (Input.GetKeyDown(KeyCode.Escape) && pauseState == PauseStates.ONPAUSE && count > 0.2f)
        {
            Continue();
            count = 0;
        }

    }

    IEnumerator Pause (float waitTime) {


        if (!paused)
        {
            paused = true;
            phoneAnimator.SetTrigger("PauseIn");
            phoneCanvasAnimator.SetTrigger("FadeIn");
			fpsController.enabled = false;


            canvas.gameObject.SetActive(true);
        }

        else if (paused && Input.GetKeyDown(KeyCode.Escape))
        {
			
            phoneAnimator.SetTrigger("PauseOut");
            phoneCanvasAnimator.SetTrigger("FadeOut");

            yield return new WaitForSeconds(waitTime);
            paused = false;
            canvas.gameObject.SetActive(false);
            fpsController.enabled = true;
            pauseState = PauseStates.OUTPAUSE;

        }
	}

    IEnumerator Unpause(float waitTime)
    {

        if (paused)
        {

            phoneAnimator.SetTrigger("PauseOut");
            phoneCanvasAnimator.SetTrigger("FadeOut");


            yield return new WaitForSeconds(waitTime);
            paused = false;
            canvas.gameObject.SetActive(false);
            fpsController.enabled = true;
            pauseState = PauseStates.OUTPAUSE;

        }
    }

    IEnumerator UnpauseWithCam(float waitTime)
    {

        if (paused)
        {

            phoneAnimator.SetTrigger("PauseOut");
            phoneCanvasAnimator.SetTrigger("FadeOut");


            yield return new WaitForSeconds(waitTime);
            paused = false;
            canvas.gameObject.SetActive(false);
            fpsController.enabled = true;
            pauseState = PauseStates.OUTPAUSE;
            (GameObject.FindObjectOfType(typeof(Player_Interact)) as Player_Interact).SendMessage("CelularEnable_Trigger");

        }
    }

    public void Continue()
    {
        StartCoroutine(Unpause(unpauseDelay));
    }

    public void Camera()
    {

        StartCoroutine(UnpauseWithCam(unpauseDelay));

    }

    public void Gallery()
    {

        if (pauseState == PauseStates.SUBMENU)
        {
            pauseState = PauseStates.ONPAUSE;
            phoneCanvasAnimator.SetTrigger("GalleryOut");
            indice = 0;
            buttons_Anim[indice].SetTrigger("Highlighted");

        }
        else
        {
            pauseState = PauseStates.SUBMENU;
            phoneCanvasAnimator.SetTrigger("GalleryIn");
            albumRef.ActivateAlbum();
        }

    }

    public void Options()
    {

    }

    public void Quit()
    {

    }

}
