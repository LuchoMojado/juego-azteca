using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public delegate void FloatsDelegate(float a, float b);
    public event FloatsDelegate OnRotation;

    float _currentSpeed, _normalSpeed, _sprintSpeed, _jumpStrength, _stepStrength;
    Transform _playerTransform;
    Rigidbody _myRB;

    public Movement(Transform transform, Rigidbody rigidbody, float speed, float jumpStrength, float stepStrength)
    {
        _playerTransform = transform;
        _myRB = rigidbody;
        _currentSpeed = speed;
        _normalSpeed = speed;
        _sprintSpeed = speed * 1.5f;
        _jumpStrength = jumpStrength;
        _stepStrength = stepStrength;
    }

    public void Move(float horizontalInput, float verticalInput)
    {
        if (horizontalInput == 0 && verticalInput == 0) return;

        var dir = GetDir(horizontalInput, verticalInput);

        _playerTransform.forward = dir;
        _myRB.MovePosition(_playerTransform.position + _currentSpeed * Time.fixedDeltaTime * dir);
    }

    /*public void Sprint()
    {
        _currentSpeed = _sprintSpeed;
    }

    public void StopSprint()
    {
        _currentSpeed = _normalSpeed;
    }*/

    public void Jump()
    {
        _myRB.AddForce(Vector3.up * _jumpStrength);
    }

    public void Step(float horizontalInput, float verticalInput)
    {
        Vector3 dir, cameraForward;

        if (horizontalInput == 0 && verticalInput == 0)
        {
            cameraForward = Camera.main.transform.forward;
            cameraForward.MakeHorizontal();
            dir = -cameraForward;
            Debug.Log(dir.y);
        }
        else
        {
            dir = GetDir(horizontalInput, verticalInput, out cameraForward);
        }
        
        _myRB.AddForce(dir * _stepStrength);
        _playerTransform.forward = cameraForward;
    }

    public bool IsGrounded()
    {
        Ray groundedRay = new Ray(_playerTransform.position, -_playerTransform.up);

        return Physics.Raycast(groundedRay, 1.1f);
    }

    Vector3 GetDir(float hInput, float vInput)
    {
        var cameraForward = Camera.main.transform.forward;
        var cameraRight = Camera.main.transform.right;
        cameraForward.y = 0;
        cameraRight.y = 0;

        Vector3 direction = cameraForward * vInput + cameraRight * hInput;

        if (direction.sqrMagnitude > 1)
        {
            direction.Normalize();
        }

        return direction;
    }

    Vector3 GetDir(float hInput, float vInput, out Vector3 cameraForward)
    {
        cameraForward = Camera.main.transform.forward;
        var cameraRight = Camera.main.transform.right;
        cameraRight.MakeHorizontal();
        cameraForward.MakeHorizontal();


        Vector3 direction = cameraForward * vInput + cameraRight * hInput;

        if (direction.sqrMagnitude > 1)
        {
            direction.Normalize();
        }

        return direction;
    }
}
