using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Callbacks;


public class ModularKitPaletteWindow : EditorWindow
{
    private ModularKitDefinition _selectedKit;
    private ModularBuildInstance _activeBuild;
    private ModularBuildDefinition _selectedBuildDefinition;
    private ModularBuildLevel _activeBuildLevel;

    private string _selectedCategoryId;
    private string _selectedModuleId;

    private GameObject _ghostInstance;
    private string _ghostModuleId;
    private Quaternion _ghostBaseRotation;
    private bool _isPickHeightActive;
    private bool _hasCustomPlacementHeight;
    private float _customPlacementY;

    private float _currentRotationY;

    private Material _ghostPreviewMaterial;
    private Material _ghostBlockedMaterial;



    [MenuItem("Asset/Create/Modular Kit Builder")]
    private static void OpenWindow()
    {

        var window = GetWindow<ModularKitPaletteWindow>();

        window.titleContent = new GUIContent("Modular Kit Builder");

        window.Show();

    }


    private void OnGUI()
    {
        // =========================================================
        // NO ACTIVE BUILD
        // =========================================================

        if (_activeBuild == null)
        {
            EditorGUILayout.LabelField(
                "Modular Kit Builder",
                EditorStyles.boldLabel
            );

            EditorGUILayout.Space(8);


            ModularKitDefinition existingKit = _selectedKit;


            _selectedKit =
                (ModularKitDefinition)EditorGUILayout.ObjectField(
                    "Kit",
                    _selectedKit,
                    typeof(ModularKitDefinition),
                    false
                );


            if (existingKit != _selectedKit)
            {
                _selectedCategoryId = null;
                _selectedModuleId = null;

                ClearGhost();
            }


            if (_selectedKit != null)
            {
                EditorGUILayout.Space(4);

                EditorGUILayout.LabelField(
                    "Selected Kit",
                    _selectedKit.DisplayName
                );


                if (GUILayout.Button("Create New Build"))
                {
                    CreateBuild();
                }
            }


            EditorGUILayout.Space(12);

            EditorGUILayout.LabelField(
                "Continue Existing Build",
                EditorStyles.boldLabel
            );


            _selectedBuildDefinition =
                (ModularBuildDefinition)EditorGUILayout.ObjectField(
                    "Existing Build",
                    _selectedBuildDefinition,
                    typeof(ModularBuildDefinition),
                    false
                );


            if (_selectedBuildDefinition != null)
            {
                if (GUILayout.Button("Continue Build"))
                {
                    ContinueBuild(_selectedBuildDefinition);
                }
            }


            return;
        }


        // =========================================================
        // ACTIVE BUILD VALIDATION
        // =========================================================

        if (_selectedKit == null)
        {
            return;
        }


        if (_activeBuildLevel == null)
        {
            _activeBuildLevel =
                _activeBuild.GetComponent<ModularBuildLevel>();
        }


        if (_activeBuildLevel == null)
        {
            EditorGUILayout.HelpBox(
                "Active Build does not contain a ModularBuildLevel.",
                MessageType.Warning
            );

            return;
        }


        // =========================================================
        // BUILD INFO
        // =========================================================

        EditorGUILayout.LabelField(
            "BUILD",
            EditorStyles.boldLabel
        );


        EditorGUILayout.LabelField(
            "Kit",
            _selectedKit.DisplayName
        );


        EditorGUILayout.LabelField(
            "Active Build",
            _activeBuild.gameObject.name
        );


        // =========================================================
        // FLOORS
        // =========================================================

        EditorGUILayout.Space(12);


        EditorGUILayout.LabelField(
            "FLOORS",
            EditorStyles.boldLabel
        );


        if (_activeBuildLevel.ActiveLevel != null)
        {
            EditorGUILayout.LabelField(
                "Active Floor",
                _activeBuildLevel.ActiveLevel.name
            );
        }


        EditorGUILayout.LabelField(
            "Placement Height",
            _hasCustomPlacementHeight
                ? "Custom Y: " + _customPlacementY.ToString("0.###")
                : "Active Floor"
        );


        EditorGUILayout.HelpBox(
            "Hold V and click a vertex to pick its height.",
            MessageType.Info
        );


        if (_hasCustomPlacementHeight)
        {
            if (GUILayout.Button("Use Active Floor Height"))
            {
                _hasCustomPlacementHeight = false;

                SceneView.RepaintAll();

                Repaint();
            }
        }


        EditorGUILayout.Space(4);


        float floorButtonWidth = 75f;

        int floorColumns =
            Mathf.FloorToInt(position.width / floorButtonWidth);

        floorColumns = Mathf.Max(1, floorColumns);


        int currentFloorColumn = 0;


        for (int i = 0;
             i < _activeBuildLevel.Levels.Count;
             i++)
        {
            if (currentFloorColumn == 0)
            {
                GUILayout.BeginHorizontal();
            }


            Transform level =
                _activeBuildLevel.Levels[i];


            bool isActive =
                i == _activeBuildLevel.ActiveLevelIndex;


            string floorName =
                "Floor " + i.ToString("00");


            bool isLevelSelected =
                GUILayout.Toggle(
                    isActive,
                    floorName,
                    GUI.skin.button,
                    GUILayout.Width(floorButtonWidth)
                );


            if (isLevelSelected && !isActive)
            {
                _activeBuildLevel.SetActiveLevel(i);

                _hasCustomPlacementHeight = false;

                ClearGhost();

                SceneView.RepaintAll();

                Repaint();
            }


            currentFloorColumn++;


            if (currentFloorColumn >= floorColumns)
            {
                currentFloorColumn = 0;

                GUILayout.EndHorizontal();
            }
        }


        if (currentFloorColumn != 0)
        {
            GUILayout.EndHorizontal();
        }


        EditorGUILayout.Space(4);


        if (GUILayout.Button("+ Add Floor"))
        {
            _activeBuildLevel.AddLevel();

            _hasCustomPlacementHeight = false;

            ClearGhost();

            SceneView.RepaintAll();

            Repaint();
        }


        // =========================================================
        // MODULE CATEGORIES
        // =========================================================

        EditorGUILayout.Space(12);


        EditorGUILayout.LabelField(
            "MODULE CATEGORIES",
            EditorStyles.boldLabel
        );


        string existingCategory =
            _selectedCategoryId;


        foreach (CategoryDefinition category
                 in _selectedKit.Categories)
        {
            bool isCategorySelected =
                _selectedCategoryId ==
                category.CategoryId;


            bool newCategoryState =
                GUILayout.Toggle(
                    isCategorySelected,
                    category.DisplayName,
                    GUI.skin.button
                );


            if (newCategoryState)
            {
                _selectedCategoryId =
                    category.CategoryId;
            }


            if (!newCategoryState &&
                isCategorySelected)
            {
                _selectedCategoryId = null;
            }
        }


        if (existingCategory != _selectedCategoryId)
        {
            _selectedModuleId = null;

            ClearGhost();
        }


        // =========================================================
        // MODULES
        // =========================================================

        EditorGUILayout.Space(12);


        EditorGUILayout.LabelField(
            "MODULES",
            EditorStyles.boldLabel
        );


        if (string.IsNullOrWhiteSpace(
            _selectedCategoryId))
        {
            EditorGUILayout.HelpBox(
                "Select a module category.",
                MessageType.Info
            );

            return;
        }


        int currentColumn = 0;

        float tileWidth = 110f;


        int columns =
            Mathf.FloorToInt(
                position.width / tileWidth
            );


        columns =
            Mathf.Max(1, columns);


        bool hasModules = false;


        foreach (ModuleDefinition module
                 in _selectedKit.Modules)
        {
            if (module.CategoryId !=
                _selectedCategoryId)
            {
                continue;
            }


            hasModules = true;


            if (currentColumn == 0)
            {
                GUILayout.BeginHorizontal();
            }


            GUILayout.BeginVertical(
                GUILayout.Width(tileWidth)
            );


            Texture2D preview =
                AssetPreview.GetAssetPreview(
                    module.Prefab
                );


            if (preview != null)
            {
                GUILayout.Label(
                    preview,
                    GUILayout.Width(tileWidth),
                    GUILayout.Height(tileWidth)
                );
            }
            else
            {
                GUILayout.Space(tileWidth);
            }


            bool isModuleSelected =
                _selectedModuleId ==
                module.ModuleId;


            bool newToggleState =
                GUILayout.Toggle(
                    isModuleSelected,
                    module.DisplayName,
                    GUI.skin.button
                );


            if (newToggleState)
            {
                if (_selectedModuleId !=
                    module.ModuleId)
                {
                    _selectedModuleId =
                        module.ModuleId;

                    ClearGhost();
                }
            }


            if (!newToggleState &&
                isModuleSelected)
            {
                _selectedModuleId = null;

                ClearGhost();
            }


            GUILayout.EndVertical();


            currentColumn++;


            if (currentColumn >= columns)
            {
                currentColumn = 0;

                GUILayout.EndHorizontal();
            }
        }


        if (currentColumn != 0)
        {
            GUILayout.EndHorizontal();
        }


        if (!hasModules)
        {
            EditorGUILayout.HelpBox(
                "No modules found in this category.",
                MessageType.Info
            );
        }
    }


    private void OnSceneGUI(SceneView sceneView)
    {

        if (_activeBuild == null)
        {

            return;

        }


        if (_selectedKit == null)
        {

            return;

        }


        if (string.IsNullOrWhiteSpace(_selectedModuleId))
        {

            return;

        }


        ModuleDefinition selectedModule = FindModuleById(_selectedModuleId);


        if (selectedModule == null)
        {

            return;

        }


        GameObject prefab = selectedModule.Prefab;


        if (prefab == null)
        {

            return;

        }


        Event currentEvent = Event.current;


        if (currentEvent.type == EventType.KeyDown &&
            currentEvent.keyCode == KeyCode.Escape)
        {
            CancelPlacement();

            currentEvent.Use();

            return;
        }


        if (currentEvent.type == EventType.KeyDown &&
            currentEvent.keyCode == KeyCode.V)
        {

            _isPickHeightActive = true;

            Debug.Log("Pick Height ON");

            currentEvent.Use();

            SceneView.RepaintAll();

        }
        else if (currentEvent.type == EventType.KeyUp &&
                 currentEvent.keyCode == KeyCode.V)
        {

            _isPickHeightActive = false;

            Debug.Log("Pick Height OFF");

            currentEvent.Use();

            SceneView.RepaintAll();

        }


        Transform activeLevel = _activeBuildLevel.ActiveLevel;


        if (activeLevel == null)
        {

            return;

        }


        float placementY =
            _hasCustomPlacementHeight
                ? _customPlacementY
                : activeLevel.position.y;


        bool hasHeightCandidate = false;

        Vector3 heightCandidate = Vector3.zero;


        if (_isPickHeightActive)
        {

            Transform[] snapTargets =
                _activeBuild.GetComponentsInChildren<Transform>();


            if (HandleUtility.FindNearestVertex(
                    currentEvent.mousePosition,
                    snapTargets,
                    out Vector3 nearestVertex))
            {

                hasHeightCandidate = true;

                heightCandidate = nearestVertex;

                placementY = nearestVertex.y;


                Color previousHandlesColor = Handles.color;

                Handles.color = Color.cyan;


                Handles.SphereHandleCap(
                    0,
                    nearestVertex,
                    Quaternion.identity,
                    HandleUtility.GetHandleSize(nearestVertex) * 0.08f,
                    EventType.Repaint
                );


                Handles.color = previousHandlesColor;

            }


            if (currentEvent.type == EventType.MouseDown &&
                currentEvent.button == 0 &&
                !currentEvent.alt)
            {

                if (hasHeightCandidate)
                {

                    _customPlacementY = heightCandidate.y;

                    _hasCustomPlacementHeight = true;

                    Debug.Log(
                        "Picked placement height: " +
                        _customPlacementY
                    );

                    Repaint();

                    SceneView.RepaintAll();

                }


                currentEvent.Use();

                return;

            }

        }


        Ray mouseRay =
            HandleUtility.GUIPointToWorldRay(
                currentEvent.mousePosition
            );


        Plane plane =
            new Plane(
                Vector3.up,
                new Vector3(0f, placementY, 0f)
            );


        if (!plane.Raycast(mouseRay, out float distance))
        {

            return;

        }


        Vector3 worldPosition =
            mouseRay.GetPoint(distance);


        float snappedX =
            _selectedKit.GridCellSize.x *
            Mathf.Round(
                worldPosition.x /
                _selectedKit.GridCellSize.x
            );


        float snappedZ =
            _selectedKit.GridCellSize.y *
            Mathf.Round(
                worldPosition.z /
                _selectedKit.GridCellSize.y
            );


        Vector3 snappedPosition =
            new Vector3(
                snappedX,
                placementY,
                snappedZ
            );




        if (_selectedModuleId != _ghostModuleId)
        {

            ClearGhost();

        }

        if (_ghostInstance == null)
        {
            _ghostInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

            _ghostBaseRotation = _ghostInstance.transform.rotation;

            _ghostInstance.name = prefab.name + "_GHOST";

            _ghostModuleId = selectedModule.ModuleId;
        }


        if (currentEvent.type == EventType.KeyDown &&
            currentEvent.keyCode == KeyCode.R)
        {

            _currentRotationY += _selectedKit.RotationSnap;

            _currentRotationY %= 360f;

            currentEvent.Use();

        }


        _ghostInstance.transform.position = snappedPosition;

        Quaternion snappedRotation = Quaternion.Euler(0f, _currentRotationY, 0f);

        _ghostInstance.transform.rotation = snappedRotation * _ghostBaseRotation;



        PlacedModuleInstance overlappingModule =
            FindOverlappingModule(
                snappedPosition,
                selectedModule.Footprint,
                _currentRotationY,
                activeLevel,
                selectedModule.PlacementTypeId
            );


        bool isBlocked = overlappingModule != null;


        if (isBlocked)
        {

            ApplyGhostAppearance(
                _ghostInstance,
                _ghostBlockedMaterial
            );

        }

        else
        {

            ApplyGhostAppearance(
                _ghostInstance,
                _ghostPreviewMaterial
            );

        }


        if (currentEvent.type == EventType.MouseDown &&
            currentEvent.button == 0 &&
            !currentEvent.alt &&
            !isBlocked)
        {

            GameObject placedObject =
                (GameObject)PrefabUtility.InstantiatePrefab(prefab);


            placedObject.transform.SetParent(activeLevel);



            placedObject.transform.position =
                snappedPosition;


            placedObject.transform.rotation =
                _ghostInstance.transform.rotation;


            PlacedModuleInstance placedModule =
                placedObject.AddComponent<PlacedModuleInstance>();


            placedModule.Initialize(
                _selectedModuleId,
                _selectedKit.KitId,
                selectedModule.Footprint,
                selectedModule.PlacementTypeId
            );


            currentEvent.Use();

        }

    }


