using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCControl : MonoBehaviour {

    [SerializeField]
    private float distance = 5f;
    [Tooltip("填实际角度的1/4")]
    [SerializeField]
    private float sightAngel;
    [SerializeField]
    private float walkAroundTime;
    [SerializeField]
    private float walkAroundRadius;

    [SerializeField]
    private float patrolSpeed, runToTalkSpd, walkAroundSpd;

    public Transform curTarget;
    private MapManager mapManager;
    private PolyNavAgent navAgent;
    private PatrolRandomWaypoints randomPatrol;
    private PatrolWaypoints patrol;

    [SerializeField]
    GameObject mark1Pre,mark2Pre;

    //当前所处状态： 0=未发现； 1= 发现目标；2=被npc发现；3=对话中；4=寻找目标并前往；6=游荡中
    public int curState;
    public int preState; //前一个state

    void Awake()
    {
        navAgent = this.GetComponent<PolyNavAgent>();
        randomPatrol = this.GetComponent<PatrolRandomWaypoints>();
        patrol = this.GetComponent<PatrolWaypoints>();
        curTarget = GameObject.FindGameObjectWithTag("Player").transform;
        curState = 0;
    }

    void Start()
    {
        PatrolInSequence(); //按顺序巡逻
    }

    void Update()
    {
        switch (curState)
        {
            case -1:
                break;

            case 0:   //巡逻state    
                LookForPlayer();
                //CalculateSight(); //计算视线内是否有目标
                break;

            case 1:
                //找目标说话（）{换速度，指定目标位移，目标不能动，进行对话type=？}
                GoToTargetAndTalk();
                break;

            case 2:
                //被对话,改变tag为已知，等对话结束
                this.tag = "KnownNPC";
                StayAndWait();
                break;

            case 3:
                //等对话结束；进入游荡状态 state = 6；
                break;

            case 4:
                PatroToBoss();
                DetectNearbyTargets(); //去找boss路上看着点别人           
                break;

            case 6:
                PatrolRandomly();
                break;
        }
    }
    void LookForTarget()
    {
    }

    void StayAndWait()
    {
        this.navAgent.Stop();
        patrol.enabled = false;
        curState = -1;
    }
    void BackToPreState()
    {
        randomPatrol.enabled = false;
        curState = 4;
    }

    void PatrolInSequence()
    {
        patrol.enabled = true;
    }

    void PatroToBoss()
    {
        Transform boss = GameObject.FindGameObjectWithTag("Boss").transform;

    }

    void PatrolRandomly() //以当前位置为中心，设定半径来回瞎走
    {
        navAgent.maxSpeed = walkAroundSpd;
        float x = this.transform.position.x;
        float y = this.transform.position.y;
        float r = Random.Range(0.5f, walkAroundRadius);
        randomPatrol.WPoints.Add(new Vector2(x + r, y + r));
        randomPatrol.WPoints.Add(new Vector2(x - r, y + r));
        randomPatrol.WPoints.Add(new Vector2(x + r, y - r));
        randomPatrol.WPoints.Add(new Vector2(x - r, y - r));
        randomPatrol.enabled = true;
        print("应该开始到处晃");
        curState = -1;
        Invoke("BackToPreState", walkAroundTime);
    }

    void GoToTargetAndTalk()
    {
        if(curTarget.tag == "Player")
        {
            curTarget.GetComponent<PlayerControl>().isTalking = true;
        }else if (curTarget.tag == "UnknownNPC")
        {
            curTarget.GetComponent<NPCControl>().curState = 2;
        }
        print("应该往目标跑了！");
        Vector2 pos0 =  - (curTarget.transform.position - transform.position).normalized * 0.2f;
        Vector2 tarPos = new Vector2(curTarget.position.x, curTarget.position.y) + pos0;
        //int curPatrolPoint = patrol.currentIndex;
        navAgent.maxSpeed = runToTalkSpd;
        patrol.MoveToOnlyPoint(tarPos);
        //patrol.WPoints[curPatrolPoint+1] = tarPos;
        //patrol.MoveNext();
        patrol.enabled = false;
        Invoke("TalkAndWait",2f);
        curState = -1;//站着等对话结束改状态
    }

    void TalkAndWait()
    {
        GameObject duihua = Instantiate(mark2Pre, transform.position + new Vector3(0f, 0.1f, 0f), mark2Pre.transform.rotation) as GameObject;
        duihua.transform.SetParent(this.transform);
        curState = 6;
        if(curTarget.tag == "Player")
        {
            curTarget.GetComponent<PlayerControl>().isTalking = false;
        }
        
    }
    void LookForPlayer()
    {
        Collider2D playerInSight = Physics2D.OverlapCircle(transform.position, distance, LayerMask.GetMask("Player"));
        if(playerInSight != null)
        {
            DrawSight();
            Debug.DrawLine(transform.position, playerInSight.transform.position, Color.yellow);
            if(playerInSight.GetComponent<PlayerControl>().isTalking == false)
            {
                Vector3 pPos = playerInSight.transform.position;
                if (Vector3.Angle(transform.up, pPos - transform.position) < sightAngel * 2f)
                {
                    GameObject duihua = Instantiate(mark1Pre, transform.position + new Vector3(0f, 0.1f, 0f), mark2Pre.transform.rotation) as GameObject;
                    duihua.transform.SetParent(this.transform);
                    print("发现了主角！");
                    this.tag = "KnownNPC";
                    curState = 1;
                    //print(Vector3.Angle(transform.up, pPos - transform.position));
                    Debug.DrawLine(transform.position, playerInSight.transform.position, Color.red);
                }
            }     
        }        
    }


    private Collider2D[] nearbyTargets;
    void DetectNearbyTargets()
    {
        nearbyTargets = Physics2D.OverlapCircleAll(transform.position, distance, LayerMask.GetMask("NPC"));
       
        for(int i = 0; i < nearbyTargets.Length; i++)
        {
            //这些npc里找不知道的
            if(nearbyTargets[i].tag == "UnknownNPC")
            {
                DrawSight();
                Debug.DrawLine(transform.position, nearbyTargets[i].transform.position, Color.yellow);
                Vector3 nPos = nearbyTargets[i].transform.position;
                //看看在不在视线内
                if(Vector3.Angle(transform.up, nPos - transform.position) < sightAngel * 2f)
                {
                    //就是你了！来听八卦吧！
                    GameObject duihua = Instantiate(mark1Pre, transform.position + new Vector3(0f, 0.1f, 0f), mark2Pre.transform.rotation) as GameObject;
                    duihua.transform.SetParent(this.transform);
                    Debug.DrawLine(transform.position, nearbyTargets[i].transform.position, Color.red);
                    print("找到一个来八卦的！");
                    curTarget = nearbyTargets[i].transform;
                    //curTarget.GetComponent<NPCControl>().curState = 
                    //GoToTargetAndTalk();
                    curState = 1;
                    break;
                }
            }
        }
    }

    //画一下视野
    void DrawSight()
    {
        Quaternion r0 = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z - sightAngel * 2);
        Quaternion r1 = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z + sightAngel * 2);
        Vector3 f1 = (transform.position + (r0 * Vector3.up) * distance);
        Vector3 f2 = (transform.position + (r1 * Vector3.up) * distance);
        Debug.DrawLine(transform.position, f1, Color.green);
        Debug.DrawLine(transform.position, f2, Color.green);
    }

    /*
    void CalculateSight()
    {
        Quaternion r = transform.rotation;
        Vector3 f0 = (transform.position + (r * Vector3.up) * distance);
        Debug.DrawLine(transform.position, f0, Color.red); //中线

        //共4个三角形，除中线的四条边定点顺时针依次为f1,f2,f3,f4
        Quaternion r0 = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z - sightAngel * 2);
        Quaternion r1 = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z - sightAngel);
        Quaternion r2 = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z + sightAngel);
        Quaternion r3 = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z + sightAngel * 2);
        Vector3 f1 = (transform.position + (r0 * Vector3.up) * distance);
        Vector3 f2 = (transform.position + (r1 * Vector3.up) * distance);
        Vector3 f3 = (transform.position + (r2 * Vector3.up) * distance);
        Vector3 f4 = (transform.position + (r3 * Vector3.up) * distance);

        //左右两边和连线
        Debug.DrawLine(transform.position, f1, Color.red);
        Debug.DrawLine(transform.position, f2, Color.red);
        Debug.DrawLine(transform.position, f3, Color.red);
        Debug.DrawLine(transform.position, f4, Color.red);
        Debug.DrawLine(f1, f2, Color.red);
        Debug.DrawLine(f2, f0, Color.red);
        Debug.DrawLine(f0, f3, Color.red);
        Debug.DrawLine(f3, f4, Color.red);

        if (isINTriangle(curTarget.position, transform.position, f1, f2)|| isINTriangle(curTarget.position, transform.position, f2, f0) 
            || isINTriangle(curTarget.position, transform.position, f0, f3) || isINTriangle(curTarget.position, transform.position, f3, f4))
        {
            if(curTarget.tag == "Player")
            {
                this.tag = "KnownNPC";
            }
            curState = 1;
            //patrol.enabled = false;
        }
        else
        {
            //Debug.Log(curTarget.position + "cube not in this !!!");
        }
    }

    bool isINTriangle(Vector3 point, Vector3 v0, Vector3 v1, Vector3 v2)
    {
        float x = point.x;
        float y = point.y;
        //三角形
        float v0x = v0.x;
        float v0y = v0.y;
        float v1x = v1.x;
        float v1y = v1.y;
        float v2x = v2.x;
        float v2y = v2.y;
        
        //视角三角形
        float t = triangleArea(v0x, v0y, v1x, v1y, v2x, v2y);

        //目标点是否在视角三角形内，目标点和3个点分别组成三角形相加，若点在里面，则和与视角三角形相等
        float a = triangleArea(v0x, v0y, v1x, v1y, x, y) + triangleArea(v0x, v0y, x, y, v2x, v2y) + triangleArea(x, y, v1x, v1y, v2x, v2y);
        if (Mathf.Abs(t - a) <= 0.01f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private float triangleArea(float v0x, float v0y, float v1x, float v1y, float v2x, float v2y)
    {
        return Mathf.Abs((v0x * v1y + v1x * v2y + v2x * v0y
            - v1x * v0y - v2x * v1y - v0x * v2y) / 2f);
    }

    */
}
