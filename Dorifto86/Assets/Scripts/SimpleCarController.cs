using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum Axel {
    FRONT, REAR
}

[Serializable]
public struct Wheel {
    public GameObject model;
    public WheelCollider collider;
    public Axel axel;
}

public class SimpleCarController : MonoBehaviour {
    [SerializeField]
    private float _maxAcceleration = 20f;
    [SerializeField]
    private float _turnSensitivity = 1f;
    [SerializeField]
    private float _maxSteerAngle = 45f;
    [SerializeField]
    private Vector3 _centerOfMass;
    [SerializeField]
    private List<Wheel> _wheels;

    private float _inputX, _inputY;

    private Rigidbody _rb;

    private void Start() {
        _rb = GetComponent<Rigidbody>();
        _rb.centerOfMass = _centerOfMass;
    }
    private void Update() {
        AnimateWheels();
        GetInputs();
    }
    private void FixedUpdate() {
        Move();
        Turn();
    }

    private void Move() {
        foreach (var wheel in _wheels) {
            //if (wheel.axel == Axel.REAR) {
                Debug.Log(_inputY);
                wheel.collider.motorTorque = _inputY * _maxAcceleration * 125 * Time.deltaTime;
            //}
                
        }
    }

    private void GetInputs() {
        _inputX = Input.GetAxis("Horizontal");
        _inputY = Input.GetAxis("Vertical");
    }

    private void Turn() {
        foreach (var wheel in _wheels) {
            if (wheel.axel == Axel.FRONT) {
                var steerAngle = _inputX * _turnSensitivity * _maxSteerAngle;
                wheel.collider.steerAngle = Mathf.Lerp(wheel.collider.steerAngle, steerAngle, 0.5f);
            }
        }
    }

    private void AnimateWheels() {
        foreach (var wheel in _wheels) {
            Quaternion rot;
            Vector3 pos;
            wheel.collider.GetWorldPose(out pos, out rot);
            wheel.model.transform.position = pos;
            wheel.model.transform.rotation = rot;
        }
    }
}