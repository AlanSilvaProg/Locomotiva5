using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialBlockCuller : MonoBehaviour
{
    [SerializeField]
    private GameObject[] startOff;
    // Start is called before the first frame update
    void Start()
    {

        foreach (GameObject obj in startOff)
        {
            obj.gameObject.SetActive(false);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
