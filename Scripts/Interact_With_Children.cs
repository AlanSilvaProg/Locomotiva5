using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact_With_Children : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Interaction()
    {
        foreach(Transform child in transform)
        {
            
            child.gameObject.SendMessage("Interaction");
           
        }
    }

    private void Act()
    {
        foreach (Transform child in transform)
        {

            child.gameObject.SendMessage("Act");

        }
    }

}
