using AISandbox.AI;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

public static class Task3SandboxSceneBuilder
{
    private const string RootName = "Task3_NavMesh_AI_Sandbox";

    [MenuItem("Tools/Task3/Build NavMesh AI Sandbox")]
    public static void BuildSandbox()
    {
        GameObject oldRoot = GameObject.Find(RootName);
        if (oldRoot != null)
        {
            Object.DestroyImmediate(oldRoot);
        }

        GameObject root = new GameObject(RootName);

        Material ground = CreateMaterial("M_Ground_Arena", new Color(0.42f, 0.45f, 0.43f));
        Material wall = CreateMaterial("M_Wall_Dark", new Color(0.16f, 0.17f, 0.18f));
        Material obstacle = CreateMaterial("M_Obstacle_BlueGray", new Color(0.25f, 0.34f, 0.40f));
        Material goal = CreateMaterial("M_Goal_Green", new Color(0.1f, 0.72f, 0.34f));
        Material patrol = CreateMaterial("M_Patrol_Blue", new Color(0.1f, 0.35f, 1f));
        Material aiMat = CreateMaterial("M_AI_Yellow", new Color(1f, 0.84f, 0.15f));

        GameObject map = Child(root, "Map");
        GameObject mechanisms = Child(root, "Mechanisms");
        GameObject points = Child(root, "Points");
        GameObject actors = Child(root, "Actors");

        GameObject groundObject = Cube("Ground_NavMesh", new Vector3(0f, -0.25f, 0f), new Vector3(40f, 0.5f, 40f), ground, map.transform);
        groundObject.isStatic = true;

        BuildBoundary(map.transform, wall);
        BuildStaticObstacles(map.transform, obstacle);
        BuildMechanisms(mechanisms.transform);

        Transform target = Cylinder("Player_Target_Goal", new Vector3(14f, 0.25f, 14f), new Vector3(1.8f, 0.5f, 1.8f), goal, points.transform).transform;
        Transform[] patrolPoints =
        {
            Sphere("PatrolPoint_A", new Vector3(-13f, 0.35f, -13f), 0.7f, patrol, points.transform).transform,
            Sphere("PatrolPoint_B", new Vector3(10f, 0.35f, -11f), 0.7f, patrol, points.transform).transform,
            Sphere("PatrolPoint_C", new Vector3(-9f, 0.35f, 5f), 0.7f, patrol, points.transform).transform,
        };

        GameObject ai = Capsule("AI_StateMachine_Agent", new Vector3(-15f, 1f, -15f), new Vector3(1f, 1f, 1f), aiMat, actors.transform);
        NavMeshAgent agent = ai.AddComponent<NavMeshAgent>();
        agent.speed = 4.2f;
        agent.angularSpeed = 720f;
        agent.acceleration = 14f;
        agent.stoppingDistance = 1.2f;

        Rigidbody body = ai.AddComponent<Rigidbody>();
        body.isKinematic = true;
        body.freezeRotation = true;

        SandboxAIStateMachine stateMachine = ai.AddComponent<SandboxAIStateMachine>();
        ConfigureStateMachine(stateMachine, target, patrolPoints);

        Camera sceneCamera = Camera.main;
        if (sceneCamera == null)
        {
            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            sceneCamera = cameraObject.AddComponent<Camera>();
        }

        sceneCamera.transform.position = new Vector3(0f, 28f, -29f);
        sceneCamera.transform.rotation = Quaternion.Euler(58f, 0f, 0f);

        NavMeshSurface surface = groundObject.AddComponent<NavMeshSurface>();
        surface.collectObjects = CollectObjects.All;
        surface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
        surface.BuildNavMesh();

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("Task3 sandbox created. Use Tools/Task3/Build NavMesh AI Sandbox to rebuild it.");
    }

