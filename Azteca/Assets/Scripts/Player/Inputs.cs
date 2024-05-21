using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inputs
{
    public System.Action inputUpdate, inputLateUpdate, cameraInputs;
    float _inputHorizontal, _inputVertical;
    float _inputMouseX, _inputMouseY;
    Movement _movement;
    PlayerController _player;
    CinemachineCameraController _cameraController;
    bool _jump, _primaryAttack = false, _secondaryAttack = false, _locked = false;

    KeyCode _Kstep, _Kjump, _KprimaryAttack, _KsecondaryAttack, _Klock, _Ksun, _Kobssidian, _Kscape;

    public bool PrimaryAttack
    {
        get
        {
            return _primaryAttack;
        }

        set
        {
            if (value == !_primaryAttack)
            {
                _primaryAttack = value;

                if (value)
                {
                    _player.ActivateMagic();
                }
                else
                {
                    inputUpdate = Unpaused;
                }
            }
        }
    }

    public bool SecondaryAttack
    {
        get
        {
            return _secondaryAttack;
        }

        set
        {
            if (value == !_secondaryAttack)
            {
                _secondaryAttack = value;

                if (value)
                {
                    _player.ActivateMagic();
                }
                else
                {
                    inputUpdate = Unpaused;
                }
            }
        }
    }

    public Inputs(Movement movement, PlayerController player, CinemachineCameraController camera)
    {
        _movement = movement;
        _player = player;
        _cameraController = camera;
        inputLateUpdate = FreeLook;
        cameraInputs = CameraInputsMouse;
    }


    public void CameraInputsMouse()
    {
        _inputMouseX = Input.GetAxisRaw("Mouse X");

        _inputMouseY = Input.GetAxisRaw("Mouse Y");
    }
    public void CameraInputsJoystick()
    {
        _inputMouseX = Input.GetAxisRaw("Mouse X");

        _inputMouseY = Input.GetAxisRaw("Mouse Y");
    }

    public void Altern(bool joystick)
    {
        if(joystick)
        {
            cameraInputs = CameraInputsJoystick;
            _Kstep = KeyCode.Joystick1Button2;
            _Kjump = KeyCode.Joystick1Button1;
            _KprimaryAttack = KeyCode.Joystick1Button7;
            _KsecondaryAttack = KeyCode.Joystick1Button8; //ayuda juli
            _Klock = KeyCode.Joystick1Button11;
            _Ksun = KeyCode.Joystick1Button4;
            _Kobssidian = KeyCode.Joystick1Button6;
            _Kscape = KeyCode.Joystick1Button9;
        }
        else
        {
            cameraInputs = CameraInputsMouse;
            _Kstep = KeyCode.LeftShift;
            _Kjump = KeyCode.Space;
            _KprimaryAttack = KeyCode.Mouse0;
            _KsecondaryAttack = KeyCode.Mouse1;
            _Klock = KeyCode.Q; //no se que tecla estaria bien para esto
            _Ksun = KeyCode.Alpha1;
            _Kobssidian = KeyCode.Alpha2;
            _Kscape = KeyCode.Escape;
        }
        
    }

    public void Unpaused()
    {
        cameraInputs();

        _inputHorizontal = Input.GetAxis("Horizontal");

        _inputVertical = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(_Kscape))
        {
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            //UIManager.instance.SetPauseMenu(true);
            UIManager.instance.Paused();
            inputUpdate = Paused;
        }

        if (Input.GetKeyDown(_Kstep))
        {
            _player.Step(_inputHorizontal, _inputVertical);
        }

        if (Input.GetKeyDown(_Kjump))
        {
            _jump = true;
        }

        if (Input.GetKeyDown(_KprimaryAttack))
        {
            PrimaryAttack = true;
        }

        if (Input.GetKeyDown(_KsecondaryAttack))
        {
            SecondaryAttack = true;
        }

        if (Input.GetKeyDown(_Klock))
        {
            ToggleLock();
        }

        if (Input.GetKeyDown(_Ksun))
        {
            _player.ChangeActiveMagic(PlayerController.MagicType.Sun);
            _player.renderer.material.color = Color.red;
        }

        if (Input.GetKeyDown(_Kobssidian))
        {
            _player.ChangeActiveMagic(PlayerController.MagicType.Obsidian);
            _player.renderer.material.color = Color.black;
        }
    }

    public void Stepping()
    {
        _inputHorizontal = 0;

        _inputVertical = 0;

        cameraInputs();

        if (Input.GetKeyDown(_Kscape))
        {
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            //UIManager.instance.SetPauseMenu(true);
            UIManager.instance.Paused();
            inputUpdate = Paused;
        }

        if (Input.GetKeyDown(_Ksun))
        {
            _player.ChangeActiveMagic(PlayerController.MagicType.Sun);
        }

        if (Input.GetKeyDown(_Kobssidian))
        {
            _player.ChangeActiveMagic(PlayerController.MagicType.Obsidian);
        }

        if (Input.GetKeyDown(_Klock))
        {
            ToggleLock();
        }
    }

    public void FixedCast()
    {
        _inputHorizontal = 0;

        _inputVertical = 0;

        cameraInputs();

        /*if (Mathf.Abs(Input.GetAxis("Horizontal")) == 1 || Mathf.Abs(Input.GetAxis("Vertical")) == 1)
        {
            Attack = false;
            inputUpdate = Unpaused;
        }

        if (Input.GetKeyDown(_Kscape))
        {
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            //UIManager.instance.SetPauseMenu(true);
            UIManager.instance.Paused();
            inputUpdate = Paused;
        }

        if (Input.GetKeyDown(_Kstep))
        {
            _player.Step(_inputHorizontal, _inputVertical);
            PrimaryAttack = false;
            inputUpdate = Unpaused;
        }

        if (Input.GetKeyDown(_Kjump))
        {
            _jump = true;
            PrimaryAttack = false;
            inputUpdate = Unpaused;
        }

        if (Input.GetKeyUp(_KprimaryAttack))
        {
            PrimaryAttack = false;
            inputUpdate = Unpaused;
        }*/

        if (Input.GetKeyDown(_Klock))
        {
            ToggleLock();
        }
    }

    public void MovingCast()
    {
        _inputHorizontal = Input.GetAxis("Horizontal");

        _inputVertical = Input.GetAxis("Vertical");

        cameraInputs();

        if (Input.GetKeyDown(_Kscape))
        {
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            //UIManager.instance.SetPauseMenu(true);
            UIManager.instance.Paused();
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

        if (Input.GetKeyUp(_KprimaryAttack))
        {
            PrimaryAttack = false;
            inputUpdate = Unpaused;
        }

        if (Input.GetKeyDown(_Klock))
        {
            ToggleLock();
        }
    }

    public void Nothing()
    {

    }

    public void Paused()
    {
        if (Input.GetKeyDown(_Kscape))
        {
            Time.timeScale = 1;
            UIManager.instance.UnPaused();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            //UIManager.instance.SetPauseMenu(false);
            inputUpdate = Unpaused;
        }
    }

    public void InputsFixedUpdate()
    {
        _movement.Move(_inputHorizontal, _inputVertical, !PrimaryAttack);

        if (_jump)
        {
            _player.Jump();
            _jump = false;
        }
    }

    void ToggleLock()
    {
        if (_player.currentBoss == null) return;
        _locked = !_locked;
        inputLateUpdate = _locked ? LockedOn : FreeLook;
    }

    public void FreeLook()
    {
        _cameraController.FreeLook();
    }

    void LockedOn()
    {
        _cameraController.LockOn(_player.currentBoss.transform);
    }
}
