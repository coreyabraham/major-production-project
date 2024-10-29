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
        public UnityEvent<PlayerSystem, CameraSystem> ExternalsCached;
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

    [System.Serializable]
    public struct SceneEntry
    {
        public string name;
        public int index;
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
    public SceneEntry[] SceneEntries;
    public string[] BlacklistedPauseScenes;

    [field: Header("Events")]
    public GameEvents Events;

    private readonly Dictionary<int, string> Levels = new();

    private string TargetSceneName;
    private float ElapsedPlaytime;

    private bool SceneRequested = false;

    private float SceneLoadingProgress = 0.0f;
    private float SceneLoadingModifier = 0.01f;

    private AsyncOperation SceneLoadingOperation;

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

    public bool IsTargetSceneAValidLevel(Scene Target)
    {
        if (Target.buildIndex < Levels.Count || Target.buildIndex > Levels.Count) return false;
        return Levels[Target.buildIndex] == Target.name;
    }

    public bool IsCurrentSceneAValidLevel() => IsTargetSceneAValidLevel(SceneManager.GetActiveScene());

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

        Events.ExternalsCached?.Invoke(Player, Camera);
    }

    public void RequestLoadScene(string SceneName)
    {
        if (string.IsNullOrWhiteSpace(SceneName))
        {
            Debug.LogWarning(name + " | Could not Load Scene with Name: " + SceneName + "\nMake sure the target Scene you're attempting to load is in the `Build Settings` 'Scenes in Build' list!");
            return;
        }

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
        StartCoroutine(TrackLoadingOperation());
        StartCoroutine(UnloadPreviouScene(SceneManager.GetActiveScene()));

        TargetSceneName = string.Empty;
        SceneRequested = false;
    }

    IEnumerator LoadSceneInBackground(string SceneName)
    {
        SceneLoadingOperation = SceneManager.LoadSceneAsync(SceneName);
        SceneLoadingOperation.allowSceneActivation = false;
        
        SceneLoadingProgress = 0.0f;

        while (SceneLoadingProgress <= 1.0f)
        {
            Events.LoadingProgress?.Invoke(SceneLoadingProgress);
            SceneLoadingProgress += SceneLoadingModifier;

            yield return new WaitForSeconds(SceneLoadingModifier);
        }

        Events.LoadingFinished?.Invoke();
    }

    IEnumerator TrackLoadingOperation()
    {
        while (!SceneLoadingOperation.isDone)
        {
            SceneLoadingOperation.allowSceneActivation = SceneLoadingProgress >= 1.0f;
            yield return null;
        }

        SceneLoadingOperation = null;
        SceneLoadingProgress = 0.0f;
    }

    IEnumerator UnloadPreviouScene(Scene Previous)
    {
        yield return SceneManager.UnloadSceneAsync(Previous);
    }

    private void SceneLoaded(Scene Scene, LoadSceneMode Mode) => Events.SceneLoaded?.Invoke(Scene, Mode);
    private void SceneUnloaded(Scene Scene) => Events.SceneUnloaded?.Invoke(Scene);
    private void ActiveSceneChanged(Scene Old, Scene New)
    {
        RefreshCachedExternals();
        Events.SceneChanged?.Invoke(Old, New);
    }

    private void LoadEvents()
    {
        Events.ExternalsCached ??= new();
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
        foreach (SceneEntry entry in SceneEntries) Levels.Add(entry.index, entry.name);

        LoadEvents();
        RefreshCachedExternals();

        SceneManager.activeSceneChanged += ActiveSceneChanged;
        SceneManager.sceneLoaded += SceneLoaded;
        SceneManager.sceneUnloaded += SceneUnloaded;

        Events.LoadingUIFinished.AddListener(LoadScene);
    }
}
