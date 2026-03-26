using UnityEngine;

public class MeleeAttackZone : MonoBehaviour
{
    private MeleeEnemy enemy;

    private void Start()
    {
        enemy = GetComponentInParent<MeleeEnemy>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.root.CompareTag("Player"))
            enemy.TryAttack(other.transform.root.GetComponent<Entity>());
    }
}