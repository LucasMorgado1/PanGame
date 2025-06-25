using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
    public Transform target;                 // Jogador a ser seguido
    public float smoothTime = 0.3f;          // Tempo de suavização
    public Vector3 offset;                   // Posição relativa ao jogador
    private Vector3 velocity = Vector3.zero;

    private void LateUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
        transform.position = smoothedPosition;
    }
}