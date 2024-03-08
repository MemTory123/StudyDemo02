using UnityEngine;
using System.Collections;
using OfficeOpenXml;
using System.IO;

public class EnemySpawner : MonoBehaviour {

    public PathNode m_startNode;

    ArrayList m_enemylist;

    float m_timer = 0;

    int m_index = 0;

    int m_liveEnemy = 0;

	// Use this for initialization
	void Start () {

        Read();

        SpawnData data = (SpawnData)m_enemylist[m_index];
        m_timer = data.wait;
	
	}


    void Read()
    {
        m_enemylist = new ArrayList();
        string path = "Assets/EXCEL/enemy.xlsx";
        FileInfo fileInfo = new FileInfo(path);
        using (ExcelPackage package = new ExcelPackage(fileInfo))
        {
            ExcelWorksheet sheet = null;
            //excel的表数据
            for (int i = 1; i <= package.Workbook.Worksheets.Count; i++)
            {
                sheet = package.Workbook.Worksheets[i];
                //行
                int rowCount = sheet.Dimension.End.Row;
                //列
                //int columnCount = sheet.Dimension.End.Column;

                //行数据：从第2行开始读取，因为第1行，我们定义了字段
                for (var row = 2; row <= rowCount; row++)
                {
                    SpawnData data = new SpawnData();
                    //列数据：从第1行开始读取
                    for (var col = 1; col <= 4; col++)
                    {
                        //如果数据不为空，则打印
                        if (sheet.Cells[row, col].Value != null)
                        {
                            //Debug.Log(sheet.Cells[row, col].Value.ToString());
                            switch (col)
                            {
                                case 1:
                                    data.wave = int.Parse(sheet.Cells[row, col].Value.ToString());
                                    break;
                                case 2:
                                    data.enemyname = sheet.Cells[row, col].Value.ToString();
                                    break;
                                case 3:
                                    data.level = int.Parse(sheet.Cells[row, col].Value.ToString());
                                    break;
                                case 4:
                                    data.wait = float.Parse(sheet.Cells[row, col].Value.ToString());
                                    break;
                                default: break;
                            }
                        }
                    }
                    m_enemylist.Add(data);
                }

            }
        }
    }


        // Update is called once per frame
        void Update()
        {
            SpawnEnemy();
        }


        void SpawnEnemy()
        {

            if (m_index >= m_enemylist.Count)
            {
                return;
            }

            m_timer -= Time.deltaTime;
            if (m_timer > 0)
                return;


            SpawnData data = (SpawnData)m_enemylist[m_index];

            if (GameManager.Instance.m_wave < data.wave)
            {
                if (m_liveEnemy > 0)
                    return;
                else
                    GameManager.Instance.SetWave(data.wave);
            }


            m_index++;
            if (m_index < m_enemylist.Count)
                m_timer = ((SpawnData)m_enemylist[m_index]).wait;



            GameObject enemymodel = Resources.Load<GameObject>(data.enemyname + "@skin");

            GameObject enemyani = Resources.Load<GameObject>(data.enemyname + "@run");

            Vector3 dir = m_startNode.transform.position - this.transform.position;
            GameObject enmeyObj = (GameObject)Instantiate(enemymodel, this.transform.position,
                Quaternion.LookRotation(dir));
            enmeyObj.GetComponent<Animation>().AddClip(enemyani.GetComponent<Animation>().GetClip("run"), "run");


            enmeyObj.GetComponent<Animation>()["run"].wrapMode = WrapMode.Loop;

            enmeyObj.GetComponent<Animation>().CrossFade("run");

            Enemy enemy = enmeyObj.AddComponent<Enemy>();


            enemy.m_currentNode = m_startNode;


            enemy.m_life = data.level * 3;
            enemy.m_maxlife = data.level * 3;


            m_liveEnemy++;

            OnEnemyDeath(enemy, (Enemy e) =>
            {
                m_liveEnemy--;
            });
        }

        void OnEnemyDeath(Enemy enemy, System.Action<Enemy> onDeath)
        {
            enemy.onDeath = onDeath;
        }

        void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, "spawner.tif");
        }


    public class SpawnData
    {
        public int wave = 1;
        public string enemyname = "";
        public int level = 1;
        public float wait = 1.0f;
    }
}
