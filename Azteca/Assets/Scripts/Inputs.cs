using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inputs
{
    public System.Action inputUpdate;
    float _inputHorizontal, _inputVertical;
    float _inputMouseX, _inputMouseY;
    Movement _movement;
    PlayerController _player;
    bool _jump;
    bool _attack = false;

    public bool Attack
    {
        get
        {
            return _attack;
        }

        set
        {
            if (value == !_attack)
            {
                if (value)
                {
                    _attack = value;
                    inputUpdate = Casting;
                    _player.ActivateSunMagic();
                }
                else
                {
                    _attack = value;
                    inputUpdate = Unpaused;
                }
            }
        }
    }

    public Inputs(Movement movement, PlayerController player)
    {
        _movement = movement;
        _player = player;
    }

    public void Unpaused()
    {
        _inputMouseX = Input.GetAxisRaw("Mouse X");

        _inputMouseY = Input.GetAxisRaw("Mouse Y");

        _inputHorizontal = Input.GetAxis("Horizontal");

        _inputVertical = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            //UIManager.instance.SetPauseMenu(true);
            inputUpdate = Paused;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _player.Step(_inputHorizontal, _inputVertical);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _jump = true;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Attack = true;
        }
    }

    public void Stepping()
    {
        _inputHorizontal = 0;

        _inputVertical = 0;

        _inputMouseX = Input.GetAxisRaw("Mouse X");

        _inputMouseY = Input.GetAxisRaw("Mouse Y");

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            //UIManager.instance.SetPauseMenu(true);
            inputUpdate = Paused;
        }
    }

    public void Casting()
    {
        _inputHorizontal = Input.GetAxis("Horizontal");

        _inputVertical = Input.GetAxis("Vertical");

        _inputMouseX = Input.GetAxisRaw("Mouse X");

        _inputMouseY = Input.GetAxisRaw("Mouse Y");

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            //UIManager.instance.SetPauseMenu(true);
            inputUpdate = Paused;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _player.Step(_inputHorizontal, _inputVertical);
            Attack = false;
            inputUpdate = Unpaused;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _jump = true;
            Attack = false;
            inputUpdate = Unpaused;
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Attack = false;
            inputUpdate = Unpaused;
        }
    }

    public void Paused()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            //UIManager.instance.SetPauseMenu(false);
            inputUpdate = Unpaused;
        }
    }

    public void InputsFixedUpdate()
    {
        _movement.Move(_inputHorizontal, _inputVertical, !Attack);

        if (_jump)
        {
            _player.Jump();
            _jump = false;
        }
    }
}
