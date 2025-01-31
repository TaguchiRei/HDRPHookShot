using UnityEngine;

public class WireFrame : MonoBehaviour
{
    [SerializeField] MeshFilter meshFilter;
    void Start()
    {
        meshFilter.mesh.SetIndices(meshFilter.mesh.GetIndices(0),MeshTopology.Lines, 0);
    }
}
