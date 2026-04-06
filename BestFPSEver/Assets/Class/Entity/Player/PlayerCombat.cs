using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Camera playerCamera;
    public float damage = 25f;
    public float range = 50f;

    public ParticleSystem muzzleFlash;

    public AudioSource audioSource;
    public AudioClip gunShotSFX;

    public GunRecoil gunRecoil;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (muzzleFlash != null)
            muzzleFlash.Play();

        if (audioSource != null && gunShotSFX != null)
            audioSource.PlayOneShot(gunShotSFX);

        if (gunRecoil != null)
            gunRecoil.ShootRecoil();

        Ray ray = playerCamera.ScreenPointToRay(
            new Vector2(Screen.width / 2, Screen.height / 2)
        );

        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            Entity entity = hit.collider.GetComponentInParent<Entity>();
            if (entity != null)
                entity.TakeDamage(damage);
        }
    }
}