using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameSystem : Singleton<GameSystem>
{
    [System.Serializable]
    public class GameEvents
    {
        public UnityEvent PlayerDied;

        public UnityEvent RequestLoadingUI;
        [HideInInspector] public UnityEvent LoadingUIFinished;

        public UnityEvent LoadingStarted;
        public UnityEvent LoadingFinished;
        public UnityEvent<float> LoadingProgress;

        public UnityEvent<Scene> SceneUnloaded;
        public UnityEvent<Scene, LoadSceneMode> SceneLoaded;
        public UnityEvent<Scene, Scene> SceneChanged;
    }

    [field: Header("Tags")]
    public string PlayerTag = "Player";
    public string CameraTag = "Camera";

    [field: Header("Externals")]
    public PlayerSystem Player;
    public CameraSystem Camera;

    [field: Header("Miscellaneous")]
    public bool GameplayPaused = false;

    [field: Header("Collections")]
    [field: SerializeField] private List<string> LevelNames;
    private Dictionary<int, string> Levels;
    public string[] BlacklistedPauseScenes;

    [field: Header("Events")]
    public GameEvents Events;

    private string TargetSceneName;
    private float ElapsedPlaytime;

    private bool SceneRequested = false;

    public float GetElapsedPlaytime() => ElapsedPlaytime;
    public void SetPausedState(bool State) => GameplayPaused = State;

    public int GetCurrentSceneBuildIndex() => SceneManager.GetActiveScene().buildIndex;

    public string GetLevelName(int index)
    {
        bool result = Levels.TryGetValue(index, out string value);
        if (!result) value = string.Empty;
        return value;
    }

    public string GetLevelNameWithIndex() => GetLevelName(GetCurrentSceneBuildIndex());

    public bool IsCurrentSceneAValidLevel()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        if (Levels.Count < activeScene.buildIndex) return false;
        return Levels[activeScene.buildIndex] == activeScene.name;
    }

    public void PlayerDiedCallback()
    {
        // TODO: Reset all interactable / moveable objects the Player interacts with in the level they're currently in during the death transition!
        Events.PlayerDied?.Invoke();
    }

    // TODO: IMPROVE THIS SEARCHING MECHANISM!
    public void RefreshCachedExternals()
    {
        if (Player == null)
        {
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag(PlayerTag))
            {
                obj.TryGetComponent(out Player);
                if (Player != null) break;
            }
        }

        if (Camera == null)
        {
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag(CameraTag))
            {
                obj.TryGetComponent(out Camera);
                if (Camera != null) break;
            }
        }
    }

    public void RequestLoadScene(string SceneName)
    {
        if (SceneRequested) return;
        SceneRequested = true;

        TargetSceneName = SceneName;
        Events.RequestLoadingUI?.Invoke();
    }

    public void LoadScene()
    {
        if (string.IsNullOrWhiteSpace(TargetSceneName))
        {
            Debug.LogWarning(name + " | Could not Load Scene with Name: " + TargetSceneName + "\nMake sure the target Scene you're attempting to load is in the `Build Settings` 'Scenes in Build' list!");
            return;
        }

        Events.LoadingStarted?.Invoke();

        StartCoroutine(LoadSceneInBackground(TargetSceneName));
        StartCoroutine(UnloadSceneInBackground());

        TargetSceneName = string.Empty;
        SceneRequested = false;
    }

    // Could possibly be done in an Update loop instead? If so, then try adding Lerping to it!
    IEnumerator LoadSceneInBackground(string SceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneName);

        while (!operation.isDone)
        {
            Events.LoadingProgress?.Invoke(Mathf.Clamp01(operation.progress / 0.9f));
            yield return null;
        }

        Events.LoadingFinished?.Invoke();
    }

    IEnumerator UnloadSceneInBackground()
    {
        AsyncOperation operation = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        while (!operation.isDone) yield return null;
    }

    private void SceneLoaded(Scene Scene, LoadSceneMode Mode) => Events.SceneLoaded?.Invoke(Scene, Mode);
    private void SceneUnloaded(Scene Scene) => Events.SceneUnloaded?.Invoke(Scene);
    private void ActiveSceneChanged(Scene Old, Scene New)
    {
        RefreshCachedExternals();
        Events.SceneChanged?.Invoke(Old, New);
    }

    private void SetupLevelEntries()
    {
        for (int i = 0; i < LevelNames.Count; i++) Levels.Add(i, LevelNames[i]);
    }

    private void LoadEvents()
    {
        Events.PlayerDied ??= new();
        Events.RequestLoadingUI ??= new();
        Events.LoadingStarted ??= new();
        Events.LoadingProgress ??= new();
        Events.SceneUnloaded ??= new();
        Events.SceneLoaded ??= new();
        Events.SceneChanged ??= new();
    }

    private void Update()
    {
        if (GameplayPaused) return;
        ElapsedPlaytime += Time.deltaTime;
    }

    protected override void Initialize()
    {
        SetupLevelEntries();
        LoadEvents();
        RefreshCachedExternals();

        SceneManager.activeSceneChanged += ActiveSceneChanged;
        SceneManager.sceneLoaded += SceneLoaded;
        SceneManager.sceneUnloaded += SceneUnloaded;

        Events.LoadingUIFinished.AddListener(LoadScene);
    }
}
