using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCuller : MonoBehaviour
{
    [SerializeField]
    private GameObject forwardsDisableBase;
    [SerializeField]
    private GameObject forwardsEnableBase;

    [SerializeField]
    private GameObject backwardsDisableBase;
    [SerializeField]
    private GameObject backwardsEnableBase;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        
        if(other.tag == "Player")
        {
            
            Vector3 toplayer = other.transform.position - transform.position;
            float mathtemp = Vector3.Dot(toplayer, transform.forward);
         
            if (mathtemp > 0)
            {
                ForwardFunc();
            }
            else
            {
                BackwardFunc();
            }
        }
    }

    private void ForwardFunc()
    {
        
        if (forwardsDisableBase != null)
        {
            forwardsDisableBase.gameObject.SetActive(false);
    
        }
        
        if (forwardsEnableBase != null)
        {

                forwardsEnableBase.gameObject.SetActive(true);
       
        }
        
    }

    private void BackwardFunc()
    {
        
        if (backwardsDisableBase != null)
        {
        
                backwardsDisableBase.gameObject.SetActive(false);
          
        }

        if (backwardsEnableBase != null)
        {
        
                backwardsEnableBase.gameObject.SetActive(true);
          
        }
    }


}
