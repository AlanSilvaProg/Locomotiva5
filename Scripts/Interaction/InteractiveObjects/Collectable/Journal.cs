using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Journal : MonoBehaviour
{

    [Range(-1, 1)]
    public int side;

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Act()
    {
        print("página aberta e lado certo");
    }

}
