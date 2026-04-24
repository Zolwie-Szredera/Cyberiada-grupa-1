using UnityEngine;
using UnityEngine.UIElements;

namespace SecretLevel
{
    [RequireComponent(typeof(UIDocument))]
    public class SecretDeathUi : MonoBehaviour
    {
        [SerializeField] private string deathRootName = "DeathRoot";
        [SerializeField] private string exitButtonName = "ExitButton";

        private UIDocument _uiDocument;
        private VisualElement _deathRoot;
        private VisualElement _exitButton;

        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
            SetVisible(false);
        }

        private void OnEnable()
        {
            CacheRoot();
            CacheButtons();
            RegisterButtons();
            SetVisible(false);
        }

        private void OnDisable()
        {
            UnregisterButtons();
        }

        private void Start()
        {
            CacheRoot();
            SetVisible(false);
        }

        public void ShowDeathScreen()
        {
            Time.timeScale = 0f;
            CacheRoot();
            CacheButtons();
            RegisterButtons();
            SetVisible(true);
        }

        public void HideDeathScreen()
        {
            Time.timeScale = 1f;
            UnregisterButtons();
            SetVisible(false);
        }

        private void RegisterButtons()
        {
            if (_exitButton != null)
            {
                _exitButton.UnregisterCallback<ClickEvent>(OnExitClicked);
                _exitButton.RegisterCallback<ClickEvent>(OnExitClicked);
            }
        }

        private void UnregisterButtons()
        {
            if (_exitButton != null)
            {
                _exitButton.UnregisterCallback<ClickEvent>(OnExitClicked);
            }
        }

        private void CacheButtons()
        {
            if (_uiDocument == null)
            {
                _uiDocument = GetComponent<UIDocument>();
            }

            if (_uiDocument == null || _uiDocument.rootVisualElement == null)
            {
                _exitButton = null;
                return;
            }

            _exitButton = _uiDocument.rootVisualElement.Q(exitButtonName);
        }

        private void CacheRoot()
        {
            if (_uiDocument == null)
            {
                _uiDocument = GetComponent<UIDocument>();
            }

            if (_uiDocument == null || _uiDocument.rootVisualElement == null)
            {
                _deathRoot = null;
                return;
            }

            _deathRoot = _uiDocument.rootVisualElement.Q<VisualElement>(deathRootName);
            if (_deathRoot == null)
            {
                // Fallback to whole document root if named element is not set.
                _deathRoot = _uiDocument.rootVisualElement;
            }
        }

        private void SetVisible(bool isVisible)
        {
            if (_deathRoot == null)
            {
                CacheRoot();
            }

            if (_deathRoot == null)
            {
                return;
            }

            _deathRoot.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void OnExitClicked(ClickEvent evt)
        {
            Time.timeScale = 1f;
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}






