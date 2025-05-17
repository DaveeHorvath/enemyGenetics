using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private float moveSpeed;
    public Vector3 playerMoveDirection;
    public int damage_val;
    public float damage_rad_offset_x;
    public float damage_rad_offset_y;
    [SerializeField] private float inputX;
    [SerializeField] private float inputY;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");
        playerMoveDirection = new Vector3(inputX, inputY).normalized;

        animator.SetFloat("moveX", inputX);
        animator.SetFloat("moveY", inputY);

        if (playerMoveDirection ==  Vector3.zero )
        {
            animator.SetBool("moving", false);
        }
        else
        {
            animator.SetBool("moving", true);
            
        }

        // down -> (0, 0) = (0, -1)
        // up -> (0, 0.5) = (0, 1)
        // left -> (-0.25, 0.25) = (-1, 0)
        // left -> (0.25, 0.25) = (1, 0)

        if (Input.GetKeyDown(KeyCode.Space))
        {
            var a = Physics2D.OverlapCircleAll(transform.position + new Vector3(inputX * damage_rad_offset_x, inputY * damage_rad_offset_y + damage_rad_offset_y + 0.5f), 0.5f);

            for (int i = 0; i < a.Length; i++)
            {
                a[i].GetComponent<Enemy>().TakeDamage(damage_val);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + new Vector3(inputX * damage_rad_offset_y, inputY * damage_rad_offset_x + damage_rad_offset_y + 0.5f), 0.5f);
    }
    private void FixedUpdate()
    {
            rb.linearVelocity = new Vector3(playerMoveDirection.x, playerMoveDirection.y).normalized * moveSpeed;
    }
}
