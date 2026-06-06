using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MeleeEnemy : Entity
{
    [Header("Enemy Settings")]
    public float attackRange = 2f;
    public float attackDamage = 10f;
    public float attackCooldown = 1.5f;

    private Transform target;
    private bool canAttack = true;
    private NavMeshAgent agent;

    protected override void Start()
    {
        base.Start();

        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = attackRange;

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            target = playerObj.transform;
    }

    private void Update()
    {
        if (target == null) return;

        // Move toward player using NavMesh
        agent.SetDestination(target.position);

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= attackRange && canAttack)
        {
            Attack();
        }
    }

    private void Attack()
    {
        canAttack = false;

        // Stop moving while attacking
        agent.isStopped = true;

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
            canAttack = false;
            agent.isStopped = true;

            playerEntity.TakeDamage(attackDamage);

            Invoke(nameof(ResetAttack), attackCooldown);
        }
    }

    private void ResetAttack()
    {
        canAttack = true;
        agent.isStopped = false;
    }
}