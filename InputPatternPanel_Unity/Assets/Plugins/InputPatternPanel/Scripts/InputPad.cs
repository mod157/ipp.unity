using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

namespace DreamAnt.IPP
{
    
    public class InputPad : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerUpHandler, IPointerDownHandler
    {
        [SerializeField] private Camera uiCamera;
        [SerializeField] private GameObject[] commands;
        [SerializeField] private List<Point> pointList;
        [SerializeField] private BoxCollider2D inputCollider;
        [SerializeField] private LineRenderer lineRenderer;

        private RectTransform inputRectTransform;
        private RectTransform rectTransform;

        private List<GameObject> userInput = new List<GameObject>();
        private int dotPerLine = 3;

        void Awake()
        {
            inputCollider.enabled = false;
            inputRectTransform = inputCollider.GetComponent<RectTransform>();
            rectTransform = GetComponent<RectTransform>();
            dotPerLine = (int)Mathf.Sqrt(pointList.Count);
        }

        private void Start()
        {
            //GameManager.instance.TutorialLineAction(StartTutorialLine, EndTutorialLine);
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            Vector2 localPos = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position,
                Camera.main, out localPos);

            localPos.x = Mathf.Clamp(localPos.x, rectTransform.rect.xMin, rectTransform.rect.xMax);
            localPos.y = Mathf.Clamp(localPos.y, rectTransform.rect.yMin, rectTransform.rect.yMax);
            
            inputRectTransform.localPosition = localPos;

            if (userInput.Count > 0)
            {
                lineRenderer.positionCount = userInput.Count + 1;
                lineRenderer.SetPosition(userInput.Count, inputCollider.transform.position);
            }
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            //inputCollider.enabled = true;
            //userInput.Clear();
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            //inputCollider.enabled = false;
            //OnInputComplete();
        }

        public void Input(GameObject obj)
        {
            if (userInput.Count <= 0)
            {
                userInput.Add(obj);
                lineRenderer.positionCount = userInput.Count;

                lineRenderer.SetPosition(userInput.Count - 1, obj.transform.position);
                //SoundManager.Instance.PlaySound(SoundManager.SoundType.On_Point);
                return;
            }

            if (userInput.Find(o => obj.Equals(o)) == null)
            {
                var lastInputObject = userInput.Last();
                var lastInputIndex = pointList.FindIndex(o => lastInputObject.Equals(o.obj));
                var currentObjectIndex = pointList.FindIndex(o => obj.Equals(o.obj));

                List<int> betweenIndexList = GetBetweenObjectList(lastInputIndex, currentObjectIndex);
                foreach (int idx in betweenIndexList)
                {
                    if (userInput.Find(o => pointList[idx].obj.Equals(o)) != null)
                        continue;

                    userInput.Add(pointList[idx].obj);
                    lineRenderer.positionCount = userInput.Count;
                    lineRenderer.SetPosition(userInput.Count - 1, pointList[idx].obj.transform.position);
                }

                userInput.Add(obj);
                lineRenderer.positionCount = userInput.Count;

                lineRenderer.SetPosition(userInput.Count - 1, obj.transform.position);
               // SoundManager.Instance.PlaySound(SoundManager.SoundType.On_Point);
            }
        }

        private List<int> GetBetweenObjectList(int beginIndex, int endIndex)
        {
            List<int> retList = new List<int>();

            Vector2Int begin = new Vector2Int(beginIndex % this.dotPerLine, (int)beginIndex / (int)this.dotPerLine);
            Vector2Int end = new Vector2Int(endIndex % this.dotPerLine, (int)endIndex / (int)this.dotPerLine);

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

                    int index = temp * this.dotPerLine + begin.x;
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

                    int index = begin.y * this.dotPerLine + temp;
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

                        int index = tempY * this.dotPerLine + tempX;
                        retList.Add(index);
                    }
                }
            }

            return retList;
        }

        public void OnInputComplete()
        {
            string inputString = string.Empty;
            if (userInput.Count <= 0)
                return;

            foreach (var obj in userInput)
            {
                var inputPoint = pointList.Find(point => point.obj.Equals(obj));
                inputString += inputPoint.value;
            }

           /* if (GameManager.instance != null)
            {
                GameManager.instance.SetPatten(inputString);
            }*/

            this.lineRenderer.positionCount = 0;
            //this.lineRenderer.gameObject.SetActive(false);
            userInput.Clear();
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            inputCollider.enabled = false;
            OnInputComplete();
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            Vector2 localPos = Vector2.zero;
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position,
                Camera.main, out localPos);
            
            inputRectTransform.localPosition = localPos;

            inputCollider.enabled = true;
            userInput.Clear();
        }
        
        public void StartTutorialLine(string itemName)
        {
            List<Transform> tutorialCommand = new List<Transform>();
            switch (itemName)
            {
                case "Jump":
                    tutorialCommand.Add(commands[1].transform);
                    tutorialCommand.Add(commands[4].transform);
                    tutorialCommand.Add(commands[7].transform);
                    break;
                case "Dash":

                    tutorialCommand.Add(commands[0].transform);
                    tutorialCommand.Add(commands[1].transform);
                    tutorialCommand.Add(commands[2].transform);
                    break;
                case "Sit":

                    tutorialCommand.Add(commands[7].transform);
                    tutorialCommand.Add(commands[4].transform);
                    tutorialCommand.Add(commands[1].transform);
                    break;
                case "Attack":

                    tutorialCommand.Add(commands[0].transform);
                    tutorialCommand.Add(commands[4].transform);
                    tutorialCommand.Add(commands[8].transform);
                    break;
                case "Bow":

                    tutorialCommand.Add(commands[6].transform);
                    tutorialCommand.Add(commands[4].transform);
                    tutorialCommand.Add(commands[2].transform);

                    break;
            }

            lineRenderer.positionCount = tutorialCommand.Count;
            
            for (int i = 0; i < tutorialCommand.Count; i++)
            {
                lineRenderer.SetPosition(i, tutorialCommand[i].position);
            }
            // GameObject.Find("TutorialLine").GetComponent<LineController>().SetUpLine(tutorialCommand.ToArray());
        }

        public void EndTutorialLine()
        {
            lineRenderer.positionCount = 0;
        }
    }
    
    [System.Serializable]
    public struct Point
    {
        public GameObject obj;
        public string value;
    }
}
