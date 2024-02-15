using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMove : MonoBehaviour
{
    private readonly float _speed = 1.0F;

    private Rigidbody _rigidbody;
    private Queue<Vector3> _moveValueQueue;
    private Vector3 _currentPosition;
    
    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _moveValueQueue = new Queue<Vector3>();
    }
    
    void Start()
    {
        StartCoroutine(MoveEvent());
    }

    public void Move(int leftandrightValue, int upanddownValue)
    {
        if (Mathf.Abs(leftandrightValue) > 0)
        {
            _moveValueQueue.Enqueue(new Vector3(leftandrightValue, 0f, 0f));
        }

        if (Mathf.Abs(upanddownValue) > 0)
        {
            _moveValueQueue.Enqueue(new Vector3(0f, 0f, upanddownValue));
        }
    }


    private void FixedUpdate()
    {
        //Reset
        if (transform.localPosition.y < -0.5f)
        {
            transform.localPosition = new Vector3(0f, 0.5f, -4f);
        }
        if (transform.localPosition.y > 1f)
        {
            Debug.Log("Goal!");
            transform.localPosition = new Vector3(0f, 0.5f, -4f);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        transform.localPosition += new Vector3(0f, 0.3f, 0f);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    IEnumerator MoveEvent()
    {
        while (true)
        {
            if (_moveValueQueue.Count > 0 && _moveValueQueue.TryDequeue(out Vector3 result))
            {
                _rigidbody.velocity = result;
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
