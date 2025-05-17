using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class DuplicateKeyComparer<TKey>
                :
             IComparer<TKey> where TKey : IComparable
{
    #region IComparer<TKey> Members

    public int Compare(TKey x, TKey y)
    {
        int result = x.CompareTo(y);

        if (result == 0)
            return 1; // Handle equality as being greater. Note: this will break Remove(key) or
        else          // IndexOfKey(key) since the comparer never returns 0 to signal key equality
            return result;
    }

    #endregion
}

[System.Serializable]
public class EnemyGroup
{
    public string Name;
    public POI target;
    public GameObject position;
    public SortedList<float, byte[]> performance = new SortedList<float, byte[]>(new DuplicateKeyComparer<float>());
    public int baseAmount;
    Queue<Vector3> extra = new Queue<Vector3>();
    public Queue<float> recentDeaths = new Queue<float>();
    public float agitatedness;
    public float mutationMod;
    public int attributeSum;
    public bool demoMode = false;
    public float expectedDeathTime;
    [Header("Fitness Parameters")]
    [SerializeField] private float timePower;
    [SerializeField] private float timeScale;
    [SerializeField] private float damagePower, damageScale;
    [SerializeField] private float survivalPower, survivalScale;
    [SerializeField] private float playerDistancePower, playerDistanceScale;
    [SerializeField] private float healthAttributeScale, speedAttributeScale, damageAttributeScale;
    [SerializeField] private float expectedSpeed;
    public static byte[] ToByteArray(object s)
    {
        BinaryFormatter bf = new BinaryFormatter();
        using var ms = new MemoryStream();
        bf.Serialize(ms, s);
        return ms.ToArray();
    }

    public static Vector3 fromByteArray(byte[] bytes)
    {
        BinaryFormatter bf = new BinaryFormatter();
        using var ms = new MemoryStream(bytes);
        var stat = (statistics)bf.Deserialize(ms);
        Vector3 s = new Vector3(stat.Health, stat.AttackDamage, stat.Speed);
        return s;
    }

    public void SetupBase()
    {
        for (int i = 0; i < baseAmount; i++)
        {
            extra.Enqueue(posNormal(UnityEngine.Random.onUnitSphere));
        }
    }


    public statistics createNewStatistic()
    {
        if (extra.Count == 0) {
            if (performance.Count < baseAmount)
                extra.Enqueue(posNormal(UnityEngine.Random.onUnitSphere));
            else
            {
                byte[] child1 = new byte[162];
                byte[] child2 = new byte[162];
                // select "parents"
                System.Random rnd = new();
                // fix for proper weighted list selection
                var crossoverPoint = rnd.Next(0, 162);
                var parent1index = rnd.Next(1, Math.Min(10, performance.Count));
                var parent2index = rnd.Next(1, Math.Min(10, performance.Count));
                if (parent1index == parent2index)
                    parent2index = parent1index + 1;
                Debug.Log(parent1index + ", " + parent2index);
                // crossover - to be changed from single point to normal with centers at both parents at values
                Array.Copy(performance.Values[^parent1index], child1, crossoverPoint);
                Array.Copy(performance.Values[^parent2index], child2, crossoverPoint);

                Array.Copy(performance.Values[^parent2index], crossoverPoint, child1, crossoverPoint, 162 - crossoverPoint);
                Array.Copy(performance.Values[^parent1index], crossoverPoint, child2, crossoverPoint, 162 - crossoverPoint);

                // mutation
                Vector3 mutation = UnityEngine.Random.onUnitSphere * UnityEngine.Random.Range(1, 100) / 100.0f * mutationMod;
                Vector3 genom = fromByteArray(child1);
                genom += mutation;
                extra.Enqueue(genom);

                mutation = UnityEngine.Random.onUnitSphere * UnityEngine.Random.Range(1, 100) / 100.0f * mutationMod;
                genom = fromByteArray(child2);
                genom += mutation;
                extra.Enqueue(posNormal(genom.normalized));
            }
        }
        return smartMutate(extra.Dequeue());
    }

    Vector3 posNormal(Vector3 v)
    {
        return (v + new Vector3(1,1,1)) / 2 ;
    }

