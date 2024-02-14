using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

namespace DreamAnt.IPP
{
    public class InputPad : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
    {
        [Header("Component")] 
        [SerializeField] private InputTrigger inputTrigger;
        [SerializeField] private List<Point> pointList;
        [SerializeField] private BoxCollider2D inputCollider;
        [SerializeField] private LineRenderer inputLineRenderer;

        [Space(20)]
        [Header("Option")] 
        [SerializeField] private bool isInputLine = true;

        private Canvas _padCanvas;
        private Camera _padCamera;
        private RectTransform _inputRectTransform;
        private RectTransform _rectTransform;
        private List<GameObject> _userInput;
        private int _dotPerLine = 3;

        public Action<String> resultAction;

        void Awake()
        {
            inputCollider.enabled = false;
            _inputRectTransform = inputCollider.GetComponent<RectTransform>();
            _userInput = new List<GameObject>();
            _rectTransform = GetComponent<RectTransform>();
            _dotPerLine = (int)Mathf.Sqrt(pointList.Count);
            _padCanvas = GetComponentInParent<Canvas>();
            InitUICanvas();
            
            inputTrigger.SetAction(Input);
        }

        private void InitUICanvas()
        {
            _padCanvas.renderMode = RenderMode.ScreenSpaceCamera;

            if (_padCanvas.worldCamera == null)
                _padCanvas.worldCamera = Camera.main;
            
            if (_padCamera == null || _padCamera != _padCanvas.worldCamera)
                _padCamera = _padCanvas.worldCamera;
        }
        
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            _inputRectTransform.localPosition = GetPointerPosition(eventData.position);

            inputCollider.enabled = true;
            _userInput.Clear();
        }
        
        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            //Result PointerData - String
            if(resultAction != null)
                resultAction.Invoke(OnInputComplete());
            
            inputCollider.enabled = false;
            _userInput.Clear();
            
            /*
            if (isInputLine)
            {
                lineRenderer.positionCount = 0;
                lineRenderer.gameObject.SetActive(false);
            }*/
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            _inputRectTransform.localPosition = GetPointerPosition(eventData.position);

            if (isInputLine && _userInput.Count > 0)
            {
                OnAddLine(inputCollider.transform.position);
            }
        }
        
        public void Input(GameObject obj)
        {
            if (_userInput.Count == 0)
            {
                _userInput.Add(obj);
                
                if (isInputLine)
                {
                    OnAddLine(obj.transform.position);
                }

                return;
            }

            if (_userInput.Find(o => obj.Equals(o)) == null)
            {
                var lastInputObject = _userInput.Last();
                var lastInputIndex = pointList.FindIndex(o => lastInputObject.Equals(o.obj));
                var currentObjectIndex = pointList.FindIndex(o => obj.Equals(o.obj));

                List<int> betweenIndexList = GetBetweenObjectList(lastInputIndex, currentObjectIndex);
                
                foreach (int idx in betweenIndexList)
                {
                    if (_userInput.Find(o => pointList[idx].obj.Equals(o)) != null)
                        continue;

                    _userInput.Add(pointList[idx].obj);
                    OnAddLine(pointList[idx].obj.transform.position);
                }

                _userInput.Add(obj);
                OnAddLine(obj.transform.position);
            }
        }

        private string OnInputComplete()
        {
            if (_userInput.Count <= 0)
                return string.Empty;
            
            string inputString = string.Empty;
            
            foreach (var obj in _userInput)
            {
                var inputPoint = pointList.Find(point => point.obj.Equals(obj));
                inputString += inputPoint.value;
            }
            
           return inputString;
        }

        private void OnAddLine(Vector3 position)
        {
            inputLineRenderer.positionCount = _userInput.Count;
            inputLineRenderer.SetPosition(_userInput.Count - 1, position);
        }

        private Vector2 GetPointerPosition(Vector2 eventPosition)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, eventPosition,
                _padCamera, out Vector2 localPos);

            localPos.x = Mathf.Clamp(localPos.x, _rectTransform.rect.xMin, _rectTransform.rect.xMax);
            localPos.y = Mathf.Clamp(localPos.y, _rectTransform.rect.yMin, _rectTransform.rect.yMax);
            
            _inputRectTransform.localPosition = localPos;
            
            return localPos;
        }
        
        private List<int> GetBetweenObjectList(int beginIndex, int endIndex)
        {
            List<int> retList = new List<int>();

            Vector2Int begin = new Vector2Int(beginIndex % _dotPerLine, beginIndex / _dotPerLine);
            Vector2Int end = new Vector2Int(endIndex % _dotPerLine, endIndex / _dotPerLine);

            Vector2Int diff = end - begin;
            if (diff.x == 0 && diff.y == 0) //���� ��ġ(���ø� ����)
            {

            }
            else if (diff.x == 0 && diff.y != 0) // ���η� �߰��� üũ
            {
                int moveValue = diff.y > 0 ? 1 : -1;
                int temp = begin.y;
                while (true)
                {
                    temp += moveValue;
                    if (temp == end.y)
                    {
                        break;
                    }

                    int index = temp * _dotPerLine + begin.x;
                    retList.Add(index);
                }
            }
            else if (diff.x != 0 && diff.y == 0) // ���η� �߰��� üũ
            {
                int moveValue = diff.x > 0 ? 1 : -1;
                int temp = begin.x;
                while (true)
                {
                    temp += moveValue;
                    if (temp == end.x)
                    {
                        break;
                    }

                    int index = begin.y * _dotPerLine + temp;
                    retList.Add(index);
                }
            }
            else
            {
                if (Mathf.Abs(diff.x) == Mathf.Abs(diff.y)) // �밢�� �߰��� üũ
                {
                    int moveXValue = diff.x > 0 ? 1 : -1;
                    int moveYValue = diff.y > 0 ? 1 : -1;
                    int tempX = begin.x;
                    int tempY = begin.y;
                    while (true)
                    {
                        tempX += moveXValue;
                        tempY += moveYValue;
                        if (end.x == tempX && end.y == tempY)
                        {
                            break;
                        }

                        int index = tempY * _dotPerLine + tempX;
                        retList.Add(index);
                    }
                }
            }

            return retList;
        }
    }
    
    [System.Serializable]
    public struct Point
    {
        public GameObject obj;
        public string value;
    }
}
