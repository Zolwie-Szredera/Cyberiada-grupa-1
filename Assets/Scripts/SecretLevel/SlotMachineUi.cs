using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace SecretLevel
{
    [RequireComponent(typeof(UIDocument))]
    public class SlotMachineUi : MonoBehaviour
    {
        [Header("Roll animation")]
        [SerializeField] private float rollDuration = 0.8f;
        [SerializeField] private float rollStepInterval = 0.06f;
        [SerializeField] private float resultVisibleDuration = 1.0f;

        private UIDocument _uiDocument;
        private VisualElement _slotRoot;
        private Label _st3Label;
        private Coroutine _rollCoroutine;

        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
        }

        private void OnEnable()
        {
            CacheSt3Label();
            SetSlotVisible(false);
        }

        private void OnDisable()
        {
            if (_rollCoroutine != null)
            {
                StopCoroutine(_rollCoroutine);
                _rollCoroutine = null;
            }

            SetSlotVisible(false);
        }

        public PowerUpType? RollRandomItem(List<PowerUpType> items)
        {
            if (items == null || items.Count == 0)
            {
                return null;
            }

            CacheSt3Label();
            if (_st3Label == null)
            {
                return null;
            }

            PowerUpType selectedItem = items[UnityEngine.Random.Range(0, items.Count)];
            StartRollVisual(items, selectedItem);
            return selectedItem;
        }

        // Convenience overload: randomize from all enum values.
        public PowerUpType? RollRandomItem()
        {
            PowerUpType[] all = (PowerUpType[])Enum.GetValues(typeof(PowerUpType));
            if (all.Length == 0)
            {
                return null;
            }

            PowerUpType selectedItem = all[UnityEngine.Random.Range(0, all.Length)];
            StartRollVisual(new List<PowerUpType>(all), selectedItem);
            return selectedItem;
        }

        private void StartRollVisual(List<PowerUpType> items, PowerUpType finalItem)
        {
            if (_rollCoroutine != null)
            {
                StopCoroutine(_rollCoroutine);
            }

            SetSlotVisible(true);
            _rollCoroutine = StartCoroutine(RollVisualToFinal(items, finalItem));
        }

        private IEnumerator RollVisualToFinal(List<PowerUpType> items, PowerUpType finalItem)
        {
            CacheSt3Label();
            if (_st3Label == null)
            {
                _rollCoroutine = null;
                yield break;
            }

            if (rollDuration <= 0f || rollStepInterval <= 0f)
            {
                _st3Label.text = GetDisplayName(finalItem);
                _rollCoroutine = null;
                yield break;
            }

            float elapsed = 0f;
            while (elapsed < rollDuration)
            {
                PowerUpType preview = items[UnityEngine.Random.Range(0, items.Count)];
                _st3Label.text = GetDisplayName(preview);
                yield return new WaitForSeconds(rollStepInterval);
                elapsed += rollStepInterval;
            }

            _st3Label.text = GetDisplayName(finalItem);

            if (resultVisibleDuration > 0f)
            {
                yield return new WaitForSeconds(resultVisibleDuration);
            }

            SetSlotVisible(false);
            _rollCoroutine = null;
        }

        private void CacheSt3Label()
        {
            if (_uiDocument == null)
            {
                _uiDocument = GetComponent<UIDocument>();
            }

            if (_uiDocument == null || _uiDocument.rootVisualElement == null)
            {
                _slotRoot = null;
                _st3Label = null;
                return;
            }

            _slotRoot = _uiDocument.rootVisualElement.Q<VisualElement>("Gambling");
            VisualElement st3 = _uiDocument.rootVisualElement.Q<VisualElement>("ST3");
            _st3Label = st3 != null ? st3.Q<Label>() : null;
        }

        public void ShowRandomItem(List<PowerUpType> items)
        {
            RollRandomItem(items);
        }

        public void ShowItem(PowerUpType item)
        {
            CacheSt3Label();

            if (_st3Label == null)
            {
                return;
            }

            if (_rollCoroutine != null)
            {
                StopCoroutine(_rollCoroutine);
                _rollCoroutine = null;
            }

            SetSlotVisible(true);
            _st3Label.text = GetDisplayName(item);
        }

        private void SetSlotVisible(bool isVisible)
        {
            if (_slotRoot == null)
            {
                CacheSt3Label();
            }

            if (_slotRoot == null)
            {
                return;
            }

            _slotRoot.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private static string GetDisplayName(PowerUpType value)
        {
            FieldInfo field = typeof(PowerUpType).GetField(value.ToString());
            if (field == null)
            {
                return value.ToString();
            }

            InspectorNameAttribute inspectorName = field.GetCustomAttribute<InspectorNameAttribute>();
            return inspectorName != null ? inspectorName.displayName : value.ToString();
        }
    }
}