    private void OnEnable()
    {

        SceneView.duringSceneGui += OnSceneGUI;


        _ghostBlockedMaterial =
            FindMaterialByName("M_GhostBlock");


        _ghostPreviewMaterial =
            FindMaterialByName("M_GhostPreview");

    }


    private void OnDisable()
    {

        SceneView.duringSceneGui -= OnSceneGUI;

        ClearGhost();

    }


    private void ClearGhost()
    {
        if (_ghostInstance != null)
        {
            Transform selectedTransform = Selection.activeTransform;

            bool ghostIsSelected =
                selectedTransform != null &&
                (selectedTransform == _ghostInstance.transform ||
                 selectedTransform.IsChildOf(_ghostInstance.transform));

            if (ghostIsSelected)
            {
                Selection.activeObject =
                    _activeBuild != null
                        ? _activeBuild.gameObject
                        : null;
            }

            DestroyImmediate(_ghostInstance);
        }

        _ghostInstance = null;
        _ghostModuleId = null;
    }


    private ModuleDefinition FindModuleById(string moduleId)
    {

        if (_selectedKit == null)
        {

            return null;

        }


        foreach (ModuleDefinition module in _selectedKit.Modules)
        {

            if (module.ModuleId == moduleId)
            {

                return module;

            }

        }


        return null;

    }


