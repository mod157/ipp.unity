using System;
using UnityEngine;

namespace DreamAnt.IPP
{
    public class InputTrigger : MonoBehaviour
    {
        [SerializeField] private InputPad inputPad;

        private void OnCollisionEnter2D(Collision2D other)
        {
            Debug.Log("Collision - " + other.gameObject.name);
        }

        // Start is called before the first frame update
        private void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log("Trigger - " + collision.name);
            if (collision.CompareTag("InputPoint") == false)
                return;
            
            inputPad.Input(collision.gameObject);
        }
    }
}
