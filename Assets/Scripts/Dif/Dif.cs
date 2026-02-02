// References:
// P-Invoke for strings:
//   http://stackoverflow.com/questions/370079/pinvoke-for-c-function-that-returns-char/370519#370519
//
// Note:
// This script must execute before other classes can be used. Make sure the execution is prior
// to the default time.

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Dif : MonoBehaviour
{

    public string filePath;
    public Material DefaultMaterial;

    [Header("Collision / Chunking")]
    public int maxTrianglesPerChunk = 1000;

    bool isMovingPlatform;

    public bool GenerateMesh(int interiorIndex)
    {
        isMovingPlatform = (interiorIndex != -1);

        // Load DIF resource
        var resource = DifResourceManager.getResource(Path.Combine(Application.streamingAssetsPath, filePath), interiorIndex);

        if (resource == null)
        {
            Debug.LogError("Dif decode failed");
            return false;
        }

        if (resource.vertices == null ||
            resource.normals == null ||
            resource.tangents == null ||
            resource.uvs == null)
        {
            Debug.LogError(
                $"Invalid DIF resource\n" +
                $"verts: {resource.vertices != null}\n" +
                $"normals: {resource.normals != null}\n" +
                $"tangents: {resource.tangents != null}\n" +
                $"uvs: {resource.uvs != null}"
            );

            return false;
        }

        // Torque (Z-up) → Unity (Y-up)
        Quaternion torqueToUnity = Quaternion.Euler(90f, 0f, 0f);

        // Remove any old colliders
        foreach (var c in GetComponents<MeshCollider>())
            DestroyImmediate(c);

        int chunkIndex = 0;

        var materialTriMap = new Dictionary<string, List<int>>();

        for (int mat = 0; mat < resource.triangleIndices.Length; mat++)
        {
            int[] materialTris = resource.triangleIndices[mat];
            if (materialTris == null || materialTris.Length == 0)
                continue;

            string matName = resource.materials[mat];
            Material material = ResolveMaterial(matName);

            if (!materialTriMap.ContainsKey(matName))
                materialTriMap[matName] = new List<int>();
            materialTriMap[matName].AddRange(materialTris);
        }

        // Create chunks
        foreach (var kvp in materialTriMap)
        {
            string matName = kvp.Key;
            List<int> allTris = kvp.Value;
            Material material = ResolveMaterial(matName);

            for (int i = 0; i < allTris.Count; i += maxTrianglesPerChunk * 3)
            {
                int triCount = Mathf.Min(maxTrianglesPerChunk * 3, allTris.Count - i);
                int[] tris = allTris.GetRange(i, triCount).ToArray();

                CreateChunk(chunkIndex, tris, torqueToUnity, resource, material);
                chunkIndex++;
            }
        }

        return true;
    }

    void CreateChunk(
    int index,
    int[] tris,
    Quaternion torqueToUnity,
    DifResource resource,
    Material material
)
    {
        GameObject chunk = new GameObject($"DIF_Chunk_{index}");
        chunk.transform.SetParent(transform, false);
        chunk.isStatic = true;

        // --- Build render mesh ---
        Mesh mesh = new Mesh();
        mesh.name = $"DIF_Mesh_{index}";

        mesh.vertices = resource.vertices
            .Select(v => torqueToUnity * new Vector3(v.x, -v.y, v.z))
            .ToArray();

        mesh.normals = resource.normals
            .Select(n => torqueToUnity * new Vector3(n.x, -n.y, n.z))
            .ToArray();

        mesh.uv = resource.uvs;
        mesh.triangles = tris;
        mesh.RecalculateBounds();

        var mf = chunk.AddComponent<MeshFilter>();
        var mr = chunk.AddComponent<MeshRenderer>();

        if (isMovingPlatform)
        {
            var rb = chunk.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        mf.sharedMesh = mesh;
        mr.sharedMaterial = material; // ✅ CORRECT MATERIAL

        // --- Physics mesh ---
        Mesh physicsMesh = Instantiate(mesh);
        physicsMesh = WeldPhysicsMesh(physicsMesh, 0.01f);
        physicsMesh = RemoveTinyTriangles(physicsMesh, 0.0005f);
        physicsMesh.RecalculateNormals();
        physicsMesh.RecalculateBounds();

        var mc = chunk.AddComponent<MeshCollider>();
        mc.sharedMesh = physicsMesh;
        mc.convex = false;

        // Friction
        var friction = chunk.AddComponent<FrictionComponent>();
        // Resolve friction
        if (materialDict.TryGetValue(material.name, out var matProps))
        {
            friction.friction = matProps.friction;
            friction.restitution = matProps.restitution;
            friction.bounce = matProps.force;
        }
        else
        {
            friction.friction = 1.0f;
            friction.restitution = 1.0f;
        }
    }

    Material ResolveMaterial(string materialName)
    {
        if (string.IsNullOrEmpty(materialName))
            return DefaultMaterial;

        var mat = Instantiate(DefaultMaterial);
        mat.name = materialName;

        string texPath = ResolveTexturePath(materialName);
        if (File.Exists(texPath))
        {
            var tex = new Texture2D(2, 2);
            tex.LoadImage(File.ReadAllBytes(texPath));
            mat.mainTexture = tex;
        }

        return mat;
    }


    Mesh RemoveTinyTriangles(Mesh mesh, float minArea)
    {
        var verts = mesh.vertices;
        var tris = mesh.triangles;

        var newTris = new List<int>(tris.Length);

        for (int i = 0; i < tris.Length; i += 3)
        {
            Vector3 a = verts[tris[i]];
            Vector3 b = verts[tris[i + 1]];
            Vector3 c = verts[tris[i + 2]];

            float area = Vector3.Cross(b - a, c - a).magnitude * 0.5f;
            if (area >= minArea)
            {
                newTris.Add(tris[i]);
                newTris.Add(tris[i + 1]);
                newTris.Add(tris[i + 2]);
            }
        }

        mesh.triangles = newTris.ToArray();
        return mesh;
    }


    // Weld by position only (for physics mesh)
    private Mesh WeldPhysicsMesh(Mesh mesh, float tolerance)
    {
        var oldVerts = mesh.vertices;
        var oldTris = mesh.triangles;

        var newVerts = new List<Vector3>();
        var remap = new int[oldVerts.Length];
        var dict = new Dictionary<Vector3, int>();

        for (int i = 0; i < oldVerts.Length; i++)
        {
            Vector3 v = oldVerts[i];
            Vector3 key = new Vector3(
                Mathf.Round(v.x / tolerance) * tolerance,
                Mathf.Round(v.y / tolerance) * tolerance,
                Mathf.Round(v.z / tolerance) * tolerance
            );

            if (!dict.TryGetValue(key, out int idx))
            {
                idx = newVerts.Count;
                newVerts.Add(v);
                dict[key] = idx;
            }
            remap[i] = idx;
        }

        var newTris = new int[oldTris.Length];
        for (int i = 0; i < oldTris.Length; i++)
            newTris[i] = remap[oldTris[i]];

        mesh.Clear();
        mesh.vertices = newVerts.ToArray();
        mesh.triangles = newTris;
        return mesh;
    }

    // Force flat shading normals for physics mesh
    private Mesh FlatShadePhysicsMesh(Mesh mesh)
    {
        var oldVerts = mesh.vertices;
        var oldTris = mesh.triangles;

        // Each triangle gets its own unique vertices
        var newVerts = new List<Vector3>();
        var newNormals = new List<Vector3>();
        var newTris = new List<int>();

        for (int i = 0; i < oldTris.Length; i += 3)
        {
            int i0 = oldTris[i];
            int i1 = oldTris[i + 1];
            int i2 = oldTris[i + 2];

            Vector3 v0 = oldVerts[i0];
            Vector3 v1 = oldVerts[i1];
            Vector3 v2 = oldVerts[i2];

            // Compute one normal for the whole face
            Vector3 faceNormal = Vector3.Cross(v1 - v0, v2 - v0).normalized;

            int baseIndex = newVerts.Count;
            newVerts.Add(v0); newNormals.Add(faceNormal);
            newVerts.Add(v1); newNormals.Add(faceNormal);
            newVerts.Add(v2); newNormals.Add(faceNormal);

            newTris.Add(baseIndex);
            newTris.Add(baseIndex + 1);
            newTris.Add(baseIndex + 2);
        }

        mesh.Clear();
        mesh.vertices = newVerts.ToArray();
        mesh.normals = newNormals.ToArray();
        mesh.triangles = newTris.ToArray();
        mesh.RecalculateBounds();
        return mesh;
    }


    private string ResolveTexturePath(string texture)
    {
        var basePath = Path.GetDirectoryName(filePath);
        while (!string.IsNullOrEmpty(basePath))
        {
            var assetPath = Path.Combine(Application.streamingAssetsPath, basePath);
            var possibleTextures = new List<string>
            {
                Path.Combine(assetPath, texture + ".png"),
                Path.Combine(assetPath, texture + ".jpg"),
                Path.Combine(assetPath, texture + ".jp2"),
                Path.Combine(assetPath, texture + ".bmp"),
                Path.Combine(assetPath, texture + ".bm8"),
                Path.Combine(assetPath, texture + ".gif"),
                Path.Combine(assetPath, texture + ".dds"),
            };
            foreach (var possibleTexture in possibleTextures)
            {
                if (File.Exists(possibleTexture))
                {
                    return possibleTexture;
                }
            }

            basePath = Path.GetDirectoryName(basePath);
        }

        return texture;
    }

    static Dictionary<string, (float friction, float restitution, float force)> materialDict = new Dictionary<string, (float, float, float)>()
    {
        {"friction_none", (0.01f, 0.5f, 0f)},
        {"friction_low", (0.2f, 0.5f, 0f)},
        {"friction_low_shadow", (0.2f, 0.5f, 0f)},
        {"friction_high", (4.5f, 0.5f, 0f)},
        {"friction_high_shadow", (4.5f, 0.5f, 0f)},
        {"friction_ultrahigh", (4.5f, 0.5f, 0f)},
        {"friction_ramp_yellow", (2.0f, 1.0f, 0f)},
        {"oilslick", (0.05f, 0.5f, 0f)},
        {"base.slick", (0.05f, 0.5f, 0f)},
        {"ice.slick", (0.05f, 0.5f, 0f)},
        {"grass", (1.5f, 0.35f, 0f)},
        {"ice1", (0.03f, 0.95f, 0f)},
        {"rug", (6.0f, 0.5f, 0f)},
        {"tarmac", (0.35f, 0.7f, 0f)},
        {"carpet", (6.0f, 0.5f, 0f)},
        {"sand", (4.0f, 0.1f, 0f)},
        {"water", (6.0f, 0.0f, 0f)},
        {"floor_bounce", (0.2f, 0.0f, 15f)},
        {"mbp_chevron_friction", (-1.0f, 1.0f, 0f)},
        {"mbp_chevron_friction2", (-1.0f, 1.0f, 0f)},
        {"mbp_chevron_friction3", (-1.0f, 1.0f, 0f)},
        {"mmg_grass", (0.9f, 0.5f, 0f)},
        {"mmg_sand", (6.0f, 0.1f, 0f)},
        {"mmg_water", (6.0f, 0.0f, 0f)},
        {"mmg_ice", (0.03f, 0.95f, 0f)},
        {"mmg_ice_shadow", (0.03f, 0.95f, 0f)},
        {"friction_mp_high", (6f, 0.3f, 0f)},
        {"friction_mp_high_shadow", (6f, 0.3f, 0f)},
        {"friction_bouncy", (0.2f, 2.0f, 15f)}
    };
}
