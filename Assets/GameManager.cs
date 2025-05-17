using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int kills = 20;
    public int enviromentDamageTaken = 0;
    public EnemySpawner spawner;
    private float lastSpawn = 10;
    private float delay = 0;

    [SerializeField] GameObject arrivalPos;
    [SerializeField] GameObject player;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        Invoke("spawnPlayer", .4f);
    }
    
    void Update()
    {
        if (lastSpawn + delay < Time.time)
        {
            spawner.SpawnEnemy();
            lastSpawn = Time.time;
            delay = Random.Range(2.5f, 5f);
        }
    }



    void spawnPlayer()
    {
        player.SetActive(true);
    }
}
