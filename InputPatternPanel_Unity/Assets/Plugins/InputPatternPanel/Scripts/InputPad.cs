using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

namespace DreamAnt.IPP
{
    public class InputPad : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerUpHandler, IPointerDownHandler
    {
        [SerializeField] private InputTrigger inputTrigger;
        [SerializeField] private GameObject[] commands;
        [SerializeField] private List<Point> pointList;
        [SerializeField] private BoxCollider2D inputCollider;
        [SerializeField] private LineRenderer lineRenderer;

        private RectTransform _inputRectTransform;
        private RectTransform _rectTransform;

        private List<GameObject> _userInput;
        private int _dotPerLine = 3;

        void Awake()
        {
            inputCollider.enabled = false;
            _inputRectTransform = inputCollider.GetComponent<RectTransform>();
            _userInput = new List<GameObject>();
            _rectTransform = GetComponent<RectTransform>();
            _dotPerLine = (int)Mathf.Sqrt(pointList.Count);
            
            inputTrigger.SetAction(Input);
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            Vector2 localPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, eventData.position,
                Camera.main, out localPos);

            localPos.x = Mathf.Clamp(localPos.x, _rectTransform.rect.xMin, _rectTransform.rect.xMax);
            localPos.y = Mathf.Clamp(localPos.y, _rectTransform.rect.yMin, _rectTransform.rect.yMax);
            
            _inputRectTransform.localPosition = localPos;

            if (_userInput.Count > 0)
            {
                lineRenderer.positionCount = _userInput.Count + 1;
                lineRenderer.SetPosition(_userInput.Count, inputCollider.transform.position);
            }
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            /*inputCollider.enabled = true;
            _userInput.Clear();*/
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
           /* inputCollider.enabled = false;
            OnInputComplete();*/
        }

        public void Input(GameObject obj)
        {
            Debug.Log("Input Obj Name - " + obj.name);
            if (_userInput.Count <= 0)
            {
                _userInput.Add(obj);
                lineRenderer.positionCount = _userInput.Count;

                lineRenderer.SetPosition(_userInput.Count - 1, obj.transform.position);
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
                    lineRenderer.positionCount = _userInput.Count;
                    lineRenderer.SetPosition(_userInput.Count - 1, pointList[idx].obj.transform.position);
                }

                _userInput.Add(obj);
                lineRenderer.positionCount = _userInput.Count;

                lineRenderer.SetPosition(_userInput.Count - 1, obj.transform.position);
            }
        }

       

        public string OnInputComplete()
        {
            Debug.Log("InputComplete");
            string inputString = string.Empty;
            
            if (_userInput.Count <= 0)
                return string.Empty;

            foreach (var obj in _userInput)
            {
                var inputPoint = pointList.Find(point => point.obj.Equals(obj));
                inputString += inputPoint.value;
            }

           /* if (GameManager.instance != null)
            {
                GameManager.instance.SetPatten(inputString);
            }*/

           return inputString;
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            inputCollider.enabled = false;
            string inputString = OnInputComplete();
            
            Debug.Log(inputString);
            
            lineRenderer.positionCount = 0;
            lineRenderer.gameObject.SetActive(false);
            _userInput.Clear();
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, eventData.position,
                Camera.main, out var localPos);
            
            _inputRectTransform.localPosition = localPos;

            inputCollider.enabled = true;
            _userInput.Clear();
        }
        
        public void EndTutorialLine()
        {
            lineRenderer.positionCount = 0;
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
