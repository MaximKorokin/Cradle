using System;
using System.Linq;
using UnityEngine;

namespace Assets._Game.Scripts.Shared.Unity
{
    public sealed class UnityOperationTrigger : MonoBehaviour
    {
        [SerializeField]
        private UnityOperationTarget _operationTarget;
        [SerializeField]
        private UnityOperation _operation;
        [SerializeField]
        private UnityOperationTriggerType _triggerType;

        private void Awake()
        {
            if (_triggerType.HasFlag(UnityOperationTriggerType.OnAwake))
            {
                ExecuteOperation();
            }
        }

        private void OnEnable()
        {
            if (_triggerType.HasFlag(UnityOperationTriggerType.OnEnable))
            {
                ExecuteOperation();
            }
        }

        private void OnDisable()
        {
            if (_triggerType.HasFlag(UnityOperationTriggerType.OnDisable))
            {
                ExecuteOperation();
            }
        }

        private void Start()
        {
            if (_triggerType.HasFlag(UnityOperationTriggerType.OnStart))
            {
                ExecuteOperation();
            }
        }

        private void OnDestroy()
        {
            if (_triggerType.HasFlag(UnityOperationTriggerType.OnDestroy))
            {
                ExecuteOperation();
            }
        }

        private void ExecuteOperation()
        {
            var targets = GetTargetGameObjects();
            if (targets == null || targets.Length == 0)
            {
                Debug.LogWarning($"UnityOperationTrigger: Target GameObject not found for operation '{_operation}' on '{gameObject.name}'.");
                return;
            }

            foreach (var target in targets)
            {
                switch (_operation)
                {
                    case UnityOperation.DestroyGameObject:
                        Destroy(target);
                        break;
                    case UnityOperation.SetActiveTrue:
                        target.SetActive(true);
                        break;
                    case UnityOperation.SetActiveFalse:
                        target.SetActive(false);
                        break;
                    default:
                        Debug.LogWarning($"UnityOperationTrigger: Unknown operation '{_operation}' on '{gameObject.name}'.");
                        break;
                }
            }
        }

        private GameObject[] GetTargetGameObjects()
        {
            switch (_operationTarget)
            {
                case UnityOperationTarget.Self:
                    return new GameObject[] { gameObject };
                case UnityOperationTarget.Parent:
                    if (transform.parent == null)
                    {
                        Debug.LogWarning($"UnityOperationTrigger: No parent found for '{gameObject.name}' when targeting Parent.");
                        return null;
                    }
                    return new GameObject[] { transform.parent.gameObject };
                case UnityOperationTarget.Children:
                    if (transform.childCount > 0)
                    {
                        return transform.Cast<Transform>().Select(t => t.gameObject).ToArray();
                    }
                    Debug.LogWarning($"UnityOperationTrigger: No children found for '{gameObject.name}' when targeting Children.");
                    return null;
                default:
                    return null;
            }
        }
    }

    public enum UnityOperationTarget
    {
        Self = 1,
        Parent = 2,
        Children = 4,
    }

    public enum UnityOperation
    {
        DestroyGameObject = 1,
        SetActiveTrue = 2,
        SetActiveFalse = 4,
    }

    [Flags]
    public enum UnityOperationTriggerType
    {
        None = 0,
        OnAwake = 1,
        OnEnable = 2,
        OnDisable = 4,
        OnStart = 8,
        OnDestroy = 16,
    }
}
