using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyVision))]
public class FOVEditor : Editor
{
    private EnemyVision fov;
    private void OnEnable()
    {
        fov = (EnemyVision)target;
    }
    private void OnSceneGUI()
    {
        DrawGizmos();

        if (fov.inSight)
            PlayerInSight();
    }

    private void DrawGizmos()
    {
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.range);

        Vector3 viewAngle01 = DirFromAngle(fov.transform.eulerAngles.y, -fov.fov / 2);
        Vector3 viewAngle02 = DirFromAngle(fov.transform.eulerAngles.y, fov.fov / 2);

        Handles.color = Color.yellow;
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngle01 * fov.range);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngle02 * fov.range);
    }

    private void PlayerInSight()
    {
        Handles.color = Color.red;
        Handles.DrawLine(fov.transform.position, PlayerController.instance.transform.position);
    }

    private Vector3 DirFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