    statistics smartMutate(Vector3 toMutate)
    {
        // check how far off recent deaths in this system are from the expected:
        // add interpolated value based on death diff time for fullhealthmode and fulldamagemode
        if (!demoMode)
        {
            float avgDeathTime = 0;
            foreach (var item in recentDeaths)
            {
                avgDeathTime += item;
            }
            Vector3 smartOffset = Vector3.Lerp(new Vector3(1, 0, 0), new Vector3(0, 1, 0), 
                Math.Max(0, Math.Min(1, avgDeathTime / recentDeaths.Count ))) * toMutate.magnitude / 9; // max 10% impact
            toMutate += smartOffset;
        }
        toMutate = toMutate.normalized;
        statistics s = new();
        s.Health = toMutate.x;
        s.AttackDamage = toMutate.y;
        s.Speed = toMutate.z;
        // create demo with 3 fitnesstests: just to show that the theory works, we just need a good fitness function for the game itself
        return s;
    }
    public void calculateFitness(float survivalTime, int damageDealt, int distanceFromPlayer, statistics stat)
    {
        // calculateFitness
        var fitness = Mathf.Pow(Time.time, timePower) * timeScale // recency
            + Mathf.Pow(damageDealt, damagePower) * damageScale // damageDone
            + Mathf.Pow(survivalTime, survivalPower) * survivalScale // how long alive
            + Mathf.Pow(distanceFromPlayer, playerDistancePower) * playerDistanceScale // playerInteraction
                                                                                       // attribute fitness demo
            + stat.Health * healthAttributeScale
            + stat.Speed * speedAttributeScale
            + stat.AttackDamage * damageAttributeScale;
        Debug.Log("fitness: " + fitness);
        // save into the sorted array
        Debug.Log(EnemyGroup.ToByteArray(stat).Length + EnemyGroup.ToByteArray(stat).ToString());
        performance.Add(fitness, EnemyGroup.ToByteArray(stat));
        recentDeaths.Enqueue(survivalTime / (target.angle.magnitude / stat.Speed));
        if (recentDeaths.Count > 10)
            recentDeaths.Dequeue();
    }
}

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemy;
    [SerializeField] private int count;
    public List<EnemyGroup> Spawns;
    [Header("Fitness Parameters")]
    [SerializeField] private float timePower;
    [SerializeField] private float timeScale;
    [SerializeField] private float damagePower, damageScale;
    [SerializeField] private float survivalPower, survivalScale;
    [SerializeField] private float playerDistancePower, playerDistanceScale;
    [SerializeField] private float healthAttributeScale, speedAttributeScale, damageAttributeScale;
    [SerializeField] private float expectedSpeed;
    Queue<(GameObject, Enemy)> enemies = new Queue<(GameObject, Enemy)>();

    public void RegisterDeath(float survivalTime, int damageDealt, int INDEX, int distanceFromPlayer, statistics stat)
    {
        Spawns[INDEX].calculateFitness(survivalTime, damageDealt, distanceFromPlayer, stat);
    }

    private void Start()
    {
        foreach (var s in Spawns) 
        {
            s.target.angle = s.position.transform.position - s.target.transform.position;
            s.expectedDeathTime = s.target.angle.magnitude / expectedSpeed;
            s.SetupBase();
        }
        // spawn enemy prefabs to not have to do it whenever one dies
        for (int i = 0; i < count; i++)
        {
            var g = Instantiate(enemy, transform);
            var e = g.GetComponent<Enemy>();
            e.parent = this;
            enemies.Enqueue((g, e));
            g.SetActive(false);
        }
    }
    public void printStats()
    {
        foreach (var s in Spawns)
        {
            var sum = 0f;
            foreach(var p in s.performance)
            {
                sum += p.Key;
                Debug.Log(p.Key);
            }
            Debug.LogWarning(s.Name + " " + sum / s.performance.Count);
            using (StreamWriter sw = new StreamWriter(Application.dataPath + $"/log_{s.Name}.txt", true))
            {
                sw.WriteLine(sum / s.performance.Count);
            }
        }
    }

    public void SpawnEnemy()
    {
        int currentTarget = ChooseTarget();
        var (current, e) = enemies.Dequeue();
        current.SetActive(true);
        var randomOffset = UnityEngine.Random.insideUnitCircle * 2;
        e.Setup(Spawns[currentTarget].position.transform.position + new Vector3(randomOffset.x, randomOffset.y), Spawns[currentTarget].target, Spawns[currentTarget].createNewStatistic(), currentTarget);
        enemies.Enqueue((current, e));
    }

    private int ChooseTarget()
    {
        return UnityEngine.Random.Range(0, Spawns.Count);
    }
}
