using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public float shakeDuration = 0f;
    private float shakeMagnitude = 0.4f;
    private float dampingSpeed = 1.0f;
    Vector3 initialPosition;

    void OnEnable()
    {
        initialPosition = transform.localPosition;
    }

    void Update()
    {
        if (shakeDuration > 0)
        {
            transform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude;

            shakeDuration -= Time.deltaTime * dampingSpeed;
        }
        else
        {
            shakeDuration = 0f;
            transform.localPosition = initialPosition;
        }
    }

    /// <summary>
    /// Activa el screen shake.
    /// </summary>
    /// <param name="shakeDuration">Duración del efecto.</param>
    public void TriggerShake(float shakeDuration)
    {
        this.shakeDuration = shakeDuration;
    }
}
