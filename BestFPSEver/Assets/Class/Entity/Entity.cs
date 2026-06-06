using UnityEngine;

public class Entity : MonoBehaviour
{
    //HP
    public float maxHP = 100f;
    public float currentHP;

     protected virtual void Start()
    {
        currentHP = maxHP;
    }

    public virtual void TakeDamage(float damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            Die();
        }
    }
    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}
