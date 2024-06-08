using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inputs
{
    public System.Action inputUpdate, cameraInputs;
    float _inputHorizontal, _inputVertical;
    float _inputMouseX, _inputMouseY;
    Movement _movement;
    PlayerController _player;
    CinemachineCameraController _cameraController;
    bool _jump, _primaryAttack = false, _secondaryAttack = false, _aiming = false;

    KeyCode _Kstep, _Kjump, _KprimaryAttack, _KsecondaryAttack, _Kaim, _Ksun, _Kobssidian, _Kscape;

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
        FreeLook();
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
            //_KsecondaryAttack = KeyCode.Joystick1Button6; //ayuda juli
            _Kaim = KeyCode.Joystick1Button11;
            _Ksun = KeyCode.Joystick1Button4;
            _Kobssidian = KeyCode.Joystick1Button4;
            _Kscape = KeyCode.Joystick1Button9;
        }
        else
        {
            cameraInputs = CameraInputsMouse;
            _Kstep = KeyCode.LeftShift;
            _Kjump = KeyCode.Space;
            _KprimaryAttack = KeyCode.Mouse0;
            //_KsecondaryAttack = KeyCode.Mouse1;
            _Kaim = KeyCode.Mouse1; //no se que tecla estaria bien para esto
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

        Pause();

        if (Input.GetKeyDown(_Kstep))
        {
            _player.Step(_inputHorizontal, _inputVertical);

            PrimaryAttack = false;
        }

        if (Input.GetKeyDown(_Kjump))
        {
            _jump = true;
        }

        if (Input.GetKeyDown(_KprimaryAttack))
        {
            PrimaryAttack = true;
        }

        if (Input.GetKeyUp(_KprimaryAttack))
        {
            PrimaryAttack = false;
        }

        if (Input.GetKeyDown(_KsecondaryAttack))
        {
            SecondaryAttack = true;
        }

        AimUnaim();

        SelectSun();

        SelectObsidian();
    }

    public void Stepping()
    {
        _inputHorizontal = 0;

        _inputVertical = 0;

        cameraInputs();

        Pause();

        SelectSun();

        SelectObsidian();

        AimUnaim();
    }

    public void FixedCast()
    {
        _inputHorizontal = 0;

        _inputVertical = 0;

        cameraInputs();

        //if (Mathf.Abs(Input.GetAxis("Horizontal")) == 1 || Mathf.Abs(Input.GetAxis("Vertical")) == 1)
        //{
        //    Attack = false;
        //    inputUpdate = Unpaused;
        //}

        //Pause();

        /*if (Input.GetKeyDown(_Kstep))
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

        AimUnaim();
    }

    public void MovingCast()
    {
        _inputHorizontal = Input.GetAxis("Horizontal");

        _inputVertical = Input.GetAxis("Vertical");

        cameraInputs();

        Pause();

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _player.Step(_inputHorizontal, _inputVertical);
            _player.StopSun = true;
            PrimaryAttack = false;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _jump = true;
            _player.StopSun = true;
            PrimaryAttack = false;
        }

        if (Input.GetKeyUp(_KprimaryAttack))
        {
            PrimaryAttack = false;
        }

        AimUnaim();
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
        _movement.Move(_inputHorizontal, _inputVertical, !_player.UsingSun);

        if (_jump)
        {
            _player.Jump();
            _jump = false;
        }
    }

    public void InputsLateUpdate()
    {
        _cameraController.UpdateCameraRotation(_inputMouseX, _inputMouseY);
    }

    void ToggleAim()
    {
        _aiming = !_aiming;

        if (_aiming) Aim();
        else FreeLook();
    }

    public void FreeLook()
    {
        _cameraController.FreeLook();
    }

    void Aim()
    {
        _cameraController.Aim();
    }

    void Pause()
    {
        if (Input.GetKeyDown(_Kscape))
        {
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            //UIManager.instance.SetPauseMenu(true);
            UIManager.instance.Paused();
            inputUpdate = Paused;
        }
    }

    void AimUnaim()
    {
        if (Input.GetKeyDown(_Kaim))
        {
            ToggleAim();
        }
    }

    void SelectSun()
    {
        if (Input.GetKeyDown(_Ksun))
        {
            _player.ChangeActiveMagic(PlayerController.MagicType.Sun);
            _player.renderer.material.color = Color.red;
        }
    }

    void SelectObsidian()
    {
        if (Input.GetKeyDown(_Kobssidian))
        {
            _player.ChangeActiveMagic(PlayerController.MagicType.Obsidian);
            _player.renderer.material.color = Color.black;
        }
    }
}