    private void ApplyGhostAppearance(
        GameObject ghost,
        Material ghostMaterial)
    {

        if (ghost == null)
        {

            return;

        }


        if (ghostMaterial == null)
        {

            return;

        }


        Renderer[] renderers =
            ghost.GetComponentsInChildren<Renderer>();


        foreach (Renderer renderer in renderers)
        {

            Material[] materials =
                renderer.sharedMaterials;


            for (int i = 0; i < materials.Length; i++)
            {

                materials[i] = ghostMaterial;

            }


            renderer.sharedMaterials = materials;

        }

    }


    private Material FindMaterialByName(string materialName)
    {

        string[] guids =
            AssetDatabase.FindAssets(materialName);


        if (guids.Length == 0)
        {

            return null;

        }


        string materialGuid = guids[0];


        string materialPath =
            AssetDatabase.GUIDToAssetPath(materialGuid);


        Material ghostMaterial =
            AssetDatabase.LoadAssetAtPath<Material>(materialPath);


        return ghostMaterial;

    }



    private HashSet<Vector2Int> GetOccupiedCells(Vector3 position, Vector2Int footprint, float rotationY)
    {

        HashSet<Vector2Int> occupiedCells = new HashSet<Vector2Int>();

        int originX = Mathf.RoundToInt(position.x / _selectedKit.GridCellSize.x);
        int originZ = Mathf.RoundToInt(position.z / _selectedKit.GridCellSize.y);

        int quarterTurns = Mathf.RoundToInt(rotationY / 90f) % 4;

        if (quarterTurns < 0)
        {

            quarterTurns += 4;

        }


        for (int x = 0; x < footprint.x; x++)
        {

            for (int z = 0; z < footprint.y; z++)
            {

                Vector2Int cellOffset;


                switch (quarterTurns)
                {

                    case 0:

                        cellOffset = new Vector2Int(x, z);

                        break;


                    case 1:

                        cellOffset = new Vector2Int(z, -x - 1);

                        break;


                    case 2:

                        cellOffset = new Vector2Int(-x - 1, -z - 1);

                        break;


                    case 3:

                        cellOffset = new Vector2Int(-z - 1, x);

                        break;


                    default:

                        cellOffset = Vector2Int.zero;

                        break;

                }


                Vector2Int worldCell = new Vector2Int(
                    originX + cellOffset.x,
                    originZ + cellOffset.y
                );


                occupiedCells.Add(worldCell);

            }

        }


        return occupiedCells;

    }
    private PlacedModuleInstance FindOverlappingModule(Vector3 position, Vector2Int footprint, float rotationY, Transform activeLevel, string placementTypeId)
    {

        if (_activeBuild == null)
        {

            return null;

        }


        HashSet<Vector2Int> newModuleCells =
            GetOccupiedCells(position, footprint, rotationY);


        PlacedModuleInstance[] placedModules =
            activeLevel.GetComponentsInChildren<PlacedModuleInstance>();


        foreach (PlacedModuleInstance placedModule in placedModules)
        {
            if (placedModule.PlacementTypeId != placementTypeId)
            {
                continue;
            }


            HashSet<Vector2Int> placedModuleCells =
                GetOccupiedCells(
                    placedModule.transform.position,
                    placedModule.Footprint,
                    placedModule.transform.eulerAngles.y
                );





            foreach (Vector2Int cell in newModuleCells)
            {

                if (placedModuleCells.Contains(cell))
                {

                    return placedModule;

                }

            }

        }


        return null;

    }


