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
    Transform weapon;
    Transform body;

    public void Setup(Vector3 startPosition, POI _target, statistics _stat, int index)
    {
        weapon = transform.GetChild(0);
        weapon.localScale = new Vector3(1,1,1) * (.75f + _stat.AttackDamage * 4);
        body = transform.GetChild(1);
        body.localScale = new Vector3(1, 1, 1) * (.75f + _stat.Health * 4);
        damageDealt = 0;
        target = _target;
        INDEX = index;
        startTime = Time.time;
        stat = _stat;
        transform.position = startPosition;
        Debug.Log(_stat.Health + _stat.Speed + _stat.AttackDamage);
        // "point allocation"
        maxHealt = Mathf.RoundToInt(100f * stat.Health); // 100 is the max possible health
        speed = 6f * stat.Speed; // 6 is max speed
        damage = Mathf.RoundToInt(10f * stat.AttackDamage); // 10 is the max possible damage
        if (_stat.AttackDamage * _stat.AttackDamage + _stat.Health * _stat.Health + _stat.Speed * _stat.Speed > 1.2)
            Debug.LogError("Scaling" + (_stat.AttackDamage * _stat.AttackDamage + _stat.Health * _stat.Health + _stat.Speed * _stat.Speed - 1).ToString());
    }

    public void TakeDamage(int damage)
    {
        Debug.Log(damage);
        //currentHealth -= damage;
        //if (currentHealth < 0)
        //{
        //    parent.RegisterDeath(Time.time - startTime, damageDealt, INDEX, 0, stat);
        //    // play death animation
        //    gameObject.SetActive(false);
        //}
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dir = target.transform.position - transform.position;
        if (dir.magnitude > 1)
            transform.position += speed * Time.deltaTime * dir.normalized;
        else
        {
            gameObject.SetActive(false);
            parent.RegisterDeath(Time.time - startTime, damageDealt, INDEX, 0, stat);
        }
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
