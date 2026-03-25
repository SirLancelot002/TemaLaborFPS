using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
public class MeleeEnemy : Entity
{
    [Header("Enemy Settings")]
    public float moveSpeed = 3f;
    public float attackRange = 2f;
    public float attackDamage = 10f;
    public float attackCooldown = 1.5f;

    private Transform target;
    private bool canAttack = true;
    private Rigidbody rb;

    private void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            target = playerObj.transform;
    }

    private void FixedUpdate()
    {
        if (target == null) return;

        // Move towards player
        Vector3 direction = (target.position - transform.position).normalized;
        Vector3 move = direction * moveSpeed * Time.fixedDeltaTime;

        rb.MovePosition(rb.position + move);

        // Rotate towards player smoothly
        Vector3 lookDir = new Vector3(direction.x, 0, direction.z);
        if (lookDir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRot, 10f * Time.fixedDeltaTime);
        }
    }

    private void Attack()
    {
        canAttack = false;

        Entity playerEntity = target.GetComponent<Entity>();
        if (playerEntity != null)
            playerEntity.TakeDamage(attackDamage);

        Invoke(nameof(ResetAttack), attackCooldown);
    }

    public void TryAttack(Entity playerEntity)
    {
        if (!canAttack) return;

        if (playerEntity != null)
        {
            playerEntity.TakeDamage(attackDamage);
            canAttack = false;
            Invoke(nameof(ResetAttack), attackCooldown);
        }
    }

    private void ResetAttack()
    {
        canAttack = true;
    }
}