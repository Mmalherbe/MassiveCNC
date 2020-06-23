using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSmoother : MonoBehaviour
{

    MeshFilter meshfilter;
    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;

    private int[] subdivision = new int[] { 0, 2, 3, 4, 6, 8, 9, 12, 16, 18, 24 };

    [Header("Subdive Mesh")]

    [Tooltip("Divide meshes in submeshes to generate more triangles")]
    [Range(0, 10)]
    public int subdivisionLevel;

    [Tooltip("Repeat the process this many times")]
    [Range(0, 10)]
    public int timesToSubdivide;

    public void Smooth()
    {
        meshfilter = GetComponent<MeshFilter>();
        mesh = meshfilter.mesh;
        vertices = mesh.vertices;
        triangles = mesh.triangles;

        for (int i = 0; i < timesToSubdivide; i++)
        {
            MeshHelper.Subdivide(mesh, subdivision[subdivisionLevel]);
        }
        meshfilter.mesh = mesh;
        vertices = mesh.vertices;
    }
}
