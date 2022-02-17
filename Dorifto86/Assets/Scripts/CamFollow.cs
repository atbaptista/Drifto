using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    public Transform CamTarget;
    public float Speed = 5;
    public Vector3 Dist;
    public Transform LookTarget;

    private void FixedUpdate() {
        Vector3 dPos = CamTarget.position + Dist;
        transform.position = Vector3.Lerp(transform.position, dPos, Speed * Time.deltaTime);
        transform.LookAt(LookTarget.position);
    }
}
