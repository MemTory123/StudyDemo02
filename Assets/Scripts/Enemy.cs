using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

    public PathNode m_currentNode;

    // 生命
    public int m_life = 15;
    public int m_maxlife = 15;

    // 移动速度
    public float m_speed = 2;

    // 死亡动作
    public System.Action<Enemy> onDeath;

    // 生命条
    Transform m_lifebarObj;
    UnityEngine.UI.Slider m_lifebar;

	// Use this for initialization
	void Start () {

        GameManager.Instance.m_EnemyList.Add(this);

        // 在场景中找到3D UI 根节点
        GameObject uiroot3d = GameObject.Find("Canvas3D");
        // 读取生命条prefab
        GameObject prefab = (GameObject)Resources.Load("ui_lifebar");
        // 创建生命条
        GameObject lifebarobj = (GameObject)Instantiate(prefab);
        m_lifebarObj = lifebarobj.transform;
        m_lifebarObj.parent = uiroot3d.transform;
        m_lifebarObj.localScale = new Vector3(0.02f, 0.02f, 0.02f);
        m_lifebar = m_lifebarObj.GetComponent<UnityEngine.UI.Slider>();
        // 更新生命条位置和角度
        UpdateLifebar();
	}
	
	// Update is called once per frame
	void Update () {

        RotateTo();
        MoveTo();
        UpdateLifebar();
	}

    // 旋转向目标
    public void RotateTo()
    {
        float current= this.transform.eulerAngles.y;

        this.transform.LookAt(m_currentNode.transform);

        float next=Mathf.MoveTowardsAngle(current, this.transform.eulerAngles.y, 120 * Time.deltaTime);

        this.transform.eulerAngles = new Vector3(0, next, 0);
    }

    // 旋转向目标
    public void MoveTo()
    {
        Vector3 pos1 = this.transform.position;
        Vector3 pos2 = m_currentNode.transform.position;
        float dist = Vector2.Distance(new Vector2(pos1.x,pos1.z),new Vector2(pos2.x,pos2.z));
        if (dist < 1.0f)
        {
            if (m_currentNode.m_next == null) // 如果到达我方基地
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
        // 删除生命条
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
            // 每消灭一个敌人增加一些铜钱
            GameManager.Instance.SetPoint(5);
            DestroyMe();
        }
    }

    void UpdateLifebar()
    {
        // 更新生命条
        m_lifebar.value = (float)m_life / (float)m_maxlife;
        // 更新位置
        Vector3 lifebarpos = this.transform.position;
        lifebarpos.y += 2.0f;
        m_lifebarObj.transform.position = lifebarpos;
        // 更新角度
        m_lifebarObj.transform.eulerAngles = Camera.main.transform.eulerAngles;
    }

}
