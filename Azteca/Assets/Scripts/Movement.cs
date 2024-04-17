using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement
{
    public delegate void FloatsDelegate(float a, float b);
    //public event FloatsDelegate OnRotation;

    float _currentSpeed, _normalSpeed, _speedOnCast, _jumpStrength, _stepStrength;
    Transform _playerTransform;
    Rigidbody _myRB;

    public Movement(Transform transform, Rigidbody rigidbody, float speed, float speedOnCast, float jumpStrength, float stepStrength)
    {
        _playerTransform = transform;
        _myRB = rigidbody;
        _currentSpeed = speed;
        _normalSpeed = speed;
        _speedOnCast = speedOnCast;
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

    public void Casting()
    {
        _currentSpeed = _speedOnCast;
    }

    public void StopCasting()
    {
        _currentSpeed = _normalSpeed;
    }

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
        }
        else
        {
            dir = GetDir(horizontalInput, verticalInput, out cameraForward);
        }

        dir.Normalize();

        _myRB.velocity = Vector3.zero;
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
        cameraForward.MakeHorizontal();
        cameraRight.MakeHorizontal();

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
