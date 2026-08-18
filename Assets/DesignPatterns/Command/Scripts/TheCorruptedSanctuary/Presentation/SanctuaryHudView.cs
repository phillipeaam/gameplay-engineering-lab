using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace DesignPatterns.Command.Scripts.TheCorruptedSanctuary.Presentation
{
    public readonly struct SanctuaryHudState
    {
        public SanctuaryHudState(
            string phase,
            string health,
            string turn,
            string anchors,
            string score,
            string multiplier,
            string seed,
            string message,
            string[] intents,
            string[] commands,
            bool dashSelected,
            bool canPlan,
            bool showChrome,
            bool showResults,
            string resultTitle,
            string resultSummary)
        {
            Phase = phase;
            Health = health;
            Turn = turn;
            Anchors = anchors;
            Score = score;
            Multiplier = multiplier;
            Seed = seed;
            Message = message;
            Intents = intents;
            Commands = commands;
            DashSelected = dashSelected;
            CanPlan = canPlan;
            ShowChrome = showChrome;
            ShowResults = showResults;
            ResultTitle = resultTitle;
            ResultSummary = resultSummary;
        }

        public string Phase { get; }
        public string Health { get; }
        public string Turn { get; }
        public string Anchors { get; }
        public string Score { get; }
        public string Multiplier { get; }
        public string Seed { get; }
        public string Message { get; }
        public string[] Intents { get; }
        public string[] Commands { get; }
        public bool DashSelected { get; }
        public bool CanPlan { get; }
        public bool ShowChrome { get; }
        public bool ShowResults { get; }
        public string ResultTitle { get; }
        public string ResultSummary { get; }
    }

    public sealed class SanctuaryHudView : IDisposable
    {
        private readonly Label _phase;
        private readonly Label _health;
        private readonly Label _turn;
        private readonly Label _anchors;
        private readonly Label _score;
        private readonly Label _multiplier;
        private readonly Label _seed;
        private readonly Label _message;
        private readonly Label[] _intents;
        private readonly Label[] _commands;
        private readonly VisualElement _planningControls;
        private readonly VisualElement[] _chromePanels;
        private readonly VisualElement _resultsPanel;
        private readonly Label _resultTitle;
        private readonly Label _resultSummary;
        private readonly Button _stepButton;
        private readonly Button _dashButton;
        private readonly Button _guardButton;
        private readonly Button _waitButton;
        private readonly Button _upButton;
        private readonly Button _downButton;
        private readonly Button _leftButton;
        private readonly Button _rightButton;
        private readonly Button _undoButton;
        private readonly Button _clearButton;
        private readonly Button _confirmButton;
        private readonly Button _replayButton;
        private readonly Button _newRunButton;

        public SanctuaryHudView(VisualElement root)
        {
            if (root == null)
            {
                throw new ArgumentNullException(nameof(root));
            }

            _phase = Require<Label>(root, "phase-label");
            _health = Require<Label>(root, "health-label");
            _turn = Require<Label>(root, "turn-label");
            _anchors = Require<Label>(root, "anchors-label");
            _score = Require<Label>(root, "score-label");
            _multiplier = Require<Label>(root, "multiplier-label");
            _seed = Require<Label>(root, "seed-label");
            _message = Require<Label>(root, "message-label");
            _intents = new[]
            {
                Require<Label>(root, "intent-1"),
                Require<Label>(root, "intent-2"),
                Require<Label>(root, "intent-3")
            };
            _commands = new[]
            {
                Require<Label>(root, "plan-1"),
                Require<Label>(root, "plan-2"),
                Require<Label>(root, "plan-3")
            };
            _planningControls = Require<VisualElement>(root, "planning-controls");
            _chromePanels = new[]
            {
                Require<VisualElement>(root, "status-panel"),
                Require<VisualElement>(root, "sequence-panel"),
                Require<VisualElement>(root, "command-dock")
            };
            _resultsPanel = Require<VisualElement>(root, "results-panel");
            _resultTitle = Require<Label>(root, "result-title");
            _resultSummary = Require<Label>(root, "result-summary");
            _stepButton = Require<Button>(root, "step-button");
            _dashButton = Require<Button>(root, "dash-button");
            _guardButton = Require<Button>(root, "guard-button");
            _waitButton = Require<Button>(root, "wait-button");
            _upButton = Require<Button>(root, "up-button");
            _downButton = Require<Button>(root, "down-button");
            _leftButton = Require<Button>(root, "left-button");
            _rightButton = Require<Button>(root, "right-button");
            _undoButton = Require<Button>(root, "undo-button");
            _clearButton = Require<Button>(root, "clear-button");
            _confirmButton = Require<Button>(root, "confirm-button");
            _replayButton = Require<Button>(root, "replay-button");
            _newRunButton = Require<Button>(root, "new-run-button");

            _stepButton.clicked += RaiseStepSelected;
            _dashButton.clicked += RaiseDashSelected;
            _guardButton.clicked += RaiseGuardRequested;
            _waitButton.clicked += RaiseWaitRequested;
            _upButton.clicked += RaiseUpRequested;
            _downButton.clicked += RaiseDownRequested;
            _leftButton.clicked += RaiseLeftRequested;
            _rightButton.clicked += RaiseRightRequested;
            _undoButton.clicked += RaiseUndoRequested;
            _clearButton.clicked += RaiseClearRequested;
            _confirmButton.clicked += RaiseConfirmRequested;
            _replayButton.clicked += RaiseReplayRequested;
            _newRunButton.clicked += RaiseNewRunRequested;
        }

        public event Action StepSelected;
        public event Action DashSelected;
        public event Action GuardRequested;
        public event Action WaitRequested;
        public event Action<Vector2Int> DirectionRequested;
        public event Action UndoRequested;
        public event Action ClearRequested;
        public event Action ConfirmRequested;
        public event Action ReplayRequested;
        public event Action NewRunRequested;

        public void Present(SanctuaryHudState state)
        {
            _phase.text = state.Phase;
            _health.text = state.Health;
            _turn.text = state.Turn;
            _anchors.text = state.Anchors;
            _score.text = state.Score;
            _multiplier.text = state.Multiplier;
            _seed.text = state.Seed;
            _message.text = string.IsNullOrWhiteSpace(state.Message) ? "Awaiting your command..." : state.Message;

            for (var index = 0; index < _intents.Length; index++)
            {
                _intents[index].text = ValueAt(state.Intents, index, "UNKNOWN");
                _commands[index].text = ValueAt(state.Commands, index, "—");
            }

            _stepButton.EnableInClassList("is-selected", !state.DashSelected);
            _dashButton.EnableInClassList("is-selected", state.DashSelected);
            _planningControls.SetEnabled(state.CanPlan);
            foreach (var panel in _chromePanels)
            {
                panel.EnableInClassList("cinematic-hidden", !state.ShowChrome);
            }

            _resultsPanel.EnableInClassList("is-hidden", !state.ShowResults);
            _resultTitle.text = state.ResultTitle;
            _resultSummary.text = state.ResultSummary;
        }

        public void Dispose()
        {
            _stepButton.clicked -= RaiseStepSelected;
            _dashButton.clicked -= RaiseDashSelected;
            _guardButton.clicked -= RaiseGuardRequested;
            _waitButton.clicked -= RaiseWaitRequested;
            _upButton.clicked -= RaiseUpRequested;
            _downButton.clicked -= RaiseDownRequested;
            _leftButton.clicked -= RaiseLeftRequested;
            _rightButton.clicked -= RaiseRightRequested;
            _undoButton.clicked -= RaiseUndoRequested;
            _clearButton.clicked -= RaiseClearRequested;
            _confirmButton.clicked -= RaiseConfirmRequested;
            _replayButton.clicked -= RaiseReplayRequested;
            _newRunButton.clicked -= RaiseNewRunRequested;

            StepSelected = null;
            DashSelected = null;
            GuardRequested = null;
            WaitRequested = null;
            DirectionRequested = null;
            UndoRequested = null;
            ClearRequested = null;
            ConfirmRequested = null;
            ReplayRequested = null;
            NewRunRequested = null;
        }

        private static T Require<T>(VisualElement root, string name) where T : VisualElement
        {
            var element = root.Q<T>(name);
            if (element == null)
            {
                throw new InvalidOperationException($"Sanctuary HUD is missing required element '{name}'.");
            }

            return element;
        }

        private static string ValueAt(string[] values, int index, string fallback) =>
            values != null && index >= 0 && index < values.Length && !string.IsNullOrWhiteSpace(values[index])
                ? values[index]
                : fallback;

        private void RaiseStepSelected() => StepSelected?.Invoke();
        private void RaiseDashSelected() => DashSelected?.Invoke();
        private void RaiseGuardRequested() => GuardRequested?.Invoke();
        private void RaiseWaitRequested() => WaitRequested?.Invoke();
        private void RaiseUpRequested() => DirectionRequested?.Invoke(Vector2Int.up);
        private void RaiseDownRequested() => DirectionRequested?.Invoke(Vector2Int.down);
        private void RaiseLeftRequested() => DirectionRequested?.Invoke(Vector2Int.left);
        private void RaiseRightRequested() => DirectionRequested?.Invoke(Vector2Int.right);
        private void RaiseUndoRequested() => UndoRequested?.Invoke();
        private void RaiseClearRequested() => ClearRequested?.Invoke();
        private void RaiseConfirmRequested() => ConfirmRequested?.Invoke();
        private void RaiseReplayRequested() => ReplayRequested?.Invoke();
        private void RaiseNewRunRequested() => NewRunRequested?.Invoke();
    }
}
