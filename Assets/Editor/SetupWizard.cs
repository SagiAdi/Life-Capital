using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

// Menu: Tools > Life Capital > Create Everything
// Creates the data assets, the prefabs and the two scenes, connects their references,
// and adds the scenes to the build list. It is safe to run it again: everything is rebuilt.
public static class SetupWizard
{
    private const string DataFolder = "Assets/Data";
    private const string PrefabsFolder = "Assets/Prefabs";
    private const string ScenesFolder = "Assets/Scenes";
    private const string PlayerSheet = "Assets/Art/Characters/player.png";

    private const string MessagesPath = DataFolder + "/Events/Messages.asset";
    private const string BuildingEnteredPath = DataFolder + "/Events/BuildingEntered.asset";
    private const string SettingsPath = DataFolder + "/EconomySettings.asset";
    private const string StockPath = DataFolder + "/GreenTech.asset";
    private const string CoinPrefabPath = PrefabsFolder + "/Coin.prefab";
    private const string BillPrefabPath = PrefabsFolder + "/Bill.prefab";

    private static readonly Color WallColor = new Color(0.35f, 0.35f, 0.42f);

    // The game is seen from above, so nothing may move or rotate out of the X-Y plane.
    private const RigidbodyConstraints FlatConstraints =
        RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;

    [MenuItem("Tools/Life Capital/Create Everything")]
    public static void CreateEverything()
    {
        EnsureFolder("Assets", "Data");
        EnsureFolder(DataFolder, "Events");
        EnsureFolder(DataFolder, "Bills");
        EnsureFolder("Assets", "Prefabs");
        EnsureFolder("Assets", "Scenes");

        ImportSheet(PlayerSheet);

        // Event channels and data.
        LoadOrCreate<MessageChannel>(MessagesPath);
        LoadOrCreate<BuildingChannel>(BuildingEnteredPath);
        LoadOrCreate<EconomySettings>(SettingsPath);
        LoadOrCreate<StockData>(StockPath);

        string[] billTypePaths =
        {
            CreateBillType("PhoneBill", "Phone bill", 300, new Color(0.95f, 0.3f, 0.3f), 0.45f, 3.6f),
            CreateBillType("CarRepair", "Car repair", 900, new Color(0.7f, 0.1f, 0.25f), 0.8f, 2.4f),
        };

        SavePrefab(BuildCoin(), CoinPrefabPath);
        SavePrefab(BuildBill(), BillPrefabPath);
        AssetDatabase.SaveAssets();

        // Scenes. Opening a new scene unloads the assets that were loaded above,
        // so the city scene loads the assets it needs again, by path.
        CreateMenuScene();
        CreateCityScene(billTypePaths);

        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene(ScenePath("Menu"), true),
            new EditorBuildSettingsScene(ScenePath("City"), true),
        };

