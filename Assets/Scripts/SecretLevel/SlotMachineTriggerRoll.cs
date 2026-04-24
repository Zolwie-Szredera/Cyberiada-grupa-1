using System.Collections.Generic;
using UnityEngine;

namespace SecretLevel
{
    [RequireComponent(typeof(Collider2D))]
    public class SlotMachineTriggerRoll : MonoBehaviour
    {
        [SerializeField] private SlotMachineUi slotMachineUi;
        [SerializeField] private List<PowerUpType> allowedPowerUps = new();
        [SerializeField] private string targetTag = "Player";
        [SerializeField] private bool triggerOnlyOnce = true;

        private bool _used;

        private void Awake()
        {
            TryResolveSlotMachineUi();
        }

        private void Reset()
        {
            Collider2D trigger = GetComponent<Collider2D>();
            if (trigger != null)
            {
                trigger.isTrigger = true;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (triggerOnlyOnce && _used)
            {
                return;
            }

            if (!other.CompareTag(targetTag))
            {
                return;
            }

            if (slotMachineUi == null)
            {
                TryResolveSlotMachineUi();
            }

            if (slotMachineUi == null)
            {
                Debug.LogWarning("[SlotMachineTriggerRoll] Missing SlotMachineUi reference.");
                return;
            }

            PowerUpType? rolled = (allowedPowerUps != null && allowedPowerUps.Count > 0)
                ? slotMachineUi.RollRandomItem(allowedPowerUps)
                : slotMachineUi.RollRandomItem();

            if (rolled.HasValue)
            {
                _used = true;
            }
        }

        private void TryResolveSlotMachineUi()
        {
            if (slotMachineUi != null)
            {
                return;
            }

            slotMachineUi = GetComponent<SlotMachineUi>();
            if (slotMachineUi != null)
            {
                return;
            }

            slotMachineUi = GetComponentInParent<SlotMachineUi>();
            if (slotMachineUi != null)
            {
                return;
            }

            slotMachineUi = FindAnyObjectByType<SlotMachineUi>();
        }
    }
}