    private static void ConfigureStateMachine(
        SandboxAIStateMachine stateMachine,
        Transform target,
        Transform[] patrolPoints)
    {
        SerializedObject serializedStateMachine = new SerializedObject(stateMachine);
        serializedStateMachine.FindProperty("target").objectReferenceValue = target;

        SerializedProperty patrolPointsProperty = serializedStateMachine.FindProperty("patrolPoints");
        patrolPointsProperty.arraySize = patrolPoints.Length;
        for (int i = 0; i < patrolPoints.Length; i++)
        {
            patrolPointsProperty.GetArrayElementAtIndex(i).objectReferenceValue = patrolPoints[i];
        }

        serializedStateMachine.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void BuildBoundary(Transform parent, Material material)
    {
        Cube("Wall_North", new Vector3(0f, 1f, 20.5f), new Vector3(42f, 2f, 1f), material, parent).isStatic = true;
        Cube("Wall_South", new Vector3(0f, 1f, -20.5f), new Vector3(42f, 2f, 1f), material, parent).isStatic = true;
        Cube("Wall_East", new Vector3(20.5f, 1f, 0f), new Vector3(1f, 2f, 42f), material, parent).isStatic = true;
        Cube("Wall_West", new Vector3(-20.5f, 1f, 0f), new Vector3(1f, 2f, 42f), material, parent).isStatic = true;
    }

    private static void BuildStaticObstacles(Transform parent, Material material)
    {
        Cube("Obstacle_Left_Block", new Vector3(-7f, 1f, -5f), new Vector3(4f, 2f, 8f), material, parent).isStatic = true;
        Cube("Obstacle_Right_Block", new Vector3(7f, 1f, -2f), new Vector3(4f, 2f, 7f), material, parent).isStatic = true;
        Cube("Obstacle_Center_Island", new Vector3(0f, 1f, 5.5f), new Vector3(5f, 2f, 4f), material, parent).isStatic = true;
        Cube("NarrowBridge_LeftRail", new Vector3(-5.5f, 0.65f, 10f), new Vector3(1f, 1.3f, 10f), material, parent).isStatic = true;
        Cube("NarrowBridge_RightRail", new Vector3(5.5f, 0.65f, 10f), new Vector3(1f, 1.3f, 10f), material, parent).isStatic = true;
    }

    private static void BuildMechanisms(Transform parent)
    {
        Task3MechanismPrefabBuilder.EnsureMechanismPrefabs();

        InstantiatePrefab(
            Task3MechanismPrefabBuilder.RotatingBarPath,
            "RotatingBar_Hit_Stun",
            new Vector3(0f, 0f, -9f),
            parent);
        InstantiatePrefab(
            Task3MechanismPrefabBuilder.KnockbackPadPath,
            "KnockbackPad_Red",
            new Vector3(10f, 0.05f, 8f),
            parent);
        InstantiatePrefab(
            Task3MechanismPrefabBuilder.MovingWallPath,
            "MovingWall_Horizontal",
            new Vector3(-11f, 1f, 11f),
            parent);
    }

    private static GameObject InstantiatePrefab(string path, string instanceName, Vector3 position, Transform parent)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
        instance.name = instanceName;
        instance.transform.position = position;
        return instance;
    }

    private static GameObject Child(GameObject parent, string name)
    {
        GameObject child = new GameObject(name);
        child.transform.SetParent(parent.transform);
        return child;
    }

    private static GameObject Cube(string name, Vector3 position, Vector3 scale, Material material, Transform parent)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = name;
        obj.transform.position = position;
        obj.transform.localScale = scale;
        obj.transform.SetParent(parent);
        obj.GetComponent<Renderer>().sharedMaterial = material;
        return obj;
    }

    private static GameObject Sphere(string name, Vector3 position, float scale, Material material, Transform parent)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        obj.name = name;
        obj.transform.position = position;
        obj.transform.localScale = Vector3.one * scale;
        obj.transform.SetParent(parent);
        obj.GetComponent<Renderer>().sharedMaterial = material;
        return obj;
    }

    private static GameObject Capsule(string name, Vector3 position, Vector3 scale, Material material, Transform parent)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        obj.name = name;
        obj.transform.position = position;
        obj.transform.localScale = scale;
        obj.transform.SetParent(parent);
        obj.GetComponent<Renderer>().sharedMaterial = material;
        return obj;
    }

    private static GameObject Cylinder(string name, Vector3 position, Vector3 scale, Material material, Transform parent)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        obj.name = name;
        obj.transform.position = position;
        obj.transform.localScale = scale;
        obj.transform.SetParent(parent);
        obj.GetComponent<Renderer>().sharedMaterial = material;
        return obj;
    }

    private static Material CreateMaterial(string name, Color color)
    {
        const string folder = "Assets/Materials";
        string path = $"{folder}/{name}.mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material != null)
        {
            return material;
        }

        material = new Material(Shader.Find("Standard"));
        material.color = color;
        AssetDatabase.CreateAsset(material, path);
        return material;
    }
}
