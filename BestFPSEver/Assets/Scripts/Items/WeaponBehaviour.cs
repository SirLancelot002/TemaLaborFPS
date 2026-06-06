using System.Collections;
using UnityEngine;

public class WeaponBehaviour : MonoBehaviour
{
    [Header("Weapon Data")]
    public WeaponData data;

    [Header("References")]
    public Transform firePoint;
    public AudioSource audioSource;

    [Header("Effects")]
    public AudioClip shootSfx;
    public LineRenderer tracerPrefab;
    public ParticleSystem muzzleFlash;

    [Header("Tracer Settings")]
    public float tracerDuration = 0.05f;

    private float nextShootTime = 0f;
    private bool isReloading = false;
    private int currentAmmo;

    void Start()
    {
        currentAmmo = data.magazine;
    }

    public void Reload()
    {
        if (isReloading)
            return;

        if (currentAmmo == data.magazine)
            return;

        StartCoroutine(ReloadCoroutine());
    }

    public void Shoot()
    {
        if (isReloading)
            return;

        if (Time.time < nextShootTime)
            return;

        if (currentAmmo <= 0)
        {
            Debug.Log("Reload needed!");
            return;
        }

        nextShootTime = Time.time + (1f / data.fireRate);
        currentAmmo--;

        Debug.Log("Ammo: " + currentAmmo);

        PlayShootEffects();

        Vector3 direction = Camera.main.transform.forward;

        direction = Quaternion.Euler(
            Random.Range(-data.spray, data.spray),
            Random.Range(-data.spray, data.spray),
            0
        ) * direction;

        Vector3 startPoint = firePoint != null ? firePoint.position : Camera.main.transform.position;
        Vector3 endPoint;

        RaycastHit hit;

        if (Physics.Raycast(
            Camera.main.transform.position,
            direction,
            out hit,
            data.range))
        {
            Debug.Log("Hit: " + hit.collider.name);

            endPoint = hit.point;

            Entity entity = hit.collider.GetComponentInParent<Entity>();
            if (entity != null)
            {
                entity.TakeDamage(data.damage);
            }
        }
        else
        {
            endPoint = Camera.main.transform.position + direction * data.range;
        }

        if (tracerPrefab != null)
        {
            StartCoroutine(SpawnTracer(startPoint, endPoint));
        }

        Debug.DrawRay(
            Camera.main.transform.position,
            direction * data.range,
            Color.red,
            2f);
    }

    private void PlayShootEffects()
    {
        if (audioSource != null && shootSfx != null)
        {
            audioSource.PlayOneShot(shootSfx);
        }

        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }
    }

    private IEnumerator SpawnTracer(Vector3 startPoint, Vector3 endPoint)
    {
        LineRenderer tracer = Instantiate(tracerPrefab);

        tracer.positionCount = 2;
        tracer.SetPosition(0, startPoint);
        tracer.SetPosition(1, endPoint);

        yield return new WaitForSeconds(tracerDuration);

        Destroy(tracer.gameObject);
    }

    private IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        Debug.Log("Reloading...");

        yield return new WaitForSeconds(data.reloadTime);

        currentAmmo = data.magazine;
        Debug.Log("Reload complete");
        isReloading = false;
    }
}