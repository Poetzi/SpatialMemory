using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawGizmo : MonoBehaviour
{
    private GameObject gizmoContainer;

    [SerializeField] public Vector3 boxCenter = new Vector3(0, 1, 0);
    [SerializeField] public Vector3 boxSize = new Vector3(0.91f, 0.91f, 0.46f);

    public void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 1, 1, 0.5f);
        Gizmos.DrawWireCube(boxCenter, boxSize);

        // Calculate the step size for each grid cell in each dimension
        float stepX = boxSize.x / 3;
        float stepY = boxSize.y / 3;
        float stepZ = boxSize.z / 3;

        // Starting corner of the grid
        Vector3 gridStart = boxCenter - boxSize / 2;

        // Draw lines along the x-axis
        for (int x = 0; x <= 3; x++)
        {
            for (int y = 0; y <= 3; y++)
            {
                Vector3 start = gridStart + new Vector3(x * stepX, y * stepY, 0);
                Vector3 end = start + new Vector3(0, 0, boxSize.z);
                Gizmos.DrawLine(start, end);
            }
        }

        // Draw lines along the y-axis
        for (int y = 0; y <= 3; y++)
        {
            for (int z = 0; z <= 3; z++)
            {
                Vector3 start = gridStart + new Vector3(0, y * stepY, z * stepZ);
                Vector3 end = start + new Vector3(boxSize.x, 0, 0);
                Gizmos.DrawLine(start, end);
            }
        }

        // Draw lines along the z-axis
        for (int z = 0; z <= 3; z++)
        {
            for (int x = 0; x <= 3; x++)
            {
                Vector3 start = gridStart + new Vector3(x * stepX, 0, z * stepZ);
                Vector3 end = start + new Vector3(0, boxSize.y, 0);
                Gizmos.DrawLine(start, end);
            }
        }
    }

    public void CreateGizmoLines()
    {
        gizmoContainer = new GameObject("GizmoContainer");

        // Calculate steps for grid
        float stepX = boxSize.x / 3;
        float stepY = boxSize.y / 3;
        float stepZ = boxSize.z / 3;

        // Draw lines for the box grid
        DrawGrid(stepX, stepY, stepZ);
    }

    private void DrawGrid(float stepX, float stepY, float stepZ)
    {
        Vector3 gridStart = boxCenter - boxSize / 2;
        for (int i = 0; i <= 3; i++)
        {
            for (int j = 0; j <= 3; j++)
            {
                CreateLineRenderer(new Vector3(gridStart.x + i * stepX, gridStart.y + j * stepY, gridStart.z),
                                   new Vector3(gridStart.x + i * stepX, gridStart.y + j * stepY, gridStart.z + boxSize.z));

                CreateLineRenderer(new Vector3(gridStart.x, gridStart.y + i * stepY, gridStart.z + j * stepZ),
                                   new Vector3(gridStart.x + boxSize.x, gridStart.y + i * stepY, gridStart.z + j * stepZ));

                CreateLineRenderer(new Vector3(gridStart.x + i * stepX, gridStart.y, gridStart.z + j * stepZ),
                                   new Vector3(gridStart.x + i * stepX, gridStart.y + boxSize.y, gridStart.z + j * stepZ));
            }
        }
    }

    private void CreateLineRenderer(Vector3 start, Vector3 end)
    {
        GameObject lineObj = new GameObject("Line");
        lineObj.transform.parent = gizmoContainer.transform;

        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPositions(new Vector3[] { start, end });
        lr.startWidth = 0.01f;
        lr.endWidth = 0.01f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = Color.white;
        lr.endColor = Color.white;
    }
}
