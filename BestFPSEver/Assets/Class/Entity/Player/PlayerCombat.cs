using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    //Shooting
    public Camera playerCamera;
    public float damage = 25f;
    public float range = 50f;

    //Effect
    public ParticleSystem muzzleFlash;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))//LMB
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (muzzleFlash != null)
            muzzleFlash.Play();

        Ray ray = playerCamera.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));

        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            Entity entity = hit.collider.GetComponentInParent<Entity>();
            if (entity != null)
                entity.TakeDamage(damage);
        }
    }
}
