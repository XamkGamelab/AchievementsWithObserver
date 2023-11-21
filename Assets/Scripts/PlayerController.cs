using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum PlayerState
    {
        STANDING,
        JUMPING,
        CROUCHING, 
        SPECIAL_ABILITY
    }
   

    public PlayerState my_state = PlayerState.STANDING;

    void handleInput()
    {
        switch (my_state)
        {
            case PlayerState.STANDING:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Jump();
                    my_state = PlayerState.JUMPING;

                }
                else if (Input.GetKeyDown(KeyCode.LeftControl))
                {
                    my_state = PlayerState.CROUCHING;
                    StartCoroutine(GetSmall());
                }
                break;

            case PlayerState.JUMPING:
                // Check to see if Y-value has returned to 1, then change to standing state
                StartCoroutine(CheckYWithDelay()); // done inside a coroutine to force delay to an approximation
                break;

            case PlayerState.CROUCHING:
                if (Input.GetKeyUp(KeyCode.LeftControl))
                {
                    my_state = PlayerState.STANDING;
                    StartCoroutine(ResetScale());
                }
                break;

                case PlayerState.SPECIAL_ABILITY:
                SpecialAbilityEffect(10.0f, 0.2f);
             
                break;


        }
    }

    IEnumerator GetSmall()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = new Vector3(originalScale.x, originalScale.y / 3f, originalScale.z); // Scale down in the Y dimension

        float duration = 0.3f; // Adjust the duration as needed
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale; // Ensure the scale is exactly the target scale
    }

    IEnumerator ResetScale()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = Vector3.one; // Reset to the original scale

        float duration = 0.3f; // Adjust the duration as needed
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale; // Ensure the scale is exactly the target scale
    }
    IEnumerator CheckYWithDelay()
    {
        yield return new WaitForSeconds(0.1f); 
        if (Mathf.Approximately(transform.position.y, 1.00f))
        {
            my_state = PlayerState.STANDING;
        }
    }



    //public PlayerState my_state = PlayerState.JUMPING;

    //public PlayerState my_state = PlayerState.CROUCHING;

    Rigidbody _rigidbody;
    Vector3 _start_pos;
    float TimeAccu = 0.0f;
    bool bReplaying = false;

    void Jump()
    {
        _rigidbody.AddForce(10.0f * transform.up, ForceMode.Impulse);
       
        //my_state = PlayerState.JUMPING;
    }

    void ActivateSpecialAbility()
    {
        // Perform any actions related to the special ability
        // For example, give the player increased speed, double jump, etc.
        // ...

        // Change the player state to SPECIAL_ABILITY
        my_state = PlayerState.SPECIAL_ABILITY;
        StartCoroutine(SpecialAbilityEffect(1.3333f, 4.0f));
    }

    IEnumerator SpecialAbilityEffect(float scaleMultiplier, float duration)
    {
        //// Adjust the scale multiplier and duration as needed
        //float scaleMultiplier = 1.6666f;
        //float duration = 10.0f;
        //float shakeMagnitude = 0.5f;

        // Save the initial scale to revert after the special ability
        Vector3 initialScale = transform.localScale;

        // Apply the scale multiplier
        transform.localScale *= scaleMultiplier;

        // Start the shaking effect
        StartCoroutine(ShakeEffect(0.5f, duration));

        // Wait for the specified duration
        yield return new WaitForSeconds(duration);

        // Revert the scale to the initial state
        transform.localScale = initialScale;

        // Optionally, you can perform any other cleanup or post-special-ability logic here
       
    }

    IEnumerator ShakeEffect(float magnitude, float duration)
    {
        // Save the initial position to revert after the shaking effect
        Vector3 initialPosition = transform.position;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            // Generate a random offset within the specified magnitude
            Vector3 offset = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f)) * magnitude;

            // Apply the offset to the position
            transform.position = initialPosition + offset;

            // Wait for the next frame
            yield return null;

            elapsed += Time.deltaTime;
        }

        // Revert the position to the initial state
        transform.position = initialPosition;
        my_state = PlayerState.STANDING;
    }


    // Commands:
    Command cmd_W = new MoveForwardCommand();
    Command cmd_A = new MoveLeftCommand();
    Command cmd_S = new MoveBackwardCommand();
    Command cmd_D = new MoveRightCommand();

    Command cmdNothing = new DoNothingCommand();
    Command cmdForward = new MoveForwardCommand();
    Command cmdBackward = new MoveBackwardCommand();
    Command cmdLeft = new MoveLeftCommand();
    Command cmdRight = new MoveRightCommand();

    //ref Command rcmd = ref cmdNothing;

    Command _last_command = null;

    // Stacks to store the commands
    Stack<Command> _undo_commands = new Stack<Command>();
    Stack<Command> _redo_commands = new Stack<Command>();
    Stack<Command> _replay_commands = new Stack<Command>();

    // Set a keybinding
    void SetCommand(ref Command cmd, ref Command new_cmd)
    {
        cmd = new_cmd;
    }

    void SwapCommands(ref Command A, ref Command B)
    {
        Command tmp = A;
        A = B;
        B = tmp;

    //    _undo_commands.Push();
    //    Command cmd = _undo_commands.Pop();
    }

    void ClearCommands()
    {
        SetCommand(ref cmd_W, ref cmdNothing);
        SetCommand(ref cmd_A, ref cmdNothing);
        SetCommand(ref cmd_S, ref cmdNothing);
        SetCommand(ref cmd_D, ref cmdNothing);
    }

    private void OnTriggerEnter(Collider other) {

        if (other.CompareTag("Coin"))
        {
            Destroy(other.gameObject); // Destroy the coin!
        }
        else if (other.CompareTag("Enemy"))
        {  
            Destroy(other.gameObject); // Destroy the enemy!
        }
        else if (other.CompareTag("SpecialItem"))
        {
            Destroy(other.gameObject); // Destroy the special pickup
            ActivateSpecialAbility(); // Activate the special ability
        }

    } //*

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _start_pos = transform.position;

    }

    IEnumerator Replay()
    {
        // Go through all the replay commands
        while (_replay_commands.Count > 0)
        {
            Command cmd = _replay_commands.Pop();
            _undo_commands.Push(cmd);
            cmd.Execute(_rigidbody);
            yield return new WaitForSeconds(.5f);
        }

        bReplaying = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (bReplaying)
        {
            TimeAccu += Time.deltaTime;
            // ...

        }
        else
        {

            handleInput();
            if (Input.GetKeyDown(KeyCode.R))
            {
                bReplaying = true;
                TimeAccu = 0.0f;
                // Get the Undo-stack and "reverse" it
                while( _undo_commands.Count > 0)
                {
                    _replay_commands.Push(_undo_commands.Pop());
                }
                // Move the player to the start position
                transform.position = _start_pos;

                // Start the replay
                StartCoroutine( Replay());

            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                cmd_W.Execute(_rigidbody);
                _undo_commands.Push(cmd_W);
                _redo_commands.Clear();
                //_last_command = cmd_W;
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                cmd_A.Execute(_rigidbody);
                _undo_commands.Push(cmd_A);
                _redo_commands.Clear();
                //_last_command = cmd_A;
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                cmd_S.Execute(_rigidbody);
                _undo_commands.Push(cmd_S);
                _redo_commands.Clear();
                //_last_command = cmd_S;
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                cmd_D.Execute(_rigidbody);
                _undo_commands.Push(cmd_D);
                _redo_commands.Clear();
                //_last_command = cmd_D;
            }
            //if (Input.GetKeyDown(KeyCode.Space))
            //{

            //   if (my_state == PlayerState.STANDING)
            //   {
            //        Jump();
            //   }
            //}

            //CROUCH (no jumping while down)

            if (Input.GetKeyDown(KeyCode.Z))
            {
                // If there are commands in the stack...
                if (_undo_commands.Count > 0)
                {
                    // ... pop one command out and execute it.
                    Command cmd = _undo_commands.Pop();
                    _redo_commands.Push(cmd);
                    cmd.Undo(_rigidbody);
                }
            }
            if (Input.GetKeyDown(KeyCode.X))
            {

                if (_redo_commands.Count > 0)
                {
                    Command cmd = _redo_commands.Pop();
                    _undo_commands.Push(cmd);
                    cmd.Execute(_rigidbody);
                }

            }

            // We can swap commands if we want to
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                //ClearCommands();
                //SwapCommands(ref cmd_A, ref cmd_D);
            }

        }

    }
}
