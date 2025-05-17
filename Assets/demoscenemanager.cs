using UnityEngine;

public class demoscenemanager : MonoBehaviour
{
    // Update is called once per frame
    public EnemySpawner spawner;
    private float lastSpawn = 0;
    private float delay = 0;
    void Update()
    {
        if (lastSpawn + delay < Time.time)
        {
            spawner.SpawnEnemy();
            lastSpawn = Time.time;
            delay = Random.Range(1f, 1.5f);
        }
    }
}
