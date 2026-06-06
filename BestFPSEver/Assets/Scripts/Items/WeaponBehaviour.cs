using UnityEngine;

public class WeaponBehaviour : MonoBehaviour
{
    public WeaponData data;
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

        Vector3 direction = Camera.main.transform.forward;

        direction = Quaternion.Euler(
            Random.Range(-data.spray, data.spray),
            Random.Range(-data.spray, data.spray),
            0
        ) * direction;

        Debug.DrawRay(
            Camera.main.transform.position,
            direction * data.range,
            Color.red,
            2f);

        RaycastHit hit;

        if (Physics.Raycast(
            Camera.main.transform.position,
            direction,
            out hit,
            data.range))
        {
            Debug.Log("Hit: " + hit.collider.name);
        }
    }
    private System.Collections.IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        Debug.Log("Reloading...");

        yield return new WaitForSeconds(data.reloadTime);

        currentAmmo = data.magazine;
        Debug.Log("Reload complete");
        isReloading = false;
    }
}