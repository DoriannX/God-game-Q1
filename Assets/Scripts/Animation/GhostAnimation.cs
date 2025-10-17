using UnityEngine;

public class GhostAnimation : MonoBehaviour
{
    [SerializeField] private float floatAmplitude = 0.5f;
    [SerializeField] private float floatFrequency = 1f;
    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = transform.localPosition;
    }
    
    private void Update()
    {
        FloatAnimation();
    }
    
    private void FloatAnimation()
    {
        float newY = initialPosition.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.localPosition = new Vector3(initialPosition.x, newY, initialPosition.z);
    }
}