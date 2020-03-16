using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallEnemy : MonoBehaviour
{

    private GameManager gmRef;

    [Header("Quando jogador Morrer: Será adicionado pontos para um final ruim?")]
    [Tooltip("Quando interagir com o objeto, quando verdadeiro, irá adicionar pontos para um final ruim")]
    public bool addBadPointWhenDie;

    [Header("Pontos que seram adicionados ao contador de final ruim?")]
    [Tooltip("Quando interagir com o objeto, irá adicionar esta quantidade de pontos para um final ruim")]
    public int badPoints;

    public GameObject thisChaseMonsterControll;
    public GameObject thisChaseSystemControll;

    private void Start()
    {
        gmRef = GameManager.gmRef;
    }

    

    private void OnColliderEnter(Collider col)
    {

        if (col.tag == "Player")
        {
            if (gmRef.neverDie)
            {
                thisChaseMonsterControll.SendMessage("CheatMonster");
                return;
            }

            if (addBadPointWhenDie)
            {
                gmRef.badEnding += badPoints;
            }

        }

    }


}
