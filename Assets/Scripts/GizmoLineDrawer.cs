using UnityEngine;

public class GizmoLineDrawer : MonoBehaviour
{
    // Array de pontos
    public Transform[] _points;

    private void OnDrawGizmos()
    {
        // Verifica se temos pelo menos dois pontos
        if (_points == null || _points.Length < 2)
        {
            return;
        }

        // Define a cor da linha Gizmo
        Gizmos.color = Color.red;

        // Desenha a linha entre os pontos
        for (int i = 0; i < _points.Length - 1; i++)
        {
            Gizmos.DrawLine(_points[i].position, _points[i + 1].position);
        }
    }
}
