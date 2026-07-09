using UnityEngine;
using System.Collections.Generic;
using SoraKobo.Data;

namespace SoraKobo.Building
{
    /// <summary>
    /// Simple undo/redo for the Map Editor (offline, no network needed).
    /// Each action stores the delta that can be reversed.
    /// </summary>
    public class UndoRedoSystem : MonoBehaviour
    {
        public static UndoRedoSystem Instance { get; private set; }

        [Header("Settings")]
        public int maxHistory = 50;

        private struct EditorAction
        {
            public string blockId;
            public Vector2Int pos;
            public int layer;
            public bool wasPlaced; // true = undo removes, false = undo restores
        }

        private Stack<EditorAction> _undoStack = new Stack<EditorAction>();
        private Stack<EditorAction> _redoStack = new Stack<EditorAction>();

        private MapEditorController _editor;

        void Awake()
        {
            Instance = this;
            _editor = GetComponent<MapEditorController>();
            if (_editor == null) _editor = FindObjectOfType<MapEditorController>();
        }

        public void RecordPlace(string blockId, Vector2Int pos, int layer)
        {
            _undoStack.Push(new EditorAction { blockId = blockId, pos = pos, layer = layer, wasPlaced = true });
            _redoStack.Clear();
            if (_undoStack.Count > maxHistory)
            {
                // Trim oldest by rebuilding stack
                var list = new List<EditorAction>(_undoStack);
                list.RemoveAt(list.Count - 1);
                _undoStack = new Stack<EditorAction>(list);
            }
        }

        public void RecordRemove(string blockId, Vector2Int pos, int layer)
        {
            _undoStack.Push(new EditorAction { blockId = blockId, pos = pos, layer = layer, wasPlaced = false });
            _redoStack.Clear();
        }

        public void Undo()
        {
            if (_undoStack.Count == 0) return;
            var action = _undoStack.Pop();
            _redoStack.Push(action);

            if (action.wasPlaced)
                _editor?.RemoveBlock_Editor(action.pos, action.layer);
            else
                _editor?.PlaceBlock_Editor(action.blockId, action.pos, action.layer);

            UI.ToastNotification.Show("Undo");
        }

        public void Redo()
        {
            if (_redoStack.Count == 0) return;
            var action = _redoStack.Pop();
            _undoStack.Push(action);

            if (action.wasPlaced)
                _editor?.PlaceBlock_Editor(action.blockId, action.pos, action.layer);
            else
                _editor?.RemoveBlock_Editor(action.pos, action.layer);

            UI.ToastNotification.Show("Redo");
        }

        public void Clear()
        {
            _undoStack.Clear();
            _redoStack.Clear();
        }

        public bool CanUndo => _undoStack.Count > 0;
        public bool CanRedo => _redoStack.Count > 0;
    }
}
