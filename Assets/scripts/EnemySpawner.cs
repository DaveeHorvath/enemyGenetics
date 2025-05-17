using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class EnemyGroup
{
    SortedList<float, byte[]> performance;
    public int baseAmount;
    Queue<Vector3> extra;
    Queue<float> recentDeaths;
    public float agitatedness;
    public float mutationMod;
    public int attributeSum;
    public bool demoMode = false;
    public float expectedDeathTime;
    public static byte[] ToByteArray(Vector3 s)
    {
        BinaryFormatter bf = new BinaryFormatter();
        using var ms = new MemoryStream();
        bf.Serialize(ms, s);
        return ms.ToArray();
    }

    public static Vector3 fromByteArray(byte[] bytes)
    {
        BinaryFormatter bf = new BinaryFormatter();
        using var ms = new MemoryStream();
        Vector3 s = (Vector3)bf.Deserialize(ms);
        return s;
    }


    statistics createNewStatistic()
    {
        if (extra.Count == 0) {
            byte[] child1 = new byte[120];
            byte[] child2 = new byte[120];
            // select "parents"
            System.Random rnd = new();
            // fix for proper weighted list selection
            var crossoverPoint = rnd.Next(0, 120);
            var parent1index = rnd.Next(10);
            var parent2index = rnd.Next(10);
            if (parent1index == parent2index)
                parent2index = parent1index + 1;
            // crossover - to be changed from single point to normal with centers at both parents at values
            Array.Copy(performance.Values[parent1index], child1, crossoverPoint);
            Array.Copy(performance.Values[parent2index], child2, crossoverPoint);

            Array.Copy(performance.Values[parent2index], crossoverPoint, child1, crossoverPoint, 120 - crossoverPoint);
            Array.Copy(performance.Values[parent1index], crossoverPoint, child2, crossoverPoint, 120 - crossoverPoint);

            // mutation
            Vector3 mutation = UnityEngine.Random.onUnitSphere * UnityEngine.Random.Range(1, 100) / 100.0f * mutationMod;
            Vector3 genom = fromByteArray(child1);
            genom += mutation;
            extra.Enqueue(genom);

            mutation = UnityEngine.Random.onUnitSphere * UnityEngine.Random.Range(1, 100) / 100.0f * mutationMod;
            genom = fromByteArray(child2);
            genom += mutation;
            extra.Enqueue(genom);
        }
        return smartMutate(extra.Dequeue());
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
                Math.Max(0, Math.Min(1, avgDeathTime / (recentDeaths.Count * expectedDeathTime)))) * toMutate.magnitude / 3; // max 25% impact
            toMutate += smartOffset;
        }
        statistics s = new();
        s.Health = toMutate.x;
        s.AttackDamage = toMutate.y;
        s.Speed = toMutate.z;
        // create demo with 3 fitnesstests: just to show that the theory works, we just need a good fitness function for the game itself
        return s;
    }
}

public class EnemySpawner : MonoBehaviour
{
    public int MaxTargets;
    public List<EnemyGroup> Spawns;

    public void RegisterDeath(float survivalTime, int damageDealt, int INDEX)
    {
        // calculateFitness
    }

    private void Start()
    {
        // spawn enemy prefabs to not have to do it whenever one dies
    }

    public void SpawnEnemy()
    {
        int currentTarget = ChooseTarget();

    }

    private int ChooseTarget()
    {
        return UnityEngine.Random.Range(0, MaxTargets);
    }
}
