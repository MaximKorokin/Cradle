using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets._Game.Scripts.Entities.Units
{
    public sealed class EntityUnitTree
    {
        private readonly Transform _unitsRoot;

        private readonly Dictionary<string, EntityUnit> _byPath = new();
        private readonly HashSet<EntityUnit> _roots = new();
        private readonly Dictionary<string, List<EntityUnit>> _pendingByParentPath = new();

        public IReadOnlyCollection<EntityUnit> Roots => _roots;

        public EntityUnitTree(Transform unitsRoot)
        {
            _unitsRoot = unitsRoot ? unitsRoot : throw new ArgumentNullException(nameof(unitsRoot));
        }

        public bool TryGet(string path, out EntityUnit unit) => _byPath.TryGetValue(path, out unit);

        public void Add(EntityUnit unit)
        {
            if (unit == null) throw new ArgumentNullException(nameof(unit));
            if (_byPath.ContainsKey(unit.Path))
                throw new InvalidOperationException($"Unit already exists: {unit.Path}");

            _byPath[unit.Path] = unit;

            var parentPath = GetParentPath(unit.Path);
            if (parentPath != null && _byPath.TryGetValue(parentPath, out var parent))
            {
                AttachChild(parent, unit);
            }
            else
            {
                MakeRoot(unit);
                if (parentPath != null)
                    AddPending(parentPath, unit);
            }

            AttachPendingChildren(unit);
        }

        public bool RemoveRecursive(string path, bool destroyGameObjects = true)
        {
            if (!_byPath.TryGetValue(path, out var root))
                return false;

            // Collect subtree (post-order or pre-order doesn't matter for Destroy, but we need a stable list)
            var toRemove = new List<EntityUnit>();
            ExecuteDepthFirst(root, u => toRemove.Add(u));

            // Detach subtree root from parent (or roots set)
            DetachFromParentOrRoots(root);

            // Detach transforms + destroy (children will be destroyed anyway if parent is destroyed,
            // but we also clean dictionaries for every node).
            for (int i = 0; i < toRemove.Count; i++)
            {
                var u = toRemove[i];

                // Remove from maps
                _byPath.Remove(u.Path);
                _roots.Remove(u);
                _pendingByParentPath.Remove(u.Path);

                // Break hierarchy links in memory
                u.Parent = null;
                u.Children.Clear();

                // Break transform hierarchy
                if (u.GameObject != null)
                    u.GameObject.transform.SetParent(null, worldPositionStays: false);

                // Remove GameObject from scene hierarchy
                if (destroyGameObjects && u.GameObject != null)
                    UnityEngine.Object.Destroy(u.GameObject);
            }

            return true;
        }

        public void ExecuteAllDepthFirst(Action<EntityUnit> action)
        {
            foreach (var r in _roots)
                ExecuteDepthFirst(r, action);
        }

        public void UpdateOrderInLayer(int pivotOrderInLayer)
        {
            foreach (var root in _roots)
                ApplyOrderRecursive(root, pivotOrderInLayer);
        }

        // ---------------- internals ----------------

        private static void ExecuteDepthFirst(EntityUnit root, Action<EntityUnit> action)
        {
            action(root);
            for (int i = 0; i < root.Children.Count; i++)
                ExecuteDepthFirst(root.Children[i], action);
        }

        private void AttachChild(EntityUnit parent, EntityUnit child)
        {
            _roots.Remove(child);

            if (_pendingByParentPath.TryGetValue(parent.Path, out var list))
                list.Remove(child);

            child.Parent = parent;
            parent.Children.Add(child);

            child.GameObject.transform.SetParent(parent.GameObject.transform, worldPositionStays: false);
            child.GameObject.transform.localPosition = Vector3.zero;
        }

        private void MakeRoot(EntityUnit unit)
        {
            unit.Parent = null;
            _roots.Add(unit);

            unit.GameObject.transform.SetParent(_unitsRoot, worldPositionStays: false);
            unit.GameObject.transform.localPosition = Vector3.zero;
        }

        private void DetachFromParentOrRoots(EntityUnit unit)
        {
            if (unit.Parent != null)
            {
                unit.Parent.Children.Remove(unit);
                unit.Parent = null;
            }
            else
            {
                _roots.Remove(unit);
            }
        }

        private void AddPending(string parentPath, EntityUnit child)
        {
            if (!_pendingByParentPath.TryGetValue(parentPath, out var list))
                _pendingByParentPath[parentPath] = list = new List<EntityUnit>();

            if (!list.Contains(child))
                list.Add(child);
        }

        private void AttachPendingChildren(EntityUnit newParent)
        {
            if (!_pendingByParentPath.TryGetValue(newParent.Path, out var list) || list.Count == 0)
                return;

            var pending = list.ToArray();
            list.Clear();

            for (int i = 0; i < pending.Length; i++)
            {
                var child = pending[i];
                if (child.Parent != null) continue;

                AttachChild(newParent, child);
                AttachPendingChildren(child);
            }
        }

        private static string GetParentPath(string path)
        {
            var parent = Path.GetDirectoryName(path);
            return string.IsNullOrEmpty(parent) ? null : parent;
        }

        private static void ApplyOrderRecursive(EntityUnit unit, int parentOrder)
        {
            // Order is relative to the immediate parent
            int myOrder = parentOrder + unit.RelativeOrderInLayer;
            unit.SpriteRenderer.sortingOrder = myOrder;

            for (int i = 0; i < unit.Children.Count; i++)
                ApplyOrderRecursive(unit.Children[i], myOrder);
        }
    }
}
