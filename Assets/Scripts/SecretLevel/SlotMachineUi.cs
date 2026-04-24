using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace SecretLevel
{
    [RequireComponent(typeof(PowerupEffect))]
    [RequireComponent(typeof(UIDocument))]
    public class SlotMachineUi : MonoBehaviour
    {
        private static readonly string[] SlotNames = { "ST1", "ST2", "ST3", "ST4", "ST5" };

        [Header("Roll animation")]
        [SerializeField] private float rollDuration = 0.8f;
        [SerializeField] private float rollStepInterval = 0.06f;
        [SerializeField] private float resultVisibleDuration = 3.0f;
        [SerializeField] private float maxStepIntervalMultiplier = 2.4f;

        private UIDocument _uiDocument;
        private VisualElement _slotRoot;
        private Label[] _slotLabels;
        private PowerUpType[] _reelValues;
        private Coroutine _rollCoroutine;
        private PowerupEffect powerupEffect;

        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
        }

        private void OnEnable()
        {
            CacheSlotElements();
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
        public void Start()
        {
            powerupEffect = GetComponent<PowerupEffect>();
        }

        public PowerUpType? RollRandomItem(List<PowerUpType> items)
        {
            if (items == null || items.Count == 0)
            {
                return null;
            }

            CacheSlotElements();
            if (!HasAllSlotLabels())
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
            CacheSlotElements();
            if (!HasAllSlotLabels())
            {
                _rollCoroutine = null;
                yield break;
            }

            List<PowerUpType> uniquePool = BuildUniquePool(items);
            if (uniquePool.Count == 0)
            {
                _rollCoroutine = null;
                yield break;
            }

            if (rollDuration <= 0f || rollStepInterval <= 0f)
            {
                SetFinalLayout(uniquePool, finalItem);
                _rollCoroutine = null;
                yield break;
            }

            FillSlotsWithRandom(uniquePool);
            RenderReel();

            float elapsed = 0f;
            while (elapsed < rollDuration)
            {
                ShiftLeftAndAppend(ChooseUniqueForWindow(uniquePool));
                RenderReel();

                float progress = Mathf.Clamp01(elapsed / rollDuration);
                float stepDuration = Mathf.Lerp(rollStepInterval * 0.45f, rollStepInterval * maxStepIntervalMultiplier, progress);
                yield return new WaitForSeconds(stepDuration);
                elapsed += stepDuration;
            }

            // Extra slow settle steps to sell the "slot machine" stop feeling.
            ShiftLeftAndAppend(finalItem);
            RenderReel();
            yield return new WaitForSeconds(rollStepInterval * maxStepIntervalMultiplier);

            ShiftLeftAndAppend(ChooseUniqueForWindow(uniquePool));
            RenderReel();
            yield return new WaitForSeconds(rollStepInterval * (maxStepIntervalMultiplier + 0.35f));

            ShiftLeftAndAppend(ChooseUniqueForWindow(uniquePool));
            RenderReel();

            SetFinalLayout(uniquePool, finalItem);

            // Safety: final selected item must always stay in ST3.
            _reelValues[2] = finalItem;
            RenderReel();

            TriggerEffect(finalItem);

            if (resultVisibleDuration > 0f)
            {
                yield return new WaitForSeconds(resultVisibleDuration);
            }

            SetSlotVisible(false);
            _rollCoroutine = null;
        }

        private void CacheSlotElements()
        {
            if (_uiDocument == null)
            {
                _uiDocument = GetComponent<UIDocument>();
            }

            if (_uiDocument == null || _uiDocument.rootVisualElement == null)
            {
                _slotRoot = null;
                _slotLabels = null;
                return;
            }

            _slotRoot = _uiDocument.rootVisualElement.Q<VisualElement>("Gambling");
            _slotLabels = new Label[SlotNames.Length];
            _reelValues = new PowerUpType[SlotNames.Length];
            for (int i = 0; i < SlotNames.Length; i++)
            {
                VisualElement slot = _uiDocument.rootVisualElement.Q<VisualElement>(SlotNames[i]);
                _slotLabels[i] = slot != null ? slot.Q<Label>() : null;
            }
        }

        public void ShowRandomItem(List<PowerUpType> items)
        {
            RollRandomItem(items);
        }

        public void ShowItem(PowerUpType item)
        {
            CacheSlotElements();

            if (!HasAllSlotLabels())
            {
                return;
            }

            if (_rollCoroutine != null)
            {
                StopCoroutine(_rollCoroutine);
                _rollCoroutine = null;
            }

            SetSlotVisible(true);
            SetFinalLayout(new List<PowerUpType> { item }, item);
        }

        private void SetSlotVisible(bool isVisible)
        {
            if (_slotRoot == null)
            {
                CacheSlotElements();
            }

            if (_slotRoot == null)
            {
                return;
            }

            _slotRoot.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private bool HasAllSlotLabels()
        {
            if (_slotLabels == null || _slotLabels.Length != SlotNames.Length)
            {
                return false;
            }

            for (int i = 0; i < _slotLabels.Length; i++)
            {
                if (_slotLabels[i] == null)
                {
                    return false;
                }
            }

            return true;
        }

        private void FillSlotsWithRandom(List<PowerUpType> pool)
        {
            List<PowerUpType> shuffled = new(pool);
            Shuffle(shuffled);

            for (int i = 0; i < _reelValues.Length; i++)
            {
                if (i < shuffled.Count)
                {
                    _reelValues[i] = shuffled[i];
                }
                else
                {
                    _reelValues[i] = shuffled[UnityEngine.Random.Range(0, shuffled.Count)];
                }
            }
        }

        private void ShiftLeftAndAppend(PowerUpType newItem)
        {
            for (int i = 0; i < _reelValues.Length - 1; i++)
            {
                _reelValues[i] = _reelValues[i + 1];
            }

            _reelValues[^1] = newItem;
        }

        private void SetFinalLayout(List<PowerUpType> pool, PowerUpType finalItem)
        {
            HashSet<string> usedNames = new() { GetDisplayName(finalItem) };

            for (int i = 0; i < _reelValues.Length; i++)
            {
                PowerUpType value = finalItem;
                if (i != 2 && pool.Count > 0)
                {
                    List<PowerUpType> candidates = new();
                    for (int c = 0; c < pool.Count; c++)
                    {
                        string displayName = GetDisplayName(pool[c]);
                        if (!usedNames.Contains(displayName))
                        {
                            candidates.Add(pool[c]);
                        }
                    }

                    List<PowerUpType> source = candidates.Count > 0 ? candidates : pool;
                    value = source[UnityEngine.Random.Range(0, source.Count)];
                }

                _reelValues[i] = value;
                if (i != 2)
                {
                    usedNames.Add(GetDisplayName(value));
                }
            }

            _reelValues[2] = finalItem;
            RenderReel();
        }

        private void RenderReel()
        {
            for (int i = 0; i < _slotLabels.Length; i++)
            {
                _slotLabels[i].text = GetDisplayName(_reelValues[i]);
            }
        }

        private List<PowerUpType> BuildUniquePool(List<PowerUpType> pool)
        {
            List<PowerUpType> uniquePool = new();
            HashSet<string> usedNames = new();

            for (int i = 0; i < pool.Count; i++)
            {
                string displayName = GetDisplayName(pool[i]);
                if (usedNames.Add(displayName))
                {
                    uniquePool.Add(pool[i]);
                }
            }

            return uniquePool;
        }

        private PowerUpType ChooseUniqueForWindow(List<PowerUpType> pool, int targetIndex = -1, PowerUpType? forcedCenter = null)
        {
            if (pool.Count == 0)
            {
                return default;
            }

            HashSet<string> forbiddenNames = new();
            for (int i = 0; i < _reelValues.Length; i++)
            {
                if (i == targetIndex)
                {
                    continue;
                }

                if (forcedCenter.HasValue && i == 2)
                {
                    forbiddenNames.Add(GetDisplayName(forcedCenter.Value));
                    continue;
                }

                forbiddenNames.Add(GetDisplayName(_reelValues[i]));
            }

            List<PowerUpType> candidates = new();
            for (int i = 0; i < pool.Count; i++)
            {
                if (!forbiddenNames.Contains(GetDisplayName(pool[i])))
                {
                    candidates.Add(pool[i]);
                }
            }

            if (candidates.Count == 0)
            {
                candidates.AddRange(pool);
            }

            return candidates[UnityEngine.Random.Range(0, candidates.Count)];
        }

        private static void Shuffle(List<PowerUpType> values)
        {
            for (int i = values.Count - 1; i > 0; i--)
            {
                int swapIndex = UnityEngine.Random.Range(0, i + 1);
                (values[i], values[swapIndex]) = (values[swapIndex], values[i]);
            }
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
        private void TriggerEffect(PowerUpType type)
        {
            switch (type)
            {
                case PowerUpType.DamageBoost:
                    powerupEffect.DamageBoost();
                    break;

                case PowerUpType.Invincibility:
                    powerupEffect.Invincibility();
                    break;

                case PowerUpType.Onslaught:
                    powerupEffect.Onslaught();
                    break;

                case PowerUpType.Slowness:
                    powerupEffect.Slowness();
                    break;

                case PowerUpType.Saturation:
                    powerupEffect.Saturation();
                    break;

                case PowerUpType.Invisibility:
                    //unim
                    break;
            }
        }
    }

}

