using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

    public PathNode m_currentNode;

    // ����
    public int m_life = 15;
    public int m_maxlife = 15;

    // �ƶ��ٶ�
    public float m_speed = 2;

    // ��������
    public System.Action<Enemy> onDeath;

    // ������
    Transform m_lifebarObj;
    UnityEngine.UI.Slider m_lifebar;

	// Use this for initialization
	void Start () {

        GameManager.Instance.m_EnemyList.Add(this);

        // �ڳ������ҵ�3D UI ���ڵ�
        GameObject uiroot3d = GameObject.Find("Canvas3D");
        // ��ȡ������prefab
        GameObject prefab = (GameObject)Resources.Load("ui_lifebar");
        // ����������
        GameObject lifebarobj = (GameObject)Instantiate(prefab);
        m_lifebarObj = lifebarobj.transform;
        m_lifebarObj.parent = uiroot3d.transform;
        m_lifebarObj.localScale = new Vector3(0.02f, 0.02f, 0.02f);
        m_lifebar = m_lifebarObj.GetComponent<UnityEngine.UI.Slider>();
        // ����������λ�úͽǶ�
        UpdateLifebar();
	}
	
	// Update is called once per frame
	void Update () {

        RotateTo();
        MoveTo();
        UpdateLifebar();
	}

    // ��ת��Ŀ��
    public void RotateTo()
    {
        float current= this.transform.eulerAngles.y;

        this.transform.LookAt(m_currentNode.transform);

        float next=Mathf.MoveTowardsAngle(current, this.transform.eulerAngles.y, 120 * Time.deltaTime);

        this.transform.eulerAngles = new Vector3(0, next, 0);
    }

    // ��ת��Ŀ��
    public void MoveTo()
    {
        Vector3 pos1 = this.transform.position;
        Vector3 pos2 = m_currentNode.transform.position;
        float dist = Vector2.Distance(new Vector2(pos1.x,pos1.z),new Vector2(pos2.x,pos2.z));
        if (dist < 1.0f)
        {
            if (m_currentNode.m_next == null) // ��������ҷ�����
            {
                GameManager.Instance.SetDamage(1);
                DestroyMe();
            }
            else
                m_currentNode = m_currentNode.m_next;
        }

        this.transform.Translate(new Vector3(0, 0, m_speed * Time.deltaTime));

        //m_bar.SetPosition(this.transform.position, 4.0f);
    }

    public void DestroyMe()
    {
        // ɾ��������
        Destroy(m_lifebarObj.gameObject);

        GameManager.Instance.m_EnemyList.Remove(this);
        onDeath(this);
        Destroy(this.gameObject);
    }

    public void SetDamage(int damage)
    {
        m_life -= damage;
        if (m_life <= 0)
        {
            m_life = 0;
            // ÿ����һ����������һЩͭǮ
            GameManager.Instance.SetPoint(5);
            DestroyMe();
        }
    }

    void UpdateLifebar()
    {
        // ����������
        m_lifebar.value = (float)m_life / (float)m_maxlife;
        // ����λ��
        Vector3 lifebarpos = this.transform.position;
        lifebarpos.y += 2.0f;
        m_lifebarObj.transform.position = lifebarpos;
        // ���½Ƕ�
        m_lifebarObj.transform.eulerAngles = Camera.main.transform.eulerAngles;
    }

}
