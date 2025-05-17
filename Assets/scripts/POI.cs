using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class POI : MonoBehaviour
{
    public int damage;
    public float attackspeed = .5f;
    public int damageTaken;
    public Vector2 angle;
    public LayerMask mask;
    float lastAttack = -100;
    public Text number;
    public GameObject silly;
    Transform? isDamaging;

    private void Update()
    {
        if (isDamaging != null)
            silly.transform.position += (isDamaging.position - silly.transform.position).normalized * 2.5f * Time.deltaTime;
        var found = Physics2D.Raycast(transform.position, angle.normalized, 5, mask);
        if (found && lastAttack + attackspeed < Time.time)
        {
            isDamaging = found.transform;
            StartCoroutine("evil");
            silly.transform.position = transform.position;
            silly.SetActive(true);
            silly.transform.position = found.transform.position;
            
            found.transform.GetComponent<Enemy>().TakeDamage(damage);
            lastAttack = Time.time;
        }
    }

    IEnumerator evil()
    {
        yield return new WaitForSeconds(.25f);
        isDamaging = null;
        silly.SetActive(false);
    }

    public void TakeDamage(int damage)
    {
        damageTaken += damage;
        number.text = damageTaken.ToString();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, angle.normalized * 5);
    }
}
