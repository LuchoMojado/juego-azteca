using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inputs
{
    public System.Action inputUpdate, inputLateUpdate;
    float _inputHorizontal, _inputVertical;
    float _inputMouseX, _inputMouseY;
    Movement _movement;
    PlayerController _player;
    CameraController _camera;
    bool _jump, _attack = false, _locked = false;

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
                    _player.ActivateMagic();
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
        _camera = Camera.main.GetComponentInParent<CameraController>();
        inputLateUpdate = FreeLook;
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

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            ToggleLock();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _player.ChangeActiveMagic(PlayerController.MagicType.Sun);
            _player.renderer.material.color = Color.red;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _player.ChangeActiveMagic(PlayerController.MagicType.Obsidian);
            _player.renderer.material.color = Color.black;
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

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _player.ChangeActiveMagic(PlayerController.MagicType.Sun);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _player.ChangeActiveMagic(PlayerController.MagicType.Obsidian);
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            ToggleLock();
        }
    }

    public void FixedCast()
    {
        _inputHorizontal = 0;

        _inputVertical = 0;

        _inputMouseX = Input.GetAxisRaw("Mouse X");

        _inputMouseY = Input.GetAxisRaw("Mouse Y");

        if (Mathf.Abs(Input.GetAxis("Horizontal")) == 1 || Mathf.Abs(Input.GetAxis("Vertical")) == 1)
        {
            Attack = false;
            inputUpdate = Unpaused;
        }

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

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            ToggleLock();
        }
    }

    public void MovingCast()
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

        /*if (Input.GetKeyDown(KeyCode.LeftShift))
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
        }*/

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Attack = false;
            inputUpdate = Unpaused;
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            ToggleLock();
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

    void ToggleLock()
    {
        _locked = !_locked;
        inputLateUpdate = _locked ? LockedOn : FreeLook;
    }

    void FreeLook()
    {
        _camera.FreeLook(_inputMouseX, _inputMouseY);
    }

    void LockedOn()
    {
        _camera.LockedOn(_player.currentBoss.transform.position);
    }
}
