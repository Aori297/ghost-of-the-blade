using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public PlayerInputs inputActions;
    private InputAction move;
    private InputAction interact;
    public static GameInput Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        inputActions = new PlayerInputs();
        inputActions.Enable();

        move = inputActions.PlayerInput.Movement;
        //interact = inputActions.PlayerInput.Interact;


        if (move == null)
        {
            Debug.LogError("Input actions are missing. Please ensure they are properly configured.");
        }


    }

    public void DisableGameInput()
    {
        inputActions.Disable();
    }

    private void OnDestroy()
    {
        if (inputActions != null)
        {
            inputActions.Disable();
        }
    }

    public Vector2 GetMovementVector()
    {
        return move.ReadValue<Vector2>().normalized;
    }

    public bool Interact()
    {
        return interact.WasPressedThisFrame();
    }

}