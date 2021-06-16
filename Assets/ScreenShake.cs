using UnityEngine;

public class ScreenShake : MonoBehaviour
{

    private new Transform transform;
    public float shakeDuration = 0f;
    private float shakeMagnitude = 0.4f;
    private float dampingSpeed = 1.0f;
    Vector3 initialPosition;

    void Awake()
    {
        if (transform == null)
        {
            transform = GetComponent(typeof(Transform)) as Transform;
        }
    }

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

    public void TriggerShake(float shakeDuration)
    {
        this.shakeDuration = shakeDuration;
    }
}
