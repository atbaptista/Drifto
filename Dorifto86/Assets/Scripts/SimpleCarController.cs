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
    public TrailRenderer[] TireMarks;

    [Range(-1.5f, 1.5f)]
    public float ZCenter;

    [SerializeField]
    private float _skidAt = 0.35f;
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
    private bool _isSkid = false;

    private void Start() {
        _rb = GetComponent<Rigidbody>();
        _centerOfMass.z = ZCenter;
        _rb.centerOfMass = _centerOfMass;
    }
    private void Update() {
        AnimateWheels();
        GetInputs();
    }
    private void FixedUpdate() {
        Move();
        Turn();
        Skidding();
    }

    private void Move() {
        bool isBraking = false;
        if (Input.GetKey(KeyCode.Space)) {
            isBraking = true;
        }

        foreach (var wheel in _wheels) {
            if (!isBraking) {
                wheel.collider.brakeTorque = 0;
                wheel.collider.motorTorque = _inputY * _maxAcceleration * 125 * Time.deltaTime;
            } else if (isBraking){ //handbrake
                if (wheel.axel == Axel.REAR) {
                    wheel.collider.brakeTorque = 4000;
                }
            }            
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

    private void Skidding() {
        foreach (var wheel in _wheels) {
            WheelHit hit;
            wheel.collider.GetGroundHit(out hit);

            if (hit.sidewaysSlip > _skidAt || hit.forwardSlip > _skidAt) {
                StartEmitter();
                Debug.Log("skrrrt! " + Time.time);
            } else {
                StopEmitter();
            }
        }
    }

    private void StartEmitter() {
        if (_isSkid) return;
        foreach (TrailRenderer T in TireMarks) {
            T.emitting = true;
        }

        _isSkid = true; 
    }

    private void StopEmitter() {
        if (!_isSkid) return;
        foreach (TrailRenderer T in TireMarks) {
            T.emitting = false;
        }

        _isSkid = false;
    }
}