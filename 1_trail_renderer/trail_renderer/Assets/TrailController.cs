using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TrailController : MonoBehaviour
{
    private const int FRAME_MAX = 10;
    private List<Vector3> points0 = new List<Vector3>();//根本
    private List<Vector3> points1 = new List<Vector3>();//先端

    private Mesh mesh;
    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
    }

    void Update()
    {
        if (FRAME_MAX <= points0.Count)
        {
            points0.RemoveAt(0);
            points1.RemoveAt(0);
        }

        points0.Add(transform.position);
        points1.Add(transform.TransformPoint(new Vector3(0.0f, 1.0f, 0.0f)));

        if (2 <= points0.Count)
        {
            CreateMesh();
        }
    }

    private void CreateMesh()
    {
        mesh.Clear();//メッシュのクリア

        //メモリの確保
        int n = points0.Count;
        Vector3[] vertexArray = new Vector3[2 * n];
        Vector2[] uvArray = new Vector2[2 * n];
        int[] indexArray = new int[6 * (n - 1)];

        int idv = 0, idi = 0;
        float duv = 1.0f / ((float)n - 1.0f);
        for (int i = 0; i < n; i++)
        {
            //超転座標
            vertexArray[idv + 0] = transform.InverseTransformPoint(points0[i]);
            vertexArray[idv + 1] = transform.InverseTransformPoint(points1[i]);

            //UV座標
            uvArray[idv + 0].x =
            uvArray[idv + 1].x = 1.0f - (float)i;
            uvArray[idv + 0].y = 0.0f;
            uvArray[idv + 1].y = 1.0f;

            //インデックス
            if (i == n - 1) break;//最後の辺では作らない
            indexArray[idi + 0] = idv;
            indexArray[idi + 1] = idv + 1;
            indexArray[idi + 2] = idv + 2;
            indexArray[idi + 3] = idv + 2;
            indexArray[idi + 4] = idv + 1;
            indexArray[idi + 5] = idv + 3;

            idv += 2;
            idi += 6;
        }

        mesh.vertices = vertexArray;
        mesh.uv = uvArray;
        mesh.triangles = indexArray;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }
}
