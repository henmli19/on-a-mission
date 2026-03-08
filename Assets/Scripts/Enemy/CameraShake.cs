using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;
    private Vector3 originalPos;

    void Awake() => Instance = this;

    public void Shake(float intensity, float duration)
    {
        originalPos = transform.localPosition;
        InvokeRepeating("DoShake", 0, 0.01f);
        Invoke("StopShake", duration);
    }

    void DoShake()
    {
        float shakeX = Random.Range(-1f, 1f) * 0.1f;
        float shakeY = Random.Range(-1f, 1f) * 0.1f;
        transform.localPosition += new Vector3(shakeX, shakeY, 0);
    }

    void StopShake()
    {
        CancelInvoke("DoShake");
        transform.localPosition = originalPos;
    }
}