using System.Collections.Generic;
using UnityEngine;



public class Spawner : MonoBehaviour
{
    [SerializeField]
    [Range(1, 1000)]
    int numberToSpawn = 10;
    [SerializeField]
    [Range(1, 50)]
    int spheraSpawn = 10;
    [SerializeField]
    Mesh[] meshes;
    [SerializeField]
    Material mat;

    static int colorPropertyId = Shader.PropertyToID("_CircleGroundColor");
    static int colorPropertyIdSimpleLit = Shader.PropertyToID("_BaseColor");

    List<GameObject> instantiatedShapes = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        SpawnAll();

    }

    public void SpawnAll()
    {
        KillAll();
        for (int i = 0; i < numberToSpawn; i++)
        {
            //Color randColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            Color randColor = Color.white;
            Vector3 randPos = Random.insideUnitSphere * spheraSpawn;
            float scale = Random.Range(0.1f, 1f);
            AddShape(meshes[Random.Range(0, meshes.Length)], randColor, mat, randPos, scale);
        }
    }

    private void AddShape(Mesh mesh, Color color, Material material, Vector3 position, float scale)
    {
        GameObject _shape = new GameObject();
        _shape.transform.name = "shape_";
        _shape.transform.localPosition = position;
        _shape.transform.localScale = Vector3.one* scale;
        _shape.AddComponent<MeshFilter>().mesh = mesh;
        _shape.AddComponent<MeshRenderer>().material = material;
        SetColor(color, _shape.GetComponent<MeshRenderer>());
        _shape.transform.parent = this.transform;
        _shape.transform.localRotation = Random.rotation;
        instantiatedShapes.Add(_shape);

    }

    void KillAll()
    {
        foreach (var gameObject in instantiatedShapes)
        {
            Destroy(gameObject);
        }
        instantiatedShapes.Clear();
    }

    public void SetColor(Color color, MeshRenderer meshRenderer)
    {
        var propertyBlock = new MaterialPropertyBlock();
        propertyBlock.SetColor(colorPropertyId, color);
        propertyBlock.SetColor(colorPropertyIdSimpleLit, color);
        meshRenderer.SetPropertyBlock(propertyBlock);
    }

}
