using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk 
{

    public GameObject chunkObject;

    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();


    MeshFilter meshFilter;
    MeshCollider meshCollider;
    MeshRenderer meshRenderer;

    Vector3Int chunkPosition;
    float[,,] terrainMap;

    int width { get { return GameData.ChunkWidth; } }
    int height { get { return GameData.ChunkHeight; } }
    float terrainSurface { get { return GameData.terrainSurface; } }

    public Chunk(Vector3Int _position) {

        chunkObject = new GameObject();
        chunkObject.name = string.Format("Chunk {0}, {1}", _position.x, _position.z);
        chunkPosition = _position;
        chunkObject.transform.position = chunkPosition;


        // Add the required components.
        meshCollider = chunkObject.AddComponent<MeshCollider>();
        meshFilter = chunkObject.AddComponent<MeshFilter>();
        meshRenderer = chunkObject.AddComponent<MeshRenderer>();
        meshRenderer.material = Resources.Load<Material>("Materials/Terrain");
        // Set the tag of the terrain
        chunkObject.transform.tag = "Terrain";

        // Initialize the terrain map
        terrainMap = new float[width + 1, height + 1, width + 1];

        PopulateTerrainMap();
        CreateMeshData();
        //BuildMesh();
    }

    int GetCubeConfiguration(float[] cube) {
        // Starting with a configuration of zero, loop through each point in the cube and check if it is below the terrain surface.
        int configurationIndex = 0;
        for(int i = 0; i < 8; i++) {

            // If it is, set the corresponding bit to 1.
            if (cube[i] > terrainSurface)
                configurationIndex |= 1 << i;
        }

        return configurationIndex;
    }

    public void PlaceTerrain(Vector3 pos) {
        Vector3Int v3Int = new Vector3Int(Mathf.CeilToInt(pos.x), Mathf.CeilToInt(pos.y), Mathf.CeilToInt(pos.z));
        v3Int -= chunkPosition;
        terrainMap[v3Int.x, v3Int.y, v3Int.z] = 0f;
        CreateMeshData();
    }

    public void RemoveTerrain(Vector3 pos) {

        Vector3Int v3Int = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
        v3Int -= chunkPosition;
        terrainMap[v3Int.x, v3Int.y, v3Int.z] = 1f;
        CreateMeshData();
    }

    // Helper function
    float SampleTerrain(Vector3Int point) {
        return terrainMap[point.x, point.y, point.z];
    }

    // Helper function
    int VertForIndice(Vector3 vert) {
        // Loop through all the vertices currently in the vertices list.
        for(int i = 0; i < vertices.Count; i++) {

            // If a vert is found that matches, return the index.
            if (vertices[i] == vert)
                return i;
        }

        // if no match is found, add this vert to the list and return the last index. 
        vertices.Add(vert);
        return vertices.Count - 1;
    }

    void PopulateTerrainMap() {

        // The data points for terrain are stored at the corners of the "cubes", so the terrainMap needs to be 1 larger
        // than the width/height of the mesh.
        for(int x = 0; x < width + 1; x++) {
            for(int z = 0; z < width + 1; z++) {
                for (int y = 0; y < height + 1; y++) {
                    // Get a terrain height using Perlin noise.
                    // TODO: Possibly replace with a more customizable perlin noise function.
                    float thisHeight;

                    thisHeight = GameData.GetTerrainHeight(x + chunkPosition.x, z + chunkPosition.z);

                   
                    // Set the value of this point in the terrainMap
                    terrainMap[x, y, z] = (float)y - thisHeight;
                }
            }
        }
    }

    void CreateMeshData() {

        ClearMeshData();

        // Loop through each "cube" in the terrain.
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                for (int z = 0; z < width; z++) {

                    // Pass the values into the MarchCube function.
                    MarchCube(new Vector3Int(x, y, z));
                }
            }
        }

        BuildMesh();
    }

    void MarchCube(Vector3Int position) {
        // Sample terrain values at each corner of the cube
        float[] cube = new float[8];
        for(int i = 0; i < 8; i++) {
            cube[i] = SampleTerrain(position + GameData.CornerTable[i]);
        }
        // Get the configuration index of this cube.
        int configIndex = GetCubeConfiguration(cube);

        // If the configuration of this cube is 0 or 255 (completely inside the terrain or completely outside of it) don't do anything.
        if (configIndex == 0 || configIndex == 255)
            return;

        // Loop through the triangles. There are never more than five triangles to a cube and only three vertices to a triangle.
        int edgeIndex = 0;
        for(int i = 0; i < 5; i++) {
            for(int p = 0; p < 3; p++) {

                // Get current indice. Increment triangeIndex each loop.
                int indice = GameData.TriangleTable[configIndex, edgeIndex];

                // If the current edgeIndex is -1, there are no more indices.
                if (indice == -1)
                    return;

                // Get the vertices for the start and end of this edge.
                Vector3 vert1 = position + GameData.CornerTable[GameData.EdgeIndexes[indice, 0]];
                Vector3 vert2 = position + GameData.CornerTable[GameData.EdgeIndexes[indice, 1]];

                Vector3 vertPosition;
               
                // Get the midpoint of this edge.
                vertPosition = (vert1 + vert2) / 2f;
                


                // Add to the vertices and triangles list and increment the edgeIndex.

                vertices.Add(vertPosition);
                triangles.Add(vertices.Count - 1);


                edgeIndex++;
            }
        }
    }

    void ClearMeshData() {
        vertices.Clear();
        triangles.Clear();
    }

    void BuildMesh() {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
    }

   
}
