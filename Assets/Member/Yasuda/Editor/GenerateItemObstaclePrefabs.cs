using UnityEditor;
using UnityEngine;

public static class GenerateItemObstaclePrefabs
{
    private const string BasePath = "Assets/Member/Yasuda";
    private const string MaterialsPath = BasePath + "/Materials";
    private const string PrefabsPath = BasePath + "/Prefabs";

    [MenuItem("Assets/Create/Yasuda/Generate Item and Obstacle Prefabs")]
    public static void Generate()
    {
        EnsureFolder("Assets", "Member");
        EnsureFolder("Assets/Member", "Yasuda");
        EnsureFolder(BasePath, "Materials");
        EnsureFolder(BasePath, "Prefabs");

        var itemMat = CreateOrLoadMaterial(MaterialsPath + "/ItemMat.mat", new Color(0.2f, 0.8f, 0.3f, 1f));
        var obstacleMat = CreateOrLoadMaterial(MaterialsPath + "/ObstacleMat.mat", new Color(0.85f, 0.2f, 0.2f, 1f));

        CreatePrefab(
            "ItemPrefab",
            PrefabsPath + "/ItemPrefab.prefab",
            PrimitiveType.Sphere,
            itemMat,
            typeof(MaskItemObject)
        );

        CreatePrefab(
            "ObstaclePrefab",
            PrefabsPath + "/ObstaclePrefab.prefab",
            PrimitiveType.Cube,
            obstacleMat,
            typeof(ObstacleObject)
        );

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void EnsureFolder(string parent, string name)
    {
        var path = $"{parent}/{name}";
        if (!AssetDatabase.IsValidFolder(path))
        {
            AssetDatabase.CreateFolder(parent, name);
        }
    }

    private static Material CreateOrLoadMaterial(string path, Color color)
    {
        var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat != null)
        {
            return mat;
        }

        var shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
        {
            shader = Shader.Find("Standard");
        }

        mat = new Material(shader);
        mat.color = color;
        AssetDatabase.CreateAsset(mat, path);
        return mat;
    }

    private static void CreatePrefab(string name, string path, PrimitiveType primitive, Material material, System.Type scriptType)
    {
        var existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (existing != null)
        {
            return;
        }

        var go = GameObject.CreatePrimitive(primitive);
        go.name = name;

        var renderer = go.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.sharedMaterial = material;
        }

        if (scriptType != null && go.GetComponent(scriptType) == null)
        {
            go.AddComponent(scriptType);
        }

        PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
    }
}
