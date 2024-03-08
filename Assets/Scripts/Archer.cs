using UnityEngine;
using System.Collections;

// 远程防守单位
public class Archer : Defender {

    // 模型边框
    Bounds bounds;

    // 初始化数值
    protected override void Init()
    {
        m_attackArea = 5.0f;
        m_power = 1;
        CreateModel("archer");

        // 获得模型的边框
        bounds = m_model.GetComponentInChildren<SkinnedMeshRenderer>().bounds;
    }

    // 向敌人攻击
    public override void Attack()
    {
        // 当播放完攻击动画
        if (!m_ani.isPlaying)
        {
            if (m_targetEnemy != null)
            {
                // 将弓箭的初始位置设置到自身中心并向前0.5个单位
                Vector3 pos = this.transform.TransformPoint(0,bounds.center.y,0.5f);
               
                // 创建弓箭
                Arrow.Create(m_targetEnemy.transform, pos, (Enemy enemy) =>
                {
                    enemy.SetDamage(m_power);
                    m_targetEnemy = null;
                });
            }
            
            m_ani.CrossFade("idle");
        }

        m_timer -= Time.deltaTime;
        if (m_timer > 0 || m_targetEnemy == null)
            return;

        m_ani.CrossFade("attack");
        m_timer = m_attackTime;

    }
}
