using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inputs : MonoBehaviour
{
    public System.Action inputUpdate;
    float _inputHorizontal, _inputVertical;
    float _inputMouseX, _inputMouseY;
    Movement _movement;
    PlayerController _player;
    bool _jump;

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
        _movement.Move(_inputHorizontal, _inputVertical);

        if (_jump)
        {
            _player.Jump();
            _jump = false;
        }
    }
}
