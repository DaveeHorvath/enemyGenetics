using System.Collections;
using UnityEngine;

public class tmpPlayerController : MonoBehaviour
{
    // Update is called once per frame
    public LayerMask wall;
    public LayerMask enemy;
    public int damage = 10;
    public float attackrange;
    Vector2 dir;
    float lastAttack = 0;
    float attackspeed = 1;
    GameObject sillyDamager;
    void Update()
    {
        sillyDamager = transform.GetChild(0).gameObject;
        dir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (Physics2D.Raycast(transform.position, new Vector2(1, 0), .27f, wall) && dir.x > 0)
            dir.x = 0;
        if (Physics2D.Raycast(transform.position, new Vector2(-1, 0), .27f, wall) && dir.x < 0)
            dir.x = 0;
        if (Physics2D.Raycast(transform.position, new Vector2(0, 1), .55f, wall) && dir.y > 0)
            dir.y = 0;
        if (Physics2D.Raycast(transform.position, new Vector2(0, -1), .55f, wall) && dir.y < 0)
            dir.y = 0;
        transform.position += new Vector3(dir.x, dir.y).normalized * 10 * Time.deltaTime;

        if (Input.GetKey(KeyCode.Space))
            Attack();
    }
    void Attack()
    {
        if (lastAttack + attackspeed < Time.time)
        {
            var enemies = Physics2D.CircleCastAll(transform.position, attackrange, Vector2.zero, 0, enemy);
            // spawn animation
            StartCoroutine("evil");
            foreach (var enemy in enemies)
            {
                enemy.transform.GetComponent<Enemy>().TakeDamage(damage);
            }
            lastAttack = Time.time;
        }

    }

    IEnumerator evil()
    {
        sillyDamager.SetActive(true);
        yield return new WaitForSeconds(.3f);
        sillyDamager.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, new Vector2(0, 1) * .27f);
        Gizmos.DrawRay(transform.position, new Vector2(1, 0) * .55f);
        Gizmos.DrawRay(transform.position, new Vector2(-1, 0) * .55f);
        Gizmos.DrawRay(transform.position, new Vector2(0, -1) * .27f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackrange);
    }
}
