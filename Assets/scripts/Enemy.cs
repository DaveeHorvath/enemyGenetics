using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemySpawner parent;
    public int INDEX;
    public POI target;
    public statistics stat;
    public int maxHealt;
    public int currentHealth;
    public float speed = 10;
    public int damage;
    public float startTime;
    int damageDealt = 0;
    float lastAttack;

    public void Setup(Vector3 startPosition, POI _target, statistics _stat, int index)
    {
        damageDealt = 0;
        target = _target;
        INDEX = index;
        startTime = Time.time;
        stat = _stat;
        transform.position = startPosition;
        // "point allocation"
        maxHealt = Mathf.RoundToInt(100f * stat.Health); // 100 is the max possible health
        speed = 10f * stat.Speed; // 10 is max speed
        damage = Mathf.RoundToInt(10f * stat.AttackDamage); // 10 is the max possible damage
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0)
        {
            parent.RegisterDeath(Time.time - startTime, damageDealt, INDEX, 0, stat);
            // play death animation
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dir = target.transform.position - transform.position;
        if (dir.magnitude > 1)
            transform.position += speed * Time.deltaTime * dir.normalized;
        else
            Attack();
    }
    private void Attack()
    {
        if (lastAttack + 1 < Time.time)
        {
            damageDealt += damage;
            lastAttack = Time.time;
            target.damageTaken += damage;
        }
    }
}
