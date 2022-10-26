using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public float shakeDuration = 0f;
    private float shakeMagnitude = 40f;
    private float dampingSpeed = 1.0f;
    private RectTransform tr;
    Vector3 initialPosition;

    void OnEnable()
    {
        tr = GetComponent<RectTransform>();
        initialPosition = tr.anchoredPosition;
    }

    void Update()
    {
        if (shakeDuration > 0)
        {
            tr.anchoredPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude;

            shakeDuration -= Time.deltaTime * dampingSpeed;
        }
        else
        {
            shakeDuration = 0f;
            tr.anchoredPosition = initialPosition;
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
