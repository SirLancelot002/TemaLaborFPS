using UnityEngine;

public class Entity : MonoBehaviour
{
    //HP
    public float maxHP = 100f;
    public float currentHP;

    void Start()
    {
        currentHP = maxHP;
    }

    protected virtual void TakeDamage(float damage)
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


    void Update()
    {
        
    }
}
