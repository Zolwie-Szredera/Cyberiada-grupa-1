using System.Collections.Generic;
using UnityEngine;

namespace SecretLevel
{
    [RequireComponent(typeof(Collider2D))]
    public class SlotMachineTriggerRoll : MonoBehaviour
    {
        [SerializeField] private SlotMachineUi _slotMachineUi;
        [SerializeField] private List<PowerUpType> _allowedPowerUps = new();
        [SerializeField] private string _targetTag = "Player";
        [SerializeField] private bool _triggerOnlyOnce = true;

        private bool _used;

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
            if (_triggerOnlyOnce && _used)
            {
                return;
            }

            if (!other.CompareTag(_targetTag))
            {
                return;
            }

            if (_slotMachineUi == null)
            {
                Debug.LogWarning("[SlotMachineTriggerRoll] Missing SlotMachineUi reference.");
                return;
            }

            PowerUpType? rolled = (_allowedPowerUps != null && _allowedPowerUps.Count > 0)
                ? _slotMachineUi.RollRandomItem(_allowedPowerUps)
                : _slotMachineUi.RollRandomItem();

            if (rolled.HasValue)
            {
                _used = true;
            }
        }
    }
}

