using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement
{
    public delegate void FloatsDelegate(float a, float b);
    //public event FloatsDelegate OnRotation;

    float _currentSpeed, _normalSpeed, _speedOnCast, _turnRate, _jumpStrength, _stepStrength;
    Transform _playerTransform;
    Rigidbody _rb;

    public Movement(Transform transform, Rigidbody rigidbody, float speed, float speedOnCast, float turnRate, float jumpStrength, float stepStrength)
    {
        _playerTransform = transform;
        _rb = rigidbody;
        _currentSpeed = speed;
        _normalSpeed = speed;
        _turnRate = turnRate;
        _speedOnCast = speedOnCast;
        _jumpStrength = jumpStrength;
        _stepStrength = stepStrength;
    }

    /*public void Move(float horizontalInput, float verticalInput, bool changeForward)
    {
        if (horizontalInput == 0 && verticalInput == 0) return;

        var dir = GetDir(horizontalInput, verticalInput);

        if(changeForward)
            _playerTransform.forward = dir;

        _rb.MovePosition(_playerTransform.position + _currentSpeed * Time.fixedDeltaTime * dir);
    }*/

    public void Move(float horizontalInput, float verticalInput, bool changeForward)
    {
        if (horizontalInput == 0 && verticalInput == 0) return;

        var dir = GetDir(horizontalInput, verticalInput);

        if (changeForward)
        {
            if (_playerTransform.forward != dir)
            {
                var eulerRotation = _playerTransform.rotation.eulerAngles;

                var yRotation = Vector3.Angle(_playerTransform.right, dir) < Vector3.Angle(-_playerTransform.right, dir) ? _turnRate : -_turnRate;

                _rb.MoveRotation(Quaternion.Euler(eulerRotation.x, eulerRotation.y + yRotation * Time.fixedDeltaTime, eulerRotation.z));

                var angleToDesired = Vector3.Angle(_playerTransform.forward, dir);
                if (angleToDesired > 20) return;
                else if (angleToDesired < 10) _playerTransform.forward = dir;
            }
        }
            
        _rb.MovePosition(_playerTransform.position + _currentSpeed * Time.fixedDeltaTime * dir);
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
        _rb.AddForce(Vector3.up * _jumpStrength);
    }

    public void Step(float horizontalInput, float verticalInput)
    {
        Vector3 dir, cameraForward;

        if (Mathf.Abs(horizontalInput) < 1 && Mathf.Abs(verticalInput) < 1)
        {
            cameraForward = Camera.main.transform.forward.MakeHorizontal();
            dir = -cameraForward;
        }
        else
        {
            dir = GetDir(horizontalInput, verticalInput, out cameraForward);
        }

        dir.Normalize();

        _rb.velocity = Vector3.zero;
        _rb.AddForce(dir * _stepStrength);
        _playerTransform.forward = cameraForward;
    }

    public bool IsGrounded()
    {
        Ray groundedRay = new Ray(_playerTransform.position, -_playerTransform.up);

        return Physics.Raycast(groundedRay, 1.1f);
    }

    Vector3 GetDir(float hInput, float vInput)
    {
        var cameraForward = Camera.main.transform.forward.MakeHorizontal();
        var cameraRight = Camera.main.transform.right.MakeHorizontal();

        Vector3 direction = cameraForward * vInput + cameraRight * hInput;

        if (direction.sqrMagnitude > 1)
        {
            direction.Normalize();
        }

        return direction;
    }

    Vector3 GetDir(float hInput, float vInput, out Vector3 cameraForward)
    {
        cameraForward = Camera.main.transform.forward.MakeHorizontal();
        var cameraRight = Camera.main.transform.right.MakeHorizontal();

        Vector3 direction = cameraForward * vInput + cameraRight * hInput;

        if (direction.sqrMagnitude > 1)
        {
            direction.Normalize();
        }

        return direction;
    }
}
