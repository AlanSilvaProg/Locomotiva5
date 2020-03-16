using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterStates { ON, OFF}

public class ChaseMonster : MonoBehaviour
{

    private MonsterStates monsterStates = MonsterStates.OFF;
    [HideInInspector]
    public ChaseMonster chaseMonster;

    public Transform monster_Wall;

    public float veloEnemy;

    public float timeToAwake;

    public float timeToUnFreeze;


    private ChaseSystem refChaseSystem;
    private Transform target;
    private bool permission;
    private bool freeze;
    private float countUnFreeze;

    private float veloCheat;

    private GameManager gmRef;

    private void Awake()
    {
        veloCheat = veloEnemy;
        monsterStates = MonsterStates.OFF;
        chaseMonster = this;
    }

    void Start()
    {

        gmRef = GameManager.gmRef;
        refChaseSystem = transform.GetComponent<ChaseSystem>().chaseSystem;
        monster_Wall.gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {

        if (monsterStates == MonsterStates.OFF) { monster_Wall.gameObject.SetActive(false); }


        if (refChaseSystem.chaseSystemState == ChaseStates.ON)
        {

            if (monsterStates == MonsterStates.OFF)
            {

                monster_Wall.gameObject.SetActive(false);
                freeze = true;
                permission = false;
                countUnFreeze = 0;

            }

            if (monsterStates == MonsterStates.ON)
            {

                if (!permission)
                {

                    monster_Wall.gameObject.SetActive(true);
                    permission = true;

                }

                if (!freeze)
                {
                    if (gmRef.neverDie)
                    {
                        if (Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, monster_Wall.GetChild(0).transform.position) > 5)
                        {
                            monster_Wall.transform.position = Vector3.MoveTowards(monster_Wall.transform.position, new Vector3(monster_Wall.transform.position.x, monster_Wall.transform.position.y, monster_Wall.transform.forward.z * 100), veloEnemy * Time.deltaTime);
                        }
                    }
                    else
                    {
                        monster_Wall.transform.position = Vector3.MoveTowards(monster_Wall.transform.position, new Vector3(monster_Wall.transform.position.x, monster_Wall.transform.position.y, monster_Wall.transform.forward.z * 100), veloEnemy * Time.deltaTime);
                    }

                }

            }

        }
        else
        {

            if (monsterStates == MonsterStates.ON)
            {
                monsterStates = MonsterStates.OFF;
            }

        }
        
        
    }

    public void ChangePositionAndDestiny(Vector2 destiny)
    {

        monsterStates = MonsterStates.OFF;

        veloEnemy = veloCheat;

        monster_Wall.transform.rotation = refChaseSystem.chaseInfo.wagonsDestiny[(int)destiny.x].wagonDoors[(int)destiny.y].rotation;
        monster_Wall.transform.position = refChaseSystem.chaseInfo.wagonsDestiny[(int)destiny.x].wagonDoors[(int)destiny.y].position;

        if (destiny.y == 0)
        {
            target = refChaseSystem.chaseInfo.wagonsDestiny[(int)destiny.x].wagonDoors[1];
            target.position = new Vector3(target.position.x, transform.position.y, target.position.z);
        }
        else
        {
            target = refChaseSystem.chaseInfo.wagonsDestiny[(int)destiny.x].wagonDoors[0];
            target.position = new Vector3(target.position.x, transform.position.y, target.position.z);
        }

        StopAllCoroutines();

        StartCoroutine("Awakening");

    }


    public void onEnableChase(Transform firstLocation)
    {

        monsterStates = MonsterStates.OFF;

        monster_Wall.transform.rotation = firstLocation.rotation;
        monster_Wall.transform.position = firstLocation.position;

        StopAllCoroutines();

        StartCoroutine("Awakening");

    }

    IEnumerator Awakening()
    {

        yield return new WaitForSeconds(timeToAwake);
        monsterStates = MonsterStates.ON;
        StartCoroutine("UnFreeze");

    }

    IEnumerator UnFreeze()
    {

        yield return new WaitForSeconds(timeToUnFreeze);
        freeze = false;

    }

    public void CheatMonster()
    {
        print("aaaah");
        veloEnemy = 0;
    }

}
