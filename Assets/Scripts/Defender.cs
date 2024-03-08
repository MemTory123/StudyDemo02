using UnityEngine;
using System.Collections;

public class Defender : MonoBehaviour {
    // ����״̬
    public enum TileStatus
    {
        LOCK = 0,   // �������������κ���
        ROAD=1,     // ר���ڵ�������
        FREE=2,     // ר���ڴ������ص�λ�ĸ���
    }

    // ������Χ
    public float m_attackArea = 2.0f;
    // ������
    public int m_power = 1;
    // ����ʱ����
    public float m_attackTime = 2.0f;
    // ����ʱ������ʱ��
    public float m_timer = 0.0f;
    // Ŀ�����
    protected Enemy m_targetEnemy;
    // ���ص�λ��ģ��
    protected GameObject m_model;
    // ���ص�λ�Ķ���
    protected Animation m_ani;

    // ��̬�������� �������ص�λʵ��
    public static T Create<T>( Vector3 pos, Vector3 angle ) where T : Defender 
    {
        GameObject go = new GameObject();
        go.transform.position = pos;
        go.transform.eulerAngles = angle;
        go.name = "defender";
        T d = go.AddComponent<T>();
        d.Init();

        // ���Լ���ռ���ӵ���Ϣ��Ϊռ��
        TileObject.get.setData(d.transform.position.x, d.transform.position.z, (int)TileStatus.LOCK);

        return d;
    }

    // ��ʼ����ֵ
    protected virtual void Init()
    {
        m_attackArea = 2.0f;
        m_power = 2;
        CreateModel("swordman");

    }

    // ����ģ��
    protected void CreateModel(string myname)
    {
        GameObject model = Resources.Load<GameObject>(myname + "@skin");
        GameObject ani_run = Resources.Load<GameObject>(myname + "@attack");
        GameObject ani_idle = Resources.Load<GameObject>(myname + "@idle");

        m_model = (GameObject)Instantiate(model, this.transform.position, this.transform.rotation);
        m_model.transform.parent = this.transform;
        m_ani = m_model.GetComponent<Animation>();
        m_ani.AddClip(ani_run.GetComponent<Animation>().GetClip("attack"), "attack");
        m_ani.AddClip(ani_idle.GetComponent<Animation>().GetClip("idle"), "idle");
        m_ani["attack"].wrapMode = WrapMode.Clamp;
        m_ani["idle"].wrapMode = WrapMode.Loop;
        m_ani.CrossFade("idle");
    }

	
	// Update is called once per frame
	void Update () {

        FindEnemy();
        RotateTo();
        Attack();
	}

    public bool RotateTo()
    {
        if (m_targetEnemy == null)
            return true;
        // �Ƿ�ת��Ŀ��
        bool ok = false;

        // ��õ��˵�λ��
        Vector3 targetpos = m_targetEnemy.transform.position;
        targetpos.y = this.transform.position.y;

        // ���Ŀ�귽��
        Vector3 targetDir = targetpos - transform.position;
        // ��Ŀ�귽����ת�������µķ���
        Vector3 rot_delta = Vector3.RotateTowards(this.transform.forward, targetDir, 20.0f * Time.deltaTime, 0.0F);
        // ת���������
        Quaternion targetrotation = Quaternion.LookRotation(rot_delta);

        // ���Ŀ�귽���뵱ǰ����Ĳ�ֵ
        float angle = Vector3.Angle(targetDir, transform.forward);
        // ���
        if (angle < 1.0f)
        {
            // ��ת��Ŀ��
            ok = true;
        }

        transform.rotation = targetrotation;

        return ok;
    }

    // ���ҵ���
    void FindEnemy()
    {
        if (m_targetEnemy != null)
            return;

        int minlife = 0; // Ŀ�������ֵ
        foreach (Enemy enemy in GameManager.Instance.m_EnemyList)
        {
            if (enemy.m_life == 0)
                continue;

            Vector3 pos1 = this.transform.position; pos1.y = 0;
            Vector3 pos2 = enemy.transform.position; pos2.y = 0;
            // ���Ŀ������Լ��ľ���
            float dist = Vector3.Distance(pos1, pos2);
            // ���Ŀ�겻�ڹ�����Χ��
            if (dist > m_attackArea)
                continue;
            // Ѱ������ֵ��͵ĵ���
            if (minlife == 0 || minlife > enemy.m_life)
            {
                // ����Ŀ��
                m_targetEnemy = enemy;
                minlife = enemy.m_life;
            }
        }
    }

    // ����˹���
    public virtual void Attack()
    {
        m_timer -= Time.deltaTime;
        if (m_timer > 0 || m_targetEnemy == null)
            return;

        // ���Ź�������
        m_ani["attack"].time = 0;
        m_ani.CrossFade("attack");
        // ���ü�ʱ��
        m_timer = m_attackTime;

        // ������һ�빥������ʱ�Ե��˲����˺�Ч��
        StartCoroutine(AttackDelay(m_ani["attack"].length * 0.5f));

    }

    IEnumerator AttackDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (m_targetEnemy != null)
            m_targetEnemy.SetDamage(m_power);
        m_targetEnemy = null;
        m_ani.CrossFade("idle");
    }
}
