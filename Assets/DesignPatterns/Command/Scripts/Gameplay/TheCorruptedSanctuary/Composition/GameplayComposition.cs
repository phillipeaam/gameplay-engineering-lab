using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DesignPatterns.Command.Scripts.Gameplay.TheCorruptedSanctuary.Application.Execution;
using DesignPatterns.Command.Scripts.Gameplay.TheCorruptedSanctuary.Domain.Model;
using DesignPatterns.Command.Scripts.Gameplay.TheCorruptedSanctuary.Presentation;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace DesignPatterns.Command.Scripts.Gameplay.TheCorruptedSanctuary.Composition
{
    public sealed class GameplayComposition : MonoBehaviour
    {
        [Header("Optional authored assets")]
        [SerializeField] private GameObject _heroPrefab;
        [SerializeField] private GameObject _guardianPrefab;
        [SerializeField] private GameObject _anchorPrefab;
        [SerializeField] private PlayableDirector _introDirector;
        [SerializeField] private PlayableDirector _finaleDirector;

        [Header("Board")]
        [SerializeField, Min(0.5f)] private float _cellSize = 1.5f;
        [SerializeField] private Vector3 _gridOrigin = new(-4.5f, 0f, -4.5f);
        [SerializeField] private string _initialSeed = "FOREST-1042";

        [Header("Colors")]
        [SerializeField] private Color _gridColor = new(0.08f, 0.18f, 0.16f);
        [SerializeField] private Color _dangerOne = new(1f, 0.15f, 0.1f, 0.65f);
        [SerializeField] private Color _dangerTwo = new(0.8f, 0.1f, 0.5f, 0.45f);
        [SerializeField] private Color _dangerThree = new(0.55f, 0.15f, 0.8f, 0.3f);

        private readonly List<PlayerCommandData> _pendingCommands = new(TurnPlan.CommandCount);
        private readonly List<GameObject> _telegraphs = new();
        private readonly List<GameObject> _routeMarkers = new();
        private readonly Dictionary<int, AnchorView> _anchorViews = new();
        private readonly Dictionary<Vector2Int, GameObject> _cellViews = new();

        private TurnSimulator _simulator;
        private PatternDirector _patternDirector;
        private EncounterState _state;
        private GuardianPattern _currentPattern;
        private TurnResult _preview;
        private TurnRunner _turnRunner;
        private GameplayActorView _playerView;
        private GuardianView _guardianView;
        private EncounterPhase _phase;
        private PlayerCommandType _selectedCommand = PlayerCommandType.Step;
        private CancellationTokenSource _executionCancellation;
        private string _message = string.Empty;
        private string _seed;
        private Vector3 _guardianInitialScale;
        private CinemachineCamera[] _runtimeIntroCameras;
        private CinemachineCamera[] _runtimeFinaleCameras;
        private CinemachineCamera _runtimeGameplayCamera;

        public EncounterPhase Phase => _phase;
        public EncounterState State => _state;
        public TurnResult Preview => _preview;
        public IReadOnlyList<PlayerCommandData> PendingCommands => _pendingCommands;

        private void Awake()
        {
            _simulator = new TurnSimulator();
            EnsureCameraAndLight();
            BuildBoardVisuals();
            BuildActors();
            _turnRunner = gameObject.AddComponent<TurnRunner>();
            _turnRunner.Configure(_playerView, _guardianView);
            _turnRunner.BeatCompleted += PresentBeat;
            StartRun(string.IsNullOrWhiteSpace(_initialSeed) ? CreateSeed() : _initialSeed);
        }

        private void OnDestroy()
        {
            if (_turnRunner != null)
            {
                _turnRunner.BeatCompleted -= PresentBeat;
            }

            CancelExecution();
        }

        private void Update()
        {
            if (_phase != EncounterPhase.Planning || Keyboard.current == null)
            {
                return;
            }

            var keyboard = Keyboard.current;
            if (keyboard.digit1Key.wasPressedThisFrame) _selectedCommand = PlayerCommandType.Step;
            if (keyboard.digit2Key.wasPressedThisFrame) _selectedCommand = PlayerCommandType.Dash;
            if (keyboard.digit3Key.wasPressedThisFrame) AddCommand(PlayerCommandData.Guard());
            if (keyboard.digit4Key.wasPressedThisFrame) AddCommand(PlayerCommandData.Wait());

            if (keyboard.wKey.wasPressedThisFrame || keyboard.upArrowKey.wasPressedThisFrame)
                AddDirectionalCommand(Vector2Int.up);
            if (keyboard.sKey.wasPressedThisFrame || keyboard.downArrowKey.wasPressedThisFrame)
                AddDirectionalCommand(Vector2Int.down);
            if (keyboard.aKey.wasPressedThisFrame || keyboard.leftArrowKey.wasPressedThisFrame)
                AddDirectionalCommand(Vector2Int.left);
            if (keyboard.dKey.wasPressedThisFrame || keyboard.rightArrowKey.wasPressedThisFrame)
                AddDirectionalCommand(Vector2Int.right);

            if (keyboard.backspaceKey.wasPressedThisFrame) RemoveLastCommand();
            if (keyboard.deleteKey.wasPressedThisFrame) ClearCommands();
            if (keyboard.enterKey.wasPressedThisFrame || keyboard.spaceKey.wasPressedThisFrame) ConfirmPlan();
        }

        public void StartRun(string seed)
        {
            CancelExecution();
            ClearCommands();
            _seed = string.IsNullOrWhiteSpace(seed) ? CreateSeed() : seed;

            var blockedCells = SelectBlockedCells(_seed);
            var anchors = SelectAnchors(_seed, blockedCells);
            _state = new EncounterState(
                new GridState(7, 7, blockedCells),
                new PlayerState(new Vector2Int(4, 1)),
                anchors,
                _seed);

            _patternDirector = new PatternDirector(_seed, CreatePatternTemplates());
            _guardianView.transform.localScale = _guardianInitialScale;
            _guardianView.gameObject.SetActive(true);
            _playerView.Configure(_gridOrigin, _cellSize, _state.Player.Position);
            BuildAnchorVisuals();
            PresentState();
            EnsureRuntimeCinematics();
            if (_introDirector != null && _introDirector.playableAsset != null)
            {
                _ = PlayIntroAsync();
            }
            else
            {
                RevealNextPattern();
            }
        }

        private async Awaitable PlayIntroAsync()
        {
            _phase = EncounterPhase.Intro;
            _message = "The sanctuary remembers...";
            _guardianView.PresentIntroReveal();
            if (_runtimeIntroCameras != null)
            {
                SetCameraPriority(_runtimeIntroCameras, 0);
                var introCamera = _runtimeIntroCameras[0];
                var heroFocus = GetActorFocus(_playerView.transform);
                var guardianFocus = GetActorFocus(_guardianView.transform);
                var confrontationFocus = Vector3.Lerp(heroFocus, guardianFocus, 0.5f);
                var widePosition = confrontationFocus + new Vector3(10f, 9f, -13f);
                var heroPosition = heroFocus + new Vector3(5f, 2.4f, 4.5f);
                var guardianPosition = guardianFocus + new Vector3(6f, 2.8f, -5f);
                var confrontationPosition = confrontationFocus + new Vector3(9f, 6.5f, -11f);

                introCamera.transform.SetPositionAndRotation(
                    widePosition,
                    Quaternion.LookRotation(confrontationFocus + Vector3.up * 0.5f - widePosition, Vector3.up));
                introCamera.Lens.FieldOfView = 50f;
                await Awaitable.WaitForSecondsAsync(1.8f);
                _playerView.PresentIntroReady();
                await MoveCinematicCameraAsync(
                    introCamera,
                    heroPosition,
                    heroFocus + Vector3.up * 0.8f,
                    45f,
                    2.8f);
                await Awaitable.WaitForSecondsAsync(1.6f);
                _guardianView.PresentIntroReveal();
                await MoveCinematicCameraAsync(
                    introCamera,
                    guardianPosition,
                    guardianFocus + Vector3.up * 0.7f,
                    44f,
                    3f);
                await Awaitable.WaitForSecondsAsync(1.8f);
                await MoveCinematicCameraAsync(
                    introCamera,
                    confrontationPosition,
                    confrontationFocus + Vector3.up * 0.7f,
                    48f,
                    2.8f);
                await Awaitable.WaitForSecondsAsync(1.8f);
            }
            if (_runtimeGameplayCamera != null)
            {
                // Retire every intro camera before handing control back to gameplay.
                SetAllCameraPriorities(_runtimeIntroCameras, 0);
                _runtimeGameplayCamera.Priority.Value = 100;
                await Awaitable.WaitForSecondsAsync(1.5f);
            }
            else
            {
                _introDirector.Play();
                while (_introDirector.state == PlayState.Playing)
                {
                    await Awaitable.NextFrameAsync();
                }
            }

            if (_state != null && _phase == EncounterPhase.Intro)
            {
                RevealNextPattern();
            }
        }

        private void EnsureRuntimeCinematics()
        {
            if (_introDirector != null && _finaleDirector != null)
            {
                return;
            }

            var root = new GameObject("Runtime Cinematics").transform;
            root.SetParent(transform);
            var focus = new GameObject("Cinematic Focus").transform;
            focus.SetParent(root);
            focus.position = _gridOrigin + new Vector3(_cellSize * 3f, 1f, _cellSize * 3f);

            var brain = Camera.main != null ? Camera.main.GetComponent<CinemachineBrain>() : null;
            if (brain == null && Camera.main != null)
            {
                brain = Camera.main.gameObject.AddComponent<CinemachineBrain>();
            }

            var introCameras = new[]
            {
                CreateRuntimeCinematicCamera(root, "Intro Wide", new Vector3(12f, 10f, -14f), focus),
                CreateRuntimeCinematicCamera(root, "Intro Anchors", new Vector3(-10f, 6f, -8f), focus),
                CreateRuntimeCinematicCamera(root, "Intro Guardian", new Vector3(8f, 4.5f, 5f), focus),
                CreateRuntimeCinematicCamera(root, "Intro Hero", new Vector3(-7f, 3.5f, -5f), focus)
            };
            _runtimeIntroCameras = introCameras;
            var finaleCameras = new[]
            {
                CreateRuntimeCinematicCamera(root, "Finale Exposed", new Vector3(8f, 4.5f, 5f), focus),
                CreateRuntimeCinematicCamera(root, "Finale Approach", new Vector3(-6f, 3.2f, -4f), focus),
                CreateRuntimeCinematicCamera(root, "Finale Impact", new Vector3(7f, 5f, -7f), focus),
                CreateRuntimeCinematicCamera(root, "Finale Release", new Vector3(12f, 10f, -14f), focus)
            };
            _runtimeFinaleCameras = finaleCameras;

            _introDirector = CreateRuntimeDirector(root, "Intro Director", CreateRuntimeTimeline(introCameras, brain, new[] { 2d, 1.5d, 2d, 1.5d }));
            _finaleDirector = CreateRuntimeDirector(root, "Finale Director", CreateRuntimeTimeline(finaleCameras, brain, new[] { 1d, 1.5d, 0.6d, 2d }));

            _runtimeGameplayCamera = GetComponent<GameplayCameraRig>()?.GameplayCamera;
            SetAllCameraPriorities(introCameras, 0);
        }

        private static void SetAllCameraPriorities(CinemachineCamera[] cameras, int priority)
        {
            foreach (var camera in cameras)
            {
                camera.Priority.Value = priority;
            }
        }

        private static void SetCameraPriority(CinemachineCamera[] cameras, int activeIndex)
        {
            for (var i = 0; i < cameras.Length; i++)
            {
                cameras[i].Priority.Value = i == activeIndex ? 100 : 0;
            }
        }

        private static Vector3 GetActorFocus(Transform actor)
        {
            var renderers = actor.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0)
            {
                return actor.position + Vector3.up;
            }

            var bounds = renderers[0].bounds;
            for (var i = 1; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }

            return bounds.center;
        }

        private async Awaitable MoveCinematicCameraAsync(
            CinemachineCamera camera,
            Vector3 destination,
            Vector3 lookAt,
            float fieldOfView,
            float duration)
        {
            var startPosition = camera.transform.position;
            var startRotation = camera.transform.rotation;
            var targetRotation = Quaternion.LookRotation(lookAt - destination, Vector3.up);
            var startFieldOfView = camera.Lens.FieldOfView;
            var elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                var eased = t * t * (3f - 2f * t);
                camera.transform.position = Vector3.Lerp(startPosition, destination, eased);
                camera.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, eased);
                camera.Lens.FieldOfView = Mathf.Lerp(startFieldOfView, fieldOfView, eased);
                await Awaitable.NextFrameAsync();
            }

            camera.transform.SetPositionAndRotation(destination, targetRotation);
            camera.Lens.FieldOfView = fieldOfView;
        }

        private static CinemachineCamera CreateRuntimeCinematicCamera(Transform parent, string name, Vector3 position, Transform target)
        {
            var cameraObject = new GameObject(name);
            cameraObject.transform.SetParent(parent);
            cameraObject.transform.position = position;
            cameraObject.transform.LookAt(target.position);
            var camera = cameraObject.AddComponent<CinemachineCamera>();
            camera.Lens.FieldOfView = 42f;
            camera.Target = new CameraTarget { TrackingTarget = target, LookAtTarget = target };
            return camera;
        }

        private static PlayableDirector CreateRuntimeDirector(Transform parent, string name, TimelineAsset timeline)
        {
            var directorObject = new GameObject(name);
            directorObject.transform.SetParent(parent);
            var director = directorObject.AddComponent<PlayableDirector>();
            director.playableAsset = timeline;
            director.playOnAwake = false;
            return director;
        }

        private static TimelineAsset CreateRuntimeTimeline(CinemachineCamera[] cameras, CinemachineBrain brain, double[] durations)
        {
            var timeline = ScriptableObject.CreateInstance<TimelineAsset>();
            var track = timeline.CreateTrack<CinemachineTrack>(null, "Cinematic Shots");
            track.TrackPriority = 100;
            double start = 0d;
            for (var i = 0; i < cameras.Length; i++)
            {
                var clip = track.CreateClip<CinemachineShot>();
                clip.start = start;
                clip.duration = durations[i];
                clip.displayName = cameras[i].name;
                ((CinemachineShot)clip.asset).VirtualCamera.defaultValue = cameras[i];
                start += durations[i];
            }

            return timeline;
        }

        public void AddCommand(PlayerCommandData command)
        {
            if (_phase != EncounterPhase.Planning || _pendingCommands.Count >= TurnPlan.CommandCount)
            {
                return;
            }

            var candidate = _pendingCommands.Append(command).ToList();
            while (candidate.Count < TurnPlan.CommandCount)
            {
                candidate.Add(PlayerCommandData.Wait());
            }

            var validation = _simulator.ValidatePlan(_state, new TurnPlan(candidate));
            if (!validation.IsValid)
            {
                _message = validation.Message;
                return;
            }

            _pendingCommands.Add(command);
            _message = string.Empty;
            RefreshPreview();
        }

        public void RemoveLastCommand()
        {
            if (_phase != EncounterPhase.Planning || _pendingCommands.Count == 0)
            {
                return;
            }

            _pendingCommands.RemoveAt(_pendingCommands.Count - 1);
            _message = string.Empty;
            RefreshPreview();
        }

        public void ClearCommands()
        {
            _pendingCommands.Clear();
            _message = string.Empty;
            RefreshPreview();
        }

        public void ConfirmPlan()
        {
            if (_phase != EncounterPhase.Planning)
            {
                return;
            }

            var plan = CreateCompletedPlan();
            var validation = _simulator.ValidatePlan(_state, plan);
            if (!validation.IsValid)
            {
                _message = validation.Message;
                return;
            }

            ExecuteTurnAsync(plan);
        }

        private async void ExecuteTurnAsync(TurnPlan plan)
        {
            CancelExecution();
            _executionCancellation = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
            var cancellationToken = _executionCancellation.Token;

            try
            {
                _phase = EncounterPhase.Executing;
                _message = "Executing sequence";
                ClearRouteMarkers();

                var result = _simulator.Simulate(_state, plan, _currentPattern);
                await _turnRunner.ExecuteAsync(result, cancellationToken);

                _phase = EncounterPhase.Evaluating;
                _state = result.State;
                PresentState();

                switch (result.Outcome)
                {
                    case EncounterOutcome.Victory:
                        await RunFinaleAsync(cancellationToken);
                        _phase = EncounterPhase.Results;
                        _message = "Sequence complete";
                        break;
                    case EncounterOutcome.HealthDefeat:
                        _phase = EncounterPhase.Results;
                        _message = "Guardian prevailed";
                        break;
                    case EncounterOutcome.TurnLimitDefeat:
                        _phase = EncounterPhase.Results;
                        _message = "Sanctuary collapsed";
                        break;
                    default:
                        ClearCommands();
                        RevealNextPattern();
                        break;
                }
            }
            catch (OperationCanceledException)
            {
                _message = "Execution cancelled";
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
                _phase = EncounterPhase.Results;
                _message = "Sequence failed; see Console";
            }
        }

        private void RevealNextPattern()
        {
            _phase = EncounterPhase.Reveal;
            var template = _patternDirector.Select(_state.DestroyedAnchorCount);
            _currentPattern = Retarget(template, _state.Player.Position, _state.Grid);
            _phase = EncounterPhase.Planning;
            _message = "Read. Plan. Execute.";
            RefreshTelegraphs();
            RefreshPreview();
        }

        private void RefreshPreview()
        {
            if (_state == null || _currentPattern == null || _simulator == null)
            {
                return;
            }

            var plan = CreateCompletedPlan();
            var validation = _simulator.ValidatePlan(_state, plan);
            _preview = validation.IsValid ? _simulator.Simulate(_state, plan, _currentPattern) : null;
            RefreshRouteMarkers();
        }

        private TurnPlan CreateCompletedPlan()
        {
            var commands = _pendingCommands.ToList();
            while (commands.Count < TurnPlan.CommandCount)
            {
                commands.Add(PlayerCommandData.Wait());
            }

            return new TurnPlan(commands);
        }

        private void AddDirectionalCommand(Vector2Int direction)
        {
            AddCommand(_selectedCommand == PlayerCommandType.Dash
                ? PlayerCommandData.Dash(direction)
                : PlayerCommandData.Step(direction));
        }

        private void PresentBeat(BeatResult beat)
        {
            foreach (var anchorId in beat.AwakenedAnchorIds)
            {
                if (_anchorViews.TryGetValue(anchorId, out var anchorView))
                {
                    anchorView.Present(awakened: true, destroyed: anchorId == beat.DestroyedAnchorId);
                }
            }

            if (beat.AnchorWasDestroyed && _anchorViews.TryGetValue(beat.DestroyedAnchorId, out var destroyedView))
            {
                destroyedView.Present(awakened: false, destroyed: true);
                _message = $"Anchor {beat.DestroyedAnchorId + 1} shattered";
            }
            else if (beat.GuardConsumed)
            {
                _message = "Damage blocked";
            }
            else if (beat.PlayerWasHit)
            {
                _message = $"Plan exposed you on Beat {beat.BeatIndex + 1}";
            }
        }

        private void PresentState()
        {
            if (_state == null)
            {
                return;
            }

            foreach (var anchor in _state.Anchors)
            {
                if (_anchorViews.TryGetValue(anchor.Id, out var view))
                {
                    view.Present(anchor.Awakened, anchor.Destroyed);
                }
            }

            foreach (var cellView in _cellViews)
            {
                SetObjectColor(cellView.Value, _state.Grid.IsBlocked(cellView.Key)
                    ? new Color(0.04f, 0.04f, 0.04f)
                    : _gridColor);
            }
        }

        private async Awaitable RunFinaleAsync(CancellationToken cancellationToken)
        {
            _phase = EncounterPhase.Finale;
            _message = "Guardian exposed";

            if (_runtimeFinaleCameras != null)
            {
                SetAllCameraPriorities(_runtimeIntroCameras, 0);
                SetAllCameraPriorities(_runtimeFinaleCameras, 0);
                var finaleCamera = _runtimeFinaleCameras[0];
                var heroFocus = GetActorFocus(_playerView.transform);
                var guardianFocus = GetActorFocus(_guardianView.transform);
                var confrontationFocus = Vector3.Lerp(heroFocus, guardianFocus, 0.5f);
                var exposedPosition = guardianFocus + new Vector3(6f, 3.2f, -5.5f);
                var approachPosition = heroFocus + new Vector3(5f, 2.5f, 4.2f);
                var releasePosition = confrontationFocus + new Vector3(11f, 9f, -14f);

                finaleCamera.Priority.Value = 100;
                finaleCamera.transform.SetPositionAndRotation(
                    exposedPosition,
                    Quaternion.LookRotation(guardianFocus + Vector3.up * 0.6f - exposedPosition, Vector3.up));
                finaleCamera.Lens.FieldOfView = 42f;
                _guardianView.PresentExposed();
                await Awaitable.WaitForSecondsAsync(2f, cancellationToken);

                await MoveCinematicCameraAsync(
                    finaleCamera,
                    approachPosition,
                    heroFocus + Vector3.up * 0.8f,
                    44f,
                    2.8f);
                _playerView.PresentFinalStrike();
                await Awaitable.WaitForSecondsAsync(1.6f, cancellationToken);
                _guardianView.PresentDefeat();
                await Awaitable.WaitForSecondsAsync(1.8f, cancellationToken);

                await MoveCinematicCameraAsync(
                    finaleCamera,
                    releasePosition,
                    confrontationFocus + Vector3.up * 0.8f,
                    50f,
                    3.2f);
                await Awaitable.WaitForSecondsAsync(2.5f, cancellationToken);
            }
            else if (_finaleDirector != null && _finaleDirector.playableAsset != null)
            {
                _finaleDirector.Play();
                while (_finaleDirector.state == PlayState.Playing)
                {
                    await Awaitable.NextFrameAsync(cancellationToken);
                }
            }
            else
            {
                _playerView.PresentFinalStrike();
                _guardianView.PresentDefeat();
                var startScale = _guardianView.transform.localScale;
                var elapsed = 0f;
                while (elapsed < 0.8f)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    elapsed += Time.deltaTime;
                    _guardianView.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, elapsed / 0.8f);
                    await Awaitable.NextFrameAsync(cancellationToken);
                }
            }
        }

        private void CancelExecution()
        {
            if (_executionCancellation == null)
            {
                return;
            }

            _executionCancellation.Cancel();
            _executionCancellation.Dispose();
            _executionCancellation = null;
        }

        private void BuildBoardVisuals()
        {
            var boardRoot = new GameObject("Grid Visuals").transform;
            boardRoot.SetParent(transform);

            for (var x = 1; x <= 7; x++)
            {
                for (var y = 1; y <= 7; y++)
                {
                    var cell = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cell.name = $"Cell {x},{y}";
                    cell.transform.SetParent(boardRoot);
                    cell.transform.position = GridToWorld(new Vector2Int(x, y)) + Vector3.down * 0.1f;
                    cell.transform.localScale = new Vector3(_cellSize * 0.92f, 0.12f, _cellSize * 0.92f);
                    SetObjectColor(cell, _gridColor);
                    _cellViews.Add(new Vector2Int(x, y), cell);
                }
            }
        }

        private void BuildActors()
        {
            var hero = _heroPrefab != null
                ? Instantiate(_heroPrefab, transform)
                : GameObject.CreatePrimitive(PrimitiveType.Capsule);
            hero.name = "Hero";
            hero.transform.SetParent(transform);
            if (_heroPrefab == null)
            {
                hero.transform.localScale = Vector3.one * 0.8f;
            }
            _playerView = hero.GetComponent<GameplayActorView>() ?? hero.AddComponent<GameplayActorView>();

            var guardian = _guardianPrefab != null
                ? Instantiate(_guardianPrefab, transform)
                : GameObject.CreatePrimitive(PrimitiveType.Sphere);
            guardian.name = "Guardian";
            guardian.transform.SetParent(transform);
            guardian.transform.position = GridToWorld(new Vector2Int(4, 8)) + Vector3.up * 1.5f;
            if (_guardianPrefab == null)
            {
                guardian.transform.localScale = Vector3.one * 2f;
            }
            _guardianView = guardian.GetComponent<GuardianView>() ?? guardian.AddComponent<GuardianView>();
            _guardianInitialScale = guardian.transform.localScale;

            if (_guardianPrefab == null)
            {
                SetObjectColor(guardian, new Color(0.5f, 0.08f, 0.65f));
            }
        }

        private void BuildAnchorVisuals()
        {
            foreach (var view in _anchorViews.Values)
            {
                if (view != null)
                {
                    Destroy(view.gameObject);
                }
            }
            _anchorViews.Clear();

            foreach (var anchor in _state.Anchors)
            {
                var anchorObject = _anchorPrefab != null
                    ? Instantiate(_anchorPrefab, transform)
                    : GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                anchorObject.name = $"Anchor {anchor.Id + 1}";
                anchorObject.transform.SetParent(transform);
                anchorObject.transform.position = GridToWorld(anchor.Position) + Vector3.up * 0.5f;
                anchorObject.transform.localScale = new Vector3(0.55f, 0.8f, 0.55f);
                var view = anchorObject.GetComponent<AnchorView>() ?? anchorObject.AddComponent<AnchorView>();
                _anchorViews.Add(anchor.Id, view);
            }
        }

        private void RefreshTelegraphs()
        {
            ClearObjects(_telegraphs);
            if (_currentPattern == null)
            {
                return;
            }

            var colors = new[] { _dangerOne, _dangerTwo, _dangerThree };
            for (var beatIndex = 0; beatIndex < _currentPattern.Intents.Count; beatIndex++)
            {
                foreach (var cell in _currentPattern.Intents[beatIndex].AffectedCells)
                {
                    if (!_state.Grid.Contains(cell))
                    {
                        continue;
                    }

                    var marker = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    marker.name = $"Danger {beatIndex + 1} {cell.x},{cell.y}";
                    marker.transform.SetParent(transform);
                    marker.transform.position = GridToWorld(cell) + Vector3.up * (0.03f + beatIndex * 0.015f);
                    marker.transform.localScale = new Vector3(_cellSize * 0.82f, 0.035f, _cellSize * 0.82f);
                    SetObjectColor(marker, colors[beatIndex]);
                    _telegraphs.Add(marker);
                }
            }
        }

        private void RefreshRouteMarkers()
        {
            ClearRouteMarkers();
            if (_preview == null)
            {
                return;
            }

            foreach (var beat in _preview.Beats)
            {
                var marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                marker.name = $"Route {beat.BeatIndex + 1}";
                marker.transform.SetParent(transform);
                marker.transform.position = GridToWorld(beat.EndPosition) + Vector3.up * 0.35f;
                marker.transform.localScale = Vector3.one * (0.18f + beat.BeatIndex * 0.04f);
                SetObjectColor(marker, beat.PlayerWasHit && !beat.GuardConsumed ? Color.red : Color.cyan);
                _routeMarkers.Add(marker);
            }
        }

        private void ClearRouteMarkers() => ClearObjects(_routeMarkers);

        private static void ClearObjects(List<GameObject> objects)
        {
            foreach (var item in objects)
            {
                if (item != null)
                {
                    Destroy(item);
                }
            }
            objects.Clear();
        }

        private GuardianPattern Retarget(GuardianPattern template, Vector2Int target, GridState grid)
        {
            var intents = template.Intents.Select((intent, beat) =>
                CreateIntent(intent.Type, template.Id, beat, target, grid)).ToArray();
            return new GuardianPattern(template.Id, template.Tier, intents);
        }

        private static GuardianIntent CreateIntent(
            GuardianAttackType type,
            string patternId,
            int beat,
            Vector2Int target,
            GridState grid)
        {
            switch (type)
            {
                case GuardianAttackType.LineBlast:
                {
                    var vertical = patternId.Contains("COL", StringComparison.Ordinal);
                    var cells = vertical
                        ? Enumerable.Range(1, grid.Height).Select(y => new Vector2Int(target.x, y))
                        : Enumerable.Range(1, grid.Width).Select(x => new Vector2Int(x, target.y));
                    return new GuardianIntent(type, cells, vertical ? "Line Column" : "Line Row");
                }
                case GuardianAttackType.Rupture:
                    return new GuardianIntent(type, new[]
                    {
                        target,
                        target + Vector2Int.up,
                        target + Vector2Int.down,
                        target + Vector2Int.left,
                        target + Vector2Int.right
                    }.Where(grid.Contains), "Rupture");
                case GuardianAttackType.ArcSweep:
                {
                    var centerX = target.x <= 4 ? 2 : 6;
                    var cells = Enumerable.Range(1, grid.Height)
                        .SelectMany(y => Enumerable.Range(Mathf.Max(1, centerX - 1), 3)
                            .Select(x => new Vector2Int(x, y)))
                        .Where(grid.Contains);
                    return new GuardianIntent(type, cells, "Arc Sweep");
                }
                default:
                    return GuardianIntent.Wait();
            }
        }

        private static IReadOnlyList<GuardianPattern> CreatePatternTemplates()
        {
            GuardianIntent Empty(GuardianAttackType type) => new(type, Array.Empty<Vector2Int>());
            GuardianPattern Make(string id, int tier, params GuardianAttackType[] attacks) =>
                new(id, tier, attacks.Select(Empty).ToArray());

            return new[]
            {
                Make("T1-ROW", 1, GuardianAttackType.Wait, GuardianAttackType.LineBlast, GuardianAttackType.Wait),
                Make("T1-COL", 1, GuardianAttackType.Wait, GuardianAttackType.Wait, GuardianAttackType.LineBlast),
                Make("T1-RUPTURE", 1, GuardianAttackType.Wait, GuardianAttackType.Rupture, GuardianAttackType.Wait),
                Make("T2-ROW-RUPTURE", 2, GuardianAttackType.LineBlast, GuardianAttackType.Wait, GuardianAttackType.Rupture),
                Make("T2-COL-ARC", 2, GuardianAttackType.Wait, GuardianAttackType.LineBlast, GuardianAttackType.ArcSweep),
                Make("T2-RUPTURE-ROW", 2, GuardianAttackType.Rupture, GuardianAttackType.Wait, GuardianAttackType.LineBlast),
                Make("T3-ROW-ARC", 3, GuardianAttackType.LineBlast, GuardianAttackType.ArcSweep, GuardianAttackType.Rupture),
                Make("T3-COL-RUPTURE", 3, GuardianAttackType.Rupture, GuardianAttackType.LineBlast, GuardianAttackType.ArcSweep),
                Make("T3-ARC-COL", 3, GuardianAttackType.ArcSweep, GuardianAttackType.Rupture, GuardianAttackType.LineBlast)
            };
        }

        private static Vector2Int[] SelectBlockedCells(string seed)
        {
            var sets = new[]
            {
                new[] { new Vector2Int(2, 5), new Vector2Int(6, 3) },
                new[] { new Vector2Int(2, 3), new Vector2Int(6, 5), new Vector2Int(4, 6) },
                new[] { new Vector2Int(1, 4), new Vector2Int(7, 4) }
            };
            var random = new DeterministicRandom(seed + "-BLOCKERS");
            var playerStart = new Vector2Int(4, 1);
            return sets[random.Next(sets.Length)]
                .Where(cell => cell != playerStart)
                .ToArray();
        }

        private static AnchorState[] SelectAnchors(string seed, IReadOnlyCollection<Vector2Int> blockedCells)
        {
            var sets = new[]
            {
                new[] { new Vector2Int(2, 6), new Vector2Int(6, 6), new Vector2Int(4, 4) },
                new[] { new Vector2Int(2, 4), new Vector2Int(6, 4), new Vector2Int(4, 6) },
                new[] { new Vector2Int(1, 6), new Vector2Int(7, 6), new Vector2Int(4, 3) }
            };
            var random = new DeterministicRandom(seed + "-ANCHORS");
            var selected = sets[random.Next(sets.Length)]
                .Where(cell => !blockedCells.Contains(cell))
                .Take(3)
                .ToArray();

            if (selected.Length != 3)
            {
                selected = sets[0];
            }

            return selected.Select((position, id) => new AnchorState(id, position)).ToArray();
        }

        private void EnsureCameraAndLight()
        {
            var mainCamera = Camera.main;
            if (mainCamera == null)
            {
                var cameraObject = new GameObject("Main Camera");
                cameraObject.tag = "MainCamera";
                cameraObject.transform.SetParent(transform);
                mainCamera = cameraObject.AddComponent<Camera>();
            }

            if (mainCamera.GetComponent<Unity.Cinemachine.CinemachineBrain>() == null)
            {
                var brain = mainCamera.gameObject.AddComponent<Unity.Cinemachine.CinemachineBrain>();
                brain.DefaultBlend = new Unity.Cinemachine.CinemachineBlendDefinition(
                    Unity.Cinemachine.CinemachineBlendDefinition.Styles.EaseInOut, 1.2f);
            }

            var targetObject = new GameObject("Board Camera Target");
            targetObject.transform.SetParent(transform);
            targetObject.transform.position = _gridOrigin + new Vector3(_cellSize * 3f, 0.4f, _cellSize * 3.35f);
            var rig = gameObject.GetComponent<GameplayCameraRig>() ?? gameObject.AddComponent<GameplayCameraRig>();
            rig.Configure(targetObject.transform);

            if (FindFirstObjectByType<Light>() == null)
            {
                var lightObject = new GameObject("Directional Light");
                lightObject.transform.SetParent(transform);
                var light = lightObject.AddComponent<Light>();
                light.type = LightType.Directional;
                light.intensity = 1.2f;
                light.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            }
        }

        private Vector3 GridToWorld(Vector2Int cell) =>
            _gridOrigin + new Vector3((cell.x - 1) * _cellSize, 0f, (cell.y - 1) * _cellSize);

        private static void SetObjectColor(GameObject target, Color color)
        {
            var renderer = target.GetComponentInChildren<Renderer>();
            if (renderer == null)
            {
                return;
            }

            var block = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(block);
            block.SetColor("_BaseColor", color);
            block.SetColor("_Color", color);
            renderer.SetPropertyBlock(block);
        }

        private static string CreateSeed() => $"FOREST-{UnityEngine.Random.Range(0, 10000):0000}";

        private void OnGUI()
        {
            var panel = new Rect(20f, 20f, 440f, 230f);
            GUI.Box(panel, GUIContent.none);
            GUILayout.BeginArea(new Rect(panel.x + 16f, panel.y + 12f, panel.width - 32f, panel.height - 24f));

            GUILayout.Label($"THE LAST SEQUENCE   |   {_phase}");
            if (_state != null)
            {
                GUILayout.Label($"Health: {new string('♥', _state.Player.Health)}   Turn: {_state.Turn + 1}/8   Score: {_state.Score} ×{_state.Multiplier}");
                GUILayout.Label($"Seed: {_state.Seed}   Anchors: {_state.DestroyedAnchorCount}/3");
            }

            if (_currentPattern != null)
            {
                GUILayout.Label("Guardian: " + string.Join("  →  ", _currentPattern.Intents.Select(intent => intent.DisplayName)));
            }

            GUILayout.Label("Plan: " + (_pendingCommands.Count == 0
                ? "[Wait] → [Wait] → [Wait]"
                : string.Join(" → ", _pendingCommands.Select(command => command.ToString()))));
            GUILayout.Label($"Selected: {_selectedCommand}   |   1 Step  2 Dash  3 Guard  4 Wait");
            GUILayout.Label("WASD/Arrows add direction · Backspace undo · Enter execute");
            GUILayout.Label(_message);

            if (_phase == EncounterPhase.Results)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Replay Seed")) StartRun(_seed);
                if (GUILayout.Button("New Run")) StartRun(CreateSeed());
                GUILayout.EndHorizontal();
            }

            GUILayout.EndArea();
        }
    }
}
