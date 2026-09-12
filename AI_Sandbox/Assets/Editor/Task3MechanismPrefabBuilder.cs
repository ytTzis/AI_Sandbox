using AISandbox.Traps;
using UnityEditor;
using UnityEngine;

public static class Task3MechanismPrefabBuilder
{
    public const string PrefabFolder = "Assets/Prefabs/Mechanisms";
    public const string RotatingBarPath = PrefabFolder + "/RotatingBar.prefab";
    public const string KnockbackPadPath = PrefabFolder + "/KnockbackPad.prefab";
    public const string MovingWallPath = PrefabFolder + "/MovingWall.prefab";

    [MenuItem("Tools/Task3/Create Mechanism Prefabs")]
    public static void CreateMechanismPrefabs()
    {
        EnsureFolder("Assets/Prefabs");
        EnsureFolder(PrefabFolder);

        Material hazard = LoadOrCreateMaterial(
            "Assets/Materials/M_Hazard_Orange.mat",
            new Color(1f, 0.48f, 0.12f));
        Material knockback = LoadOrCreateMaterial(
            "Assets/Materials/M_Knockback_Red.mat",
            new Color(0.92f, 0.12f, 0.12f));

        if (AssetDatabase.LoadAssetAtPath<GameObject>(RotatingBarPath) == null)
        {
            SaveRotatingBar(hazard);
        }

        if (AssetDatabase.LoadAssetAtPath<GameObject>(KnockbackPadPath) == null)
        {
            SaveKnockbackPad(knockback);
        }

        if (AssetDatabase.LoadAssetAtPath<GameObject>(MovingWallPath) == null)
        {
            SaveMovingWall(knockback);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<GameObject>(RotatingBarPath);
        Debug.Log($"Task3 mechanism prefabs are ready in {PrefabFolder}.");
    }

    public static void EnsureMechanismPrefabs()
    {
        bool missingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(RotatingBarPath) == null
            || AssetDatabase.LoadAssetAtPath<GameObject>(KnockbackPadPath) == null
            || AssetDatabase.LoadAssetAtPath<GameObject>(MovingWallPath) == null;

        if (missingPrefab)
        {
            CreateMechanismPrefabs();
        }
    }

    private static void SaveRotatingBar(Material material)
    {
        GameObject root = new GameObject("RotatingBar");
        root.AddComponent<RotatingMechanism>();

        GameObject center = CreateCube(
            "Center",
            new Vector3(0f, 0.75f, 0f),
            new Vector3(1.2f, 1.5f, 1.2f),
            material,
            root.transform);
        GameObject arm = CreateCube(
            "HitArm",
            new Vector3(0f, 1.1f, 0f),
            new Vector3(11f, 0.35f, 0.55f),
            material,
            root.transform);

        arm.GetComponent<BoxCollider>().isTrigger = true;
        arm.AddComponent<KnockbackMechanism>();
        AddKinematicBody(root);

        PrefabUtility.SaveAsPrefabAsset(root, RotatingBarPath);
        Object.DestroyImmediate(root);
    }

    private static void SaveKnockbackPad(Material material)
    {
        GameObject pad = CreateCube(
            "KnockbackPad",
            Vector3.zero,
            new Vector3(5f, 0.1f, 4f),
            material,
            null);

        pad.GetComponent<BoxCollider>().isTrigger = true;
        pad.AddComponent<KnockbackMechanism>();
        AddKinematicBody(pad);

        PrefabUtility.SaveAsPrefabAsset(pad, KnockbackPadPath);
        Object.DestroyImmediate(pad);
    }

    private static void SaveMovingWall(Material material)
    {
        GameObject wall = CreateCube(
            "MovingWall",
            Vector3.zero,
            new Vector3(1.2f, 2f, 6f),
            material,
            null);

        wall.GetComponent<BoxCollider>().isTrigger = true;
        wall.AddComponent<MovingMechanism>();
        wall.AddComponent<KnockbackMechanism>();
        AddKinematicBody(wall);

        PrefabUtility.SaveAsPrefabAsset(wall, MovingWallPath);
        Object.DestroyImmediate(wall);
    }

    private static GameObject CreateCube(
        string name,
        Vector3 localPosition,
        Vector3 localScale,
        Material material,
        Transform parent)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = name;
        obj.transform.SetParent(parent, false);
        obj.transform.localPosition = localPosition;
        obj.transform.localScale = localScale;
        obj.GetComponent<Renderer>().sharedMaterial = material;
        return obj;
    }

    private static void AddKinematicBody(GameObject obj)
    {
        Rigidbody body = obj.AddComponent<Rigidbody>();
        body.isKinematic = true;
        body.useGravity = false;
    }

    private static Material LoadOrCreateMaterial(string path, Color color)
    {
        EnsureFolder("Assets/Materials");

        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material != null)
        {
            return material;
        }

        Shader shader = Shader.Find("Standard");
        material = new Material(shader)
        {
            color = color
        };
        AssetDatabase.CreateAsset(material, path);
        return material;
    }

    private static void EnsureFolder(string folderPath)
    {
        if (AssetDatabase.IsValidFolder(folderPath))
        {
            return;
        }

        int slashIndex = folderPath.LastIndexOf('/');
        string parent = folderPath.Substring(0, slashIndex);
        string folderName = folderPath.Substring(slashIndex + 1);
        EnsureFolder(parent);
        AssetDatabase.CreateFolder(parent, folderName);
    }
}
