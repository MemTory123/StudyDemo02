using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;

    public bool m_debug = true;

    public ArrayList m_PathNodes;

    public System.Collections.Generic.List<Enemy> m_EnemyList = 
        new System.Collections.Generic.List<Enemy>();

    public LayerMask m_groundlayer;

    public int m_wave = 1;
    public int m_waveMax = 10;

    public int m_life = 10;

    public int m_point = 30;

    Text m_txt_wave;
    Text m_txt_life;
    Text m_txt_point;
    Button m_but_try;

    bool m_isSelectedButton=false;

    void Awake()
    {
        Instance = this;
    }

	// Use this for initialization
	void Start () {

        UnityAction<BaseEventData> downAction = new UnityAction<BaseEventData>(OnButCreateDefenderDown);
        UnityAction<BaseEventData> upAction = new UnityAction<BaseEventData>(OnButCreateDefenderUp);

        EventTrigger.Entry down = new EventTrigger.Entry();
        down.eventID = EventTriggerType.PointerDown;
        down.callback.AddListener(downAction);

        EventTrigger.Entry up = new EventTrigger.Entry();
        up.eventID = EventTriggerType.PointerUp;
        up.callback.AddListener(upAction);


        foreach (Transform t in this.GetComponentsInChildren<Transform>())
        {
            if (t.name.CompareTo("wave") == 0)
            {
                m_txt_wave = t.GetComponent<Text>();
                SetWave(1);
            }
            else if (t.name.CompareTo("life") == 0)
            {
                m_txt_life = t.GetComponent<Text>();
                m_txt_life.text = string.Format("生命：<color=yellow>{0}</color>" , m_life);
            }
            else if (t.name.CompareTo("point") == 0)
            {
                m_txt_point = t.GetComponent<Text>();
                m_txt_point.text = string.Format("分数：<color=yellow>{0}</color>", m_point);
            }
            else if (t.name.CompareTo("but_try") == 0)
            {
                m_but_try = t.GetComponent<Button>();

                m_but_try.onClick.AddListener( OnButRetry );

                m_but_try.gameObject.SetActive(false);
              
            }
            else if (t.name.Contains("but_player"))
            {
                EventTrigger trigger = t.gameObject.AddComponent<EventTrigger>();
                trigger.triggers = new List<EventTrigger.Entry>();
                trigger.triggers.Add(down);
                trigger.triggers.Add(up);
            }
        }

        BuildPath();
	
	}

	// Update is called once per frame
	void Update () {

        if (m_isSelectedButton)
            return;

        Vector3 mousepos = Input.mousePosition;
	
        bool press=Input.GetMouseButton(0);

        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");

        GameCamera.Inst.Control(press, mx, my);
	}

    public void SetWave(int wave)
    {
        m_wave= wave;
        m_txt_wave.text = string.Format("波数：<color=yellow>{0}/{1}</color>", m_wave, m_waveMax);

    }

    public void SetDamage(int damage)
    {
        m_life -= damage;
        if (m_life <= 0) {
            m_life = 0;
            m_but_try.gameObject.SetActive(true);
        }
        m_txt_life.text = string.Format("生命：<color=yellow>{0}</color>", m_life);

    }

    public bool SetPoint(int point)
    {
        if (m_point + point < 0)
            return false;
        m_point += point;
        m_txt_point.text = string.Format("分数：<color=yellow>{0}</color>", m_point);

        return true;

    }

    void OnButRetry()
    {
        //Application.LoadLevel(Application.loadedLevelName);
        SceneManager.LoadScene(0);
    }

    void OnButCreateDefenderDown(BaseEventData data)
    {
        m_isSelectedButton = true;
    }

    void OnButCreateDefenderUp( BaseEventData data )
    {
        GameObject go = data.selectedObject;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitinfo;

        if (Physics.Raycast(ray, out hitinfo, 1000, m_groundlayer))
        {

            if (TileObject.get.getData(hitinfo.point.x, hitinfo.point.z) == (int)Defender.TileStatus.FREE)
            {

                Vector3 pos = new Vector3(hitinfo.point.x, 0, hitinfo.point.z);

                pos.x = (int)pos.x + TileObject.get.tileSize * 0.5f;
                pos.z = (int)pos.z + TileObject.get.tileSize * 0.5f;

                if (go.name.Contains("1"))
                {
                    if (SetPoint(-15))
                        Defender.Create<Defender>(pos, new Vector3(0, 180, 0));

                }
                else
                {
                    if (SetPoint(-20))
                        Defender.Create<Archer>(pos, new Vector3(0, 180, 0));
                }
            }
        }
        m_isSelectedButton = false;
    }

    [ContextMenu("BuildPath")]
    void BuildPath()
    {
        m_PathNodes = new ArrayList();

        GameObject[] objs = GameObject.FindGameObjectsWithTag("pathnode");

        for (int i = 0; i < objs.Length; i++)
        {
            PathNode node = objs[i].GetComponent<PathNode>();

            m_PathNodes.Add(node);
        }
    }


    void OnDrawGizmos()
    {
        if (!m_debug || m_PathNodes == null)
            return;

        Gizmos.color = Color.blue;

        foreach (PathNode node in m_PathNodes)
        {
            if (node.m_next != null)
            {
                Gizmos.DrawLine(node.transform.position, node.m_next.transform.position);
            }
        }
    }

}