        AssetDatabase.SaveAssets();
        EditorSceneManager.OpenScene(ScenePath("Menu"));
        Debug.Log("Life Capital is ready. The Menu scene is open: press Play.");
    }

    // ---------- scenes ----------

    private static void CreateMenuScene()
    {
        var scene = NewScene();
        SetupCamera(new Color(0.08f, 0.1f, 0.16f));
        new GameObject("Menu Screen").AddComponent<MenuScreen>();
        EditorSceneManager.SaveScene(scene, ScenePath("Menu"));
    }

    private static void CreateCityScene(string[] billTypePaths)
    {
        var scene = NewScene();
        SetupCamera(new Color(0.16f, 0.2f, 0.18f));

        var messages = Load<MessageChannel>(MessagesPath);
        var buildingEntered = Load<BuildingChannel>(BuildingEnteredPath);
        var settings = Load<EconomySettings>(SettingsPath);
        var stock = Load<StockData>(StockPath);
        BillType[] billTypes = LoadAll<BillType>(billTypePaths);
        Coin coinPrefab = Load<Coin>(CoinPrefabPath);
        Bill billPrefab = Load<Bill>(BillPrefabPath);

        // Economy: one object, one small component for every part of the player's money.
        var economy = new GameObject("Economy");
        var wallet = economy.AddComponent<Wallet>();
        var savings = economy.AddComponent<SavingsAccount>();
        var portfolio = economy.AddComponent<Portfolio>();
        var career = economy.AddComponent<Career>();
        var monthCycle = economy.AddComponent<MonthCycle>();
        var netWorth = economy.AddComponent<NetWorth>();

        Wire(wallet, "settings", settings);
        Wire(savings, "wallet", wallet);
        Wire(portfolio, "stock", stock);
        Wire(portfolio, "wallet", wallet);
        Wire(career, "settings", settings);
        Wire(monthCycle, "settings", settings);
        Wire(monthCycle, "wallet", wallet);
        Wire(monthCycle, "career", career);
        Wire(monthCycle, "savings", savings);
        Wire(monthCycle, "portfolio", portfolio);
        Wire(netWorth, "wallet", wallet);
        Wire(netWorth, "savings", savings);
        Wire(netWorth, "portfolio", portfolio);

        // Game rules.
        var game = new GameObject("Game");
        var pause = game.AddComponent<GamePause>();
        var rules = game.AddComponent<GameRules>();
        Wire(rules, "settings", settings);
        Wire(rules, "monthCycle", monthCycle);
        Wire(rules, "wallet", wallet);
        Wire(rules, "netWorth", netWorth);
        Wire(rules, "pause", pause);

        // Outer walls.
        Transform walls = new GameObject("Walls").transform;
        BuildBlock("Wall Top", new Vector2(0f, 4.5f), new Vector2(15.5f, 0.5f), WallColor, 0).transform.SetParent(walls);
        BuildBlock("Wall Bottom", new Vector2(0f, -4.5f), new Vector2(15.5f, 0.5f), WallColor, 0).transform.SetParent(walls);
        BuildBlock("Wall Left", new Vector2(-7.5f, 0f), new Vector2(0.5f, 9.5f), WallColor, 0).transform.SetParent(walls);
        BuildBlock("Wall Right", new Vector2(7.5f, 0f), new Vector2(0.5f, 9.5f), WallColor, 0).transform.SetParent(walls);

        // Buildings. Each one is a solid block with its own Building script.
        Transform buildings = new GameObject("Buildings").transform;
        Vector2 buildingSize = new Vector2(2.6f, 1.6f);

        var home = AddBuilding<HomeBuilding>(buildings, "Home", new Vector2(-5.7f, 2.9f), buildingSize, new Color(0.85f, 0.4f, 0.4f), buildingEntered);
        Wire(home, "monthCycle", monthCycle);

        var school = AddBuilding<SchoolBuilding>(buildings, "School", new Vector2(0f, 2.9f), buildingSize, new Color(0.95f, 0.6f, 0.25f), buildingEntered);
        Wire(school, "monthCycle", monthCycle);
        Wire(school, "wallet", wallet);
        Wire(school, "career", career);

        var work = AddBuilding<WorkBuilding>(buildings, "Work", new Vector2(5.7f, 2.9f), buildingSize, new Color(0.95f, 0.8f, 0.25f), buildingEntered);
        var fountain = new GameObject("Coin Fountain").AddComponent<CoinFountain>();
        fountain.transform.position = new Vector3(5.7f, 1.5f, 0f);
        fountain.transform.SetParent(buildings);
        Wire(fountain, "coinPrefab", coinPrefab);
        Wire(work, "monthCycle", monthCycle);
        Wire(work, "coinFountain", fountain);
        Wire(work, "messages", messages);

        var bank = AddBuilding<BankBuilding>(buildings, "Bank", new Vector2(-5.7f, -3.1f), buildingSize, new Color(0.3f, 0.55f, 0.95f), buildingEntered);
        Wire(bank, "monthCycle", monthCycle);
        Wire(bank, "savings", savings);
        Wire(bank, "settings", settings);

        var exchange = AddBuilding<ExchangeBuilding>(buildings, "Exchange", new Vector2(5.7f, -3.1f), buildingSize, new Color(0.3f, 0.75f, 0.45f), buildingEntered);
        Wire(exchange, "monthCycle", monthCycle);
        Wire(exchange, "portfolio", portfolio);

        // Player: a CharacterController (kinematic), seen from above.
        var player = new GameObject("Player");
        player.transform.position = Vector3.zero;
        var playerRenderer = player.AddComponent<SpriteRenderer>();
        playerRenderer.sortingOrder = 2;
        player.AddComponent<Placeholder>();

        var controller = player.AddComponent<CharacterController>();
        controller.radius = 0.3f;
        controller.height = 0.6f;
        controller.center = new Vector3(0f, -0.2f, 0f);
        controller.stepOffset = 0f;
        controller.skinWidth = 0.03f;
        controller.minMoveDistance = 0f;

        var input = player.AddComponent<PlayerInputReader>();
        Wire(player.AddComponent<PlayerWalker>(), "pause", pause);
        var sensor = player.AddComponent<BuildingSensor>();
        Wire(player.AddComponent<PlayerInteractor>(), "pause", pause);
        var money = player.AddComponent<PlayerMoney>();
        Wire(money, "wallet", wallet);
        Wire(money, "messages", messages);
        AddAnimator(player, PlayerSheet);
        player.AddComponent<WalkAnimationDriver>();

        // Bills: unexpected expenses that chase the player from the second month.
        var spawner = new GameObject("Bill Spawner").AddComponent<BillSpawner>();
        var spawnPoints = new[]
        {
            AddPoint(spawner.transform, "Spawn Left", new Vector2(-6.6f, 0f)),
            AddPoint(spawner.transform, "Spawn Right", new Vector2(6.6f, 0f)),
            AddPoint(spawner.transform, "Spawn Bottom", new Vector2(0f, -3.7f)),
        };
        Wire(spawner, "monthCycle", monthCycle);
        Wire(spawner, "pause", pause);
        Wire(spawner, "target", player.transform);
        Wire(spawner, "billPrefab", billPrefab);
        WireArray(spawner, "types", billTypes);
        WireArray(spawner, "spawnPoints", spawnPoints);

        // UI: every element listens to the game through events or channels.
        Transform ui = new GameObject("UI").transform;
        var hud = AddUi<EconomyHud>(ui, "Economy HUD");
        Wire(hud, "settings", settings);
        Wire(hud, "wallet", wallet);
        Wire(hud, "savings", savings);
        Wire(hud, "portfolio", portfolio);
        Wire(hud, "career", career);
        Wire(hud, "monthCycle", monthCycle);
        Wire(hud, "netWorth", netWorth);

        Wire(AddUi<MessageLabel>(ui, "Message Label"), "messages", messages);
        Wire(AddUi<InteractPrompt>(ui, "Interact Prompt"), "sensor", sensor);
        Wire(AddUi<TouchControls>(ui, "Touch Controls"), "input", input);
        Wire(AddUi<EndScreen>(ui, "End Screen"), "gameRules", rules);

        var panel = AddUi<BuildingPanel>(ui, "Building Panel");
        Wire(panel, "buildingEntered", buildingEntered);
        Wire(panel, "pause", pause);

        EditorSceneManager.SaveScene(scene, ScenePath("City"));
    }

    // ---------- building blocks ----------

    private static T AddUi<T>(Transform parent, string name) where T : Component
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent);
        return go.AddComponent<T>();
    }

    private static Transform AddPoint(Transform parent, string name, Vector2 position)
    {
        Transform point = new GameObject(name).transform;
        point.position = position;
        point.SetParent(parent);
        return point;
    }

    private static T AddBuilding<T>(Transform parent, string name, Vector2 position, Vector2 size, Color color, BuildingChannel channel)
        where T : Building
    {
        GameObject go = BuildBlock(name, position, size, color, 0);
        go.transform.SetParent(parent);
        T building = go.AddComponent<T>();
        Wire(building, "buildingEntered", channel);
        AddWorldLabel(name.ToUpper(), position);
        return building;
    }

    // A colored rectangle (sprite) with a solid 3D box collider. The collider is deep on the Z axis,
    // so 3D physics bodies that live on the X-Y plane can never slip past it.
    private static GameObject BuildBlock(string name, Vector2 position, Vector2 scale, Color color, int sortingOrder)
    {
        var go = new GameObject(name);
        go.transform.position = position;
        go.transform.localScale = new Vector3(scale.x, scale.y, 1f);

        var spriteRenderer = go.AddComponent<SpriteRenderer>();
        spriteRenderer.color = color;
        spriteRenderer.sortingOrder = sortingOrder;
        go.AddComponent<Placeholder>();

        var box = go.AddComponent<BoxCollider>();
        box.size = new Vector3(1f, 1f, 4f);
        return go;
    }

    // A text that sits in the world (not on the screen), used for the names on the buildings.
    private static void AddWorldLabel(string text, Vector2 position)
    {
        var go = new GameObject(text + " Label", typeof(RectTransform));
        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingOrder = 5;

        var rect = (RectTransform)go.transform;
        rect.sizeDelta = new Vector2(400f, 100f);
        rect.localScale = new Vector3(0.006f, 0.006f, 1f);
        rect.position = new Vector3(position.x, position.y, 0f);

        var textObject = new GameObject("Text", typeof(RectTransform));
        textObject.transform.SetParent(go.transform, false);
        var label = textObject.AddComponent<Text>();
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        label.fontSize = 64;
        label.fontStyle = FontStyle.Bold;
        label.alignment = TextAnchor.MiddleCenter;
        label.horizontalOverflow = HorizontalWrapMode.Overflow;
        label.verticalOverflow = VerticalWrapMode.Overflow;
        label.color = new Color(0.1f, 0.1f, 0.12f);
        label.text = text;

        var textRect = (RectTransform)textObject.transform;
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
    }

    // A coin: a small solid sphere (so coins bounce off walls and each other)
    // and a bigger trigger sphere (so the player picks it up a little before touching it).
    private static GameObject BuildCoin()
    {
        var go = new GameObject("Coin");
        go.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);

        var spriteRenderer = go.AddComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(1f, 0.85f, 0.2f);
        spriteRenderer.sortingOrder = 3;
        WireBool(go.AddComponent<Placeholder>(), "round", true);

        var body = go.AddComponent<Rigidbody>();
        body.useGravity = false;
        body.mass = 0.2f;
        body.linearDamping = 2.5f;
        body.angularDamping = 0.5f;
        body.constraints = FlatConstraints;

        go.AddComponent<SphereCollider>().radius = 0.5f;
        var pickup = go.AddComponent<SphereCollider>();
        pickup.radius = 1.2f;
        pickup.isTrigger = true;

        go.AddComponent<Coin>();
        return go;
    }

    // A bill: a solid box (it bumps into walls and other bills) and a slightly bigger trigger box
    // (touching the player is detected with the trigger). Its size and color come from its BillType.
    private static GameObject BuildBill()
    {
        var go = new GameObject("Bill");
        go.AddComponent<SpriteRenderer>().sortingOrder = 3;
        go.AddComponent<Placeholder>();

        var body = go.AddComponent<Rigidbody>();
        body.useGravity = false;
        body.mass = 1f;
        body.linearDamping = 1.5f;
        body.angularDamping = 0f;
        body.constraints = FlatConstraints;

        go.AddComponent<BoxCollider>().size = Vector3.one;
        var touch = go.AddComponent<BoxCollider>();
        touch.size = new Vector3(1.3f, 1.3f, 1f);
        touch.isTrigger = true;

        go.AddComponent<Bill>();
        return go;
    }

    private static void AddAnimator(GameObject go, string sheetPath)
    {
        // Sprite names are d<row>_f<col>, so sorting by name gives: down, left, right, up.
        Sprite[] frames = AssetDatabase.LoadAllAssetsAtPath(sheetPath).OfType<Sprite>().OrderBy(sprite => sprite.name).ToArray();
        if (frames.Length < 16)
        {
            Debug.LogWarning("Character sheet missing or not sliced: " + sheetPath + ". The player is shown as a square.");
            return;
        }

        var animator = go.AddComponent<SpriteAnimator>();
        WireArray(animator, "frames", frames);
        go.GetComponent<SpriteRenderer>().sprite = frames[0];
    }

    private static void ImportSheet(string path)
    {
        AssetDatabase.ImportAsset(path);
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null)
        {
            Debug.LogWarning("Character sheet not found: " + path);
            return;
        }

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.spritePixelsPerUnit = 32;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;

        var rects = new List<SpriteMetaData>();
        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                rects.Add(new SpriteMetaData
                {
                    name = "d" + row + "_f" + col,
                    rect = new Rect(col * 32, 128 - (row + 1) * 32, 32, 32), // Unity counts Y from the bottom
                    pivot = new Vector2(0.5f, 0.5f),
                    alignment = (int)SpriteAlignment.Center
                });
            }
        }

        importer.spritesheet = rects.ToArray();
        importer.SaveAndReimport();
    }

    // ---------- assets ----------

    // Creates a bill type asset (if it does not exist yet) and returns its path.
    private static string CreateBillType(string fileName, string displayName, int amount, Color color, float size, float chaseForce)
    {
        string path = DataFolder + "/Bills/" + fileName + ".asset";
        if (AssetDatabase.LoadAssetAtPath<BillType>(path) != null)
        {
            return path; // keep the values that were tuned in the Inspector
        }

        var type = LoadOrCreate<BillType>(path);
        type.displayName = displayName;
        type.amount = amount;
        type.color = color;
        type.size = size;
        type.chaseForce = chaseForce;
        EditorUtility.SetDirty(type);
        return path;
    }

    private static T LoadOrCreate<T>(string path) where T : ScriptableObject
    {
        var asset = AssetDatabase.LoadAssetAtPath<T>(path);
        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
        }

        return asset;
    }

    private static T Load<T>(string path) where T : Object
    {
        T asset = AssetDatabase.LoadAssetAtPath<T>(path);
        if (asset == null)
        {
            throw new System.IO.FileNotFoundException("Asset not found: " + path);
        }

        return asset;
    }

    private static T[] LoadAll<T>(string[] paths) where T : Object
    {
        return paths.Select(path => Load<T>(path)).ToArray();
    }

    private static GameObject SavePrefab(GameObject go, string path)
    {
        GameObject asset = PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
        return asset;
    }

    private static void EnsureFolder(string parent, string name)
    {
        if (!AssetDatabase.IsValidFolder(parent + "/" + name))
        {
            AssetDatabase.CreateFolder(parent, name);
        }
    }

    private static UnityEngine.SceneManagement.Scene NewScene()
    {
        // DefaultGameObjects adds the Main Camera that SetupCamera needs.
        return EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
    }

    private static void SetupCamera(Color background)
    {
        var camera = Camera.main;
        camera.orthographic = true;
        camera.orthographicSize = 5.5f;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = background;
        camera.transform.position = new Vector3(0f, 0f, -10f);
    }

    private static string ScenePath(string name)
    {
        return ScenesFolder + "/" + name + ".unity";
    }

    // ---------- wiring: sets a [SerializeField] private field of a component, by name ----------

    private static SerializedProperty Find(SerializedObject serialized, string field)
    {
        SerializedProperty property = serialized.FindProperty(field);
        if (property == null)
        {
            throw new System.ArgumentException("No serialized field '" + field + "' on " + serialized.targetObject.GetType().Name);
        }

        return property;
    }

    private static void Wire(Object target, string field, Object value)
    {
        if (value == null)
        {
            throw new System.ArgumentException("Nothing to put in the field '" + field + "' of " + target.GetType().Name);
        }

        var serialized = new SerializedObject(target);
        Find(serialized, field).objectReferenceValue = value;
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void WireBool(Object target, string field, bool value)
    {
        var serialized = new SerializedObject(target);
        Find(serialized, field).boolValue = value;
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void WireArray(Object target, string field, Object[] values)
    {
        var serialized = new SerializedObject(target);
        SerializedProperty property = Find(serialized, field);
        property.arraySize = values.Length;
        for (int i = 0; i < values.Length; i++)
        {
            if (values[i] == null)
            {
                throw new System.ArgumentException("Empty item " + i + " for the field '" + field + "' of " + target.GetType().Name);
            }

            property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
        }

        serialized.ApplyModifiedPropertiesWithoutUndo();
    }
}
