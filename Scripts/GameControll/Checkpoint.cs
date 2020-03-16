using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{

    [Header("Objetos para salvar seu atual estado")]
    [Tooltip("Se o objeto em questão estiver desabilitado, o load será feito e esse objeto será desativado e vici versa")]
    public GameObject[] saveObjectState;

    private Transform positionToSave;

    private GameManager gmRef;

    // Start is called before the first frame update
    void Start()
    {
        gmRef = GameManager.gmRef;
        positionToSave = transform;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Player")
        {

            gmRef.positionToLoad = positionToSave;
            int i = 0;
            gmRef.disableOnLoad.Clear();
            gmRef.enableOnLoad.Clear();

            foreach (GameObject obj in saveObjectState)
            {
                if (saveObjectState[i].activeSelf)
                {
                    gmRef.enableOnLoad.Add(obj);
                }
                else
                {
                    gmRef.disableOnLoad.Add(obj);
                }
                i++;
            }

            GetComponent<BoxCollider>().enabled = false;

        }

    }

}