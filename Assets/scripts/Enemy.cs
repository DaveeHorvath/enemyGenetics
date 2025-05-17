using NUnit.Framework;
using System.Collections.Generic;
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
    public List<Sprite> enemies;
    public List<Sprite> weapons;

    public void Setup(Vector3 startPosition, POI _target, statistics _stat, int index)
    {
        System.Random r = new();
        weapon = transform.GetChild(0);
        weapon.GetComponent<SpriteRenderer>().sprite = weapons[r.Next(weapons.Count)];
        weapon.localScale = new Vector3(1,1,1) * (.75f + _stat.AttackDamage * 4);
        body = transform.GetChild(1);
        body.GetComponent<SpriteRenderer>().sprite = enemies[r.Next(enemies.Count)];
        body.localScale = new Vector3(1, 1, 1) * (.75f + _stat.Health * 4);
        damageDealt = 0;
        target = _target;
        INDEX = index;
        startTime = Time.time;
        stat = _stat;
        transform.position = startPosition;
        // "point allocation"
        maxHealt = Mathf.RoundToInt(100f * stat.Health); // 100 is the max possible health
        currentHealth = maxHealt;
        speed = 0.25f + 4f * stat.Speed; // 6 is max speed
        damage = 5 + Mathf.RoundToInt(7f * stat.AttackDamage); // 10 is the max possible damage
        if (_stat.AttackDamage * _stat.AttackDamage + _stat.Health * _stat.Health + _stat.Speed * _stat.Speed > 1.2)
            Debug.LogError("Scaling" + (_stat.AttackDamage * _stat.AttackDamage + _stat.Health * _stat.Health + _stat.Speed * _stat.Speed - 1).ToString());
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0)
        {
            parent.RegisterDeath(Time.time - startTime, damageDealt, INDEX, 0, stat);
            // play death animation
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dir = target.transform.position - transform.position;
        if (dir.magnitude > 1)
            transform.position += speed * Time.deltaTime * dir.normalized;
        else
        {
            Attack();
        }
    }
    private void Attack()
    {
        if (lastAttack + 3 < Time.time)
        {
            damageDealt += damage;
            lastAttack = Time.time;
            target.TakeDamage(damage);
        }
    }
}