    private void CreateBuild()
    {

        if (_selectedKit == null)
        {

            return;

        }


        string folderPath =
            "Assets/ModularBuilds";


        if (!AssetDatabase.IsValidFolder(folderPath))
        {

            AssetDatabase.CreateFolder(
                "Assets",
                "ModularBuilds"
            );

        }


        string buildId =
            Guid.NewGuid().ToString("N");


        string displayName =
            _selectedKit.DisplayName + "_Build";


        ModularBuildDefinition buildDefinition =
            ScriptableObject.CreateInstance<ModularBuildDefinition>();


        buildDefinition.Initialize(
            buildId,
            displayName,
            _selectedKit
        );


        string assetPath =
            AssetDatabase.GenerateUniqueAssetPath(
                folderPath + "/" + displayName + ".asset"
            );


        AssetDatabase.CreateAsset(
            buildDefinition,
            assetPath
        );


        AssetDatabase.SaveAssets();


        GameObject buildRoot =
            new GameObject(displayName);


        ModularBuildInstance buildInstance =
            buildRoot.AddComponent<ModularBuildInstance>();


        ModularBuildLevel buildLevel = buildRoot.AddComponent<ModularBuildLevel>();

        buildInstance.Initialize(buildDefinition);
        buildLevel.Initialize(_selectedKit);

        _activeBuild = buildInstance;
        _activeBuildLevel = buildLevel;

        _selectedBuildDefinition = buildDefinition;

        Selection.activeObject = buildDefinition;


        buildInstance.Initialize(buildDefinition);


        _activeBuild = buildInstance;

        _selectedBuildDefinition = buildDefinition;


        Selection.activeObject = buildDefinition;

    }


