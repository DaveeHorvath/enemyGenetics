using UnityEngine;

public class POI : MonoBehaviour
{
    public int damage;
    public float attackspeed;
    public int damageTaken;
    public Vector2 angle;
    public LayerMask mask;
    float lastAttack;

    private void Update()
    {
        var found = Physics2D.Raycast(transform.position, angle.normalized, 5, mask);
        if (found && lastAttack + attackspeed < Time.time)
        {
            found.transform.GetComponent<Enemy>().TakeDamage(damage);
            lastAttack = Time.time;
        }
        Debug.Log(Time.time);
    }
    private void OnGUI()
    {
        Debug.DrawRay(transform.position, angle.normalized * 5, Color.red);
    }
}
