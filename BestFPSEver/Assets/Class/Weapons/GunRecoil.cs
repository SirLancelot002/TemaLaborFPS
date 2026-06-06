using UnityEngine;

public class GunRecoil : MonoBehaviour
{
    [Header("Recoil")]
    public Vector3 recoilPosition = new Vector3(0f, 0f, -0.1f);
    public Vector3 recoilRotation = new Vector3(-10f, 0f, 0f);

    [Header("Speed")]
    public float snappiness = 10f;
    public float returnSpeed = 20f;

    private Vector3 targetPosition;
    private Vector3 currentPosition;

    private Vector3 targetRotation;
    private Vector3 currentRotation;

    private void Update()
    {
        //Return to normal
        targetPosition = Vector3.Lerp(targetPosition, Vector3.zero, returnSpeed * Time.deltaTime);
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);

        // Move toward target recoil
        currentPosition = Vector3.Lerp(currentPosition, targetPosition, snappiness * Time.deltaTime);
        currentRotation = Vector3.Lerp(currentRotation, targetRotation, snappiness * Time.deltaTime);

        transform.localPosition = currentPosition;
        transform.localRotation = Quaternion.Euler(currentRotation);
    }

    public void ShootRecoil()
    {
        targetPosition += recoilPosition;
        targetRotation += recoilRotation;
    }
}