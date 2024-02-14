using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DreamAnt.IPP
{
    public class PadButton : MonoBehaviour
    {
        private string _uid;
        private string _value;
        private LineRenderer _renderer;
        private CircleCollider2D _collider2D;
        private void Awake()
        {
            _renderer = gameObject.GetComponentInChildren<LineRenderer>(true);
            _collider2D = GetComponent<CircleCollider2D>();
        }

        public void Initialized(int radius, string value)
        {
             _uid = Guid.NewGuid().ToString();
             _collider2D.radius = radius;
             _value = value;
        }

        public void DisplayInputPadCollider(bool isRadius)
        {
            if (!isRadius)
            { 
                _renderer.gameObject.SetActive(false);
                return;
            }
            
            Vector3[] drawPos = new Vector3[360];
            float x1, y1;
            for (int i = 0; i < 360; i ++)
            {
                x1 = Mathf.Cos(Mathf.Deg2Rad * i) * _collider2D.radius;
                y1 = Mathf.Sin(Mathf.Deg2Rad * i) * _collider2D.radius;
                
                drawPos[i] = new Vector3(x1, y1, 0);
            }
            
            _renderer.positionCount = 360;
            _renderer.SetPositions(drawPos);
            
            _renderer.gameObject.SetActive(true);
        }

        public string Uid
        {
            get => _uid;
        }

        public string Value
        {
            get => _value;
        }
    }
}
