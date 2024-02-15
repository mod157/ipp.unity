using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace DreamAnt.IPP
{
    public class InputPad : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
    {
        [Header("Component")] 
        [SerializeField] private BoxCollider2D inputCollider;
        [SerializeField] private LineRenderer inputLineRenderer;
        [SerializeField] private List<PadButton> padButtonList;
        
        [Header("InputPad")]
        [SerializeField] private GameObject buttonObject;
        [SerializeField] private Vector2Int padCount;
        [Range(0.5f, 2f)]
        [SerializeField] private float scaleSlider = 1f;
        [SerializeField] private int radius;
        [SerializeField] private GridLayoutGroup padGridLayoutGroup;
        [SerializeField] private InputTrigger inputTrigger;
        
        
        [Space(10)]
        [Header("Option")] 
        [SerializeField] private bool isInputLine = true;
        [SerializeField] private bool isPointer = false;
        [SerializeField] private bool isPadButtonRadius = false;
        [SerializeField] private bool isEditorDebug = true;

        private Canvas _padCanvas;
        private CanvasScaler _padCanvasScaler;
        private Camera _padCamera;
        private RectTransform _inputRectTransform;
        private RectTransform _rectTransform;
        private Dictionary<string, PadButton> _userInput;
        private Vector2 _pixelSize;
        
        public Action<string> resultAction;

        void Awake()
        {
            inputCollider.enabled = false;
            _inputRectTransform = inputCollider.GetComponent<RectTransform>();
            _userInput = new Dictionary<string, PadButton>();
            _rectTransform = GetComponent<RectTransform>();
            //_dotPerLine = (int)Mathf.Sqrt(padButtonList.Count);
            _padCanvas = GetComponentInParent<Canvas>();
            _padCanvasScaler = GetComponentInParent<CanvasScaler>();
            
            InitUICanvas();
            InitPadButton();
            
            inputTrigger.SetAction(Input);
        }

        private void InitUICanvas()
        {
            _padCanvas.renderMode = RenderMode.ScreenSpaceCamera;

            if (_padCanvas.worldCamera == null)
                _padCanvas.worldCamera = Camera.main;
            
            if (_padCamera == null || _padCamera != _padCanvas.worldCamera)
                _padCamera = _padCanvas.worldCamera;
            
            float wRatio = Screen.width  / _padCanvasScaler.referenceResolution.x;
            float hRatio = Screen.height / _padCanvasScaler.referenceResolution.y;

            // 결과 비율값
            float ratio =
                wRatio * (1f - _padCanvasScaler.matchWidthOrHeight) +
                hRatio * (_padCanvasScaler.matchWidthOrHeight);

            // 현재 스크린에서 RectTransform의 실제 너비, 높이
            var rect = _rectTransform.rect;
            float pixelWidth  = rect.width  * ratio;
            float pixelHeight = rect.height * ratio;

            _pixelSize = new Vector2(pixelWidth, pixelHeight);

            if (isPointer)
            {
                inputCollider.GetComponent<Image>().enabled = true;
            }
            else
            {
                inputCollider.GetComponent<Image>().enabled = false;
            }
        }

        private void InitPadButton()
        {
            if(padCount.x == 0 || padCount.y == 0)
                Debug.LogError("padSize Zero Error");

            int cellX = Mathf.RoundToInt(_pixelSize.x / padCount.x * scaleSlider);
            int cellY = Mathf.RoundToInt(_pixelSize.y / padCount.y * scaleSlider);
            
            padGridLayoutGroup.cellSize = new Vector2(cellX, cellY);
            //padGridLayoutGroup.spacing = new Vector2Int(cellX + radius, cellY + radius);
            padGridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            padGridLayoutGroup.constraintCount = padCount.x;

            int totalCount = padCount.x * padCount.y;

            for (int i = 0; i < totalCount; i++)
            {
                PadButton buttonPad = Instantiate(buttonObject, padGridLayoutGroup.transform).GetComponent<PadButton>();
                buttonPad.Initialized(radius,((char)('A' + i)).ToString());
                buttonPad.DisplayInputPadCollider(isPadButtonRadius);
                padButtonList.Add(buttonPad);
            }
        }
        
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            _inputRectTransform.localPosition = GetPointerPosition(eventData.position);
            inputCollider.enabled = true;
        }
        
        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            //Result PointerData - String
#if UNITY_EDITOR
            if(isEditorDebug)
                Debug.Log($"OnInputComplete - [{OnInputComplete()}]");
#endif
            
            if(resultAction != null)
                resultAction.Invoke(OnInputComplete());
            
            inputCollider.enabled = false;
            _userInput.Clear();

            inputLineRenderer.positionCount = 0;
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            _inputRectTransform.localPosition = GetPointerPosition(eventData.position);
            OnAddLine(inputCollider.transform.position);
        }
        
        public void Input(GameObject obj)
        {
            PadButton inputButton = padButtonList.Find(o => obj.Equals(o.gameObject));
            if (!_userInput.TryGetValue(inputButton.Uid, out PadButton result))
            {
                _userInput.Add(inputButton.Uid, inputButton);
                OnAddLine(_userInput.Count - 1, inputButton.transform.position);
            }
        }

        private string OnInputComplete()
        {
            if (_userInput.Count <= 0)
                return string.Empty;
            
            string inputString = string.Empty;
            
            foreach (var padValue in _userInput.Values)
            {
                inputString += padValue.Value;
            }
            
           return inputString;
        }

        private void OnAddLine(Vector3 position)
        {
            if (!isInputLine)
                return;
            
            inputLineRenderer.positionCount = _userInput.Count + 1;
            inputLineRenderer.SetPosition(_userInput.Count, position);
        }
        private void OnAddLine(int index, Vector3 position)
        {
            if (!isInputLine)
                return;
            
            inputLineRenderer.positionCount = _userInput.Count + 1;
            inputLineRenderer.SetPosition(index, position);
            inputLineRenderer.SetPosition(index + 1, position);
        }

        private Vector2 GetPointerPosition(Vector2 eventPosition)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, eventPosition,
                _padCamera, out Vector2 localPos);

            var rect = _rectTransform.rect;
            localPos.x = Mathf.Clamp(localPos.x, rect.xMin, rect.xMax);
            localPos.y = Mathf.Clamp(localPos.y, rect.yMin, rect.yMax);
            
            _inputRectTransform.localPosition = localPos;
            
            return localPos;
        }
    }
}
