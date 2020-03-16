using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollScale : MonoBehaviour
{
    public GameObject[] photos;
    //Array que controla a quantidade de fotos
    private float spacingSize;
    //Numero de linhas e espaço entre elas
    public GridLayoutGroup layoutInfo;
    //contem os valores para se atualizar o tamanho
    public RectTransform rt;
    //tamanho da HUD
    private Vector2 rtXY;
    private float rtY;
    //vector 2 para atualização do tamanho da HUD
    
    

    // Start is called before the first frame update
    void Start()
    {
        RectTransform rt = this.GetComponent<RectTransform>();
        rtXY = rt.sizeDelta;
    }

    // Update is called once per frame
    void Update()
    {
        spacingSize = Mathf.RoundToInt(photos.Length/2);

        //Calcula o tamanho da HUD para conter todos os elementos
        rtY = (layoutInfo.cellSize.y * (spacingSize + 1)) + (layoutInfo.spacing.y * spacingSize) + layoutInfo.padding.top + layoutInfo.padding.bottom;

        //Atualização
        if (rtY >= rtXY.y) 
        {
            rt.sizeDelta = new Vector2(rtXY.x, rtY);
        } else
        {
            rt.sizeDelta = new Vector2(rtXY.x, rtXY.y);
        }
    }
}
