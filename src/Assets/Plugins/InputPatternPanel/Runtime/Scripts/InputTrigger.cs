using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace DreamAnt.IPP
{
    public class InputTrigger : MonoBehaviour
    {
        private Action<GameObject> _triggerAction;

        public void SetAction(Action<GameObject> triggerAction)
        {
            _triggerAction = triggerAction;
        }
        
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("InputPoint") == false)
                return;

            if (_triggerAction != null)
                _triggerAction.Invoke(collision.gameObject);
        }
    }
}
