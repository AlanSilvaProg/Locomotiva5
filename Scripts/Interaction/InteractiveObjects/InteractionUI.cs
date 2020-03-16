using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionUI : MonoBehaviour
{

    public Canvas canvas_Interactive;
    public float size = 1;
    private bool avaible = true;

    // Start is called before the first frame update
    void Start()
    {
        canvas_Interactive.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {


        canvas_Interactive.transform.localScale = Vector3.Distance(Camera.main.transform.position, transform.position) * new Vector3(size,size,size);

        transform.LookAt(Camera.main.transform);

        if (!avaible)
        {
            canvas_Interactive.enabled = false;
        }

    }

    private void OnTriggerStay(Collider col)
    {

        if (col.tag == ("Player"))
        {
            canvas_Interactive.enabled = true;
        }

    }

    private void OnTriggerExit(Collider col)
    {

        if (col.tag == ("Player"))
        {
            canvas_Interactive.enabled = false;
        }

    }

    public void Block()
    {
        avaible = false;
    }

    public void Unlock()
    {
        avaible = true;
    }

}