    private ModularBuildInstance FindBuildInstance(
        ModularBuildDefinition definition)
    {

        if (definition == null)
        {

            return null;

        }


        ModularBuildInstance[] builds =
            FindObjectsByType<ModularBuildInstance>(
                FindObjectsSortMode.None
            );


        foreach (ModularBuildInstance build in builds)
        {

            if (build.Definition == definition)
            {

                return build;

            }

        }


        return null;

    }


    private void ContinueBuild(
        ModularBuildDefinition definition)
    {

        if (definition == null)
        {

            return;

        }


        ModularBuildInstance buildInstance =
            FindBuildInstance(definition);


        if (buildInstance == null)
        {

            Debug.LogWarning(
                "The selected build does not exist in the current scene."
            );

            return;

        }


        ClearGhost();


        _selectedBuildDefinition = definition;

        _selectedKit = definition.Kit;

        _activeBuild = buildInstance;


        _activeBuildLevel = buildInstance.GetComponent<ModularBuildLevel>();

        _selectedCategoryId = null;

        _selectedModuleId = null;

        _currentRotationY = 0f;

        _isPickHeightActive = false;

        _hasCustomPlacementHeight = false;

    }

    public static void OpenBuild(ModularBuildDefinition buildDefinition)
    {


        ModularKitPaletteWindow window = GetWindow<ModularKitPaletteWindow>();

        window.titleContent = new GUIContent("Modular Kit Builder");

        window.Show();
        window.Focus();

        window.ContinueBuild(buildDefinition);

    }


    [OnOpenAsset]
    private static bool OnOpenAsset(EntityId entityId, int line)
    {

        UnityEngine.Object asset = EditorUtility.EntityIdToObject(entityId);


        string assetPath = AssetDatabase.GetAssetPath(asset);

        ModularBuildDefinition buildDefinition = AssetDatabase.LoadAssetAtPath<ModularBuildDefinition>(assetPath);


        if (buildDefinition == null)
        {


            return false;



        }

        OpenBuild(buildDefinition);

        return true;


    }

    private void CancelPlacement()
    {
        _selectedModuleId = null;
        _currentRotationY = 0f;
        _isPickHeightActive = false;

        ClearGhost();

        Repaint();
        SceneView.RepaintAll();
    }

}