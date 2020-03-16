using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEvent : MonoBehaviour
{

    public bool unloockDoor;

    public GameObject door;

    private void OnTriggerEnter(Collider col)
    {

        if(col.tag == "Player")
        {
            if (unloockDoor)
            {
                door.SendMessage("UnloockDoor");
                Destroy(this.gameObject);
            }
        }

    }


}
