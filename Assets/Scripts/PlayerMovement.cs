using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerMovement : MonoBehaviour
{
    public SpriteRenderer sr;
    public Rigidbody2D rb;
    public float moveSpeed = 3f;
    float horizontalMovement;
    bool keyShift = false, keyControl = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    void FixedUpdate()
    {
        if(keyControl && !keyShift){
            rb.velocity = new Vector2(horizontalMovement * moveSpeed * 0.5f, rb.velocity.y);
        } else if(keyShift && !keyControl){
            rb.velocity = new Vector2(horizontalMovement * moveSpeed * 1.5f, rb.velocity.y);
        } else {
            rb.velocity = new Vector2(horizontalMovement * moveSpeed, rb.velocity.y);
        }
    }
    
    /// Update is called every frame, if the MonoBehaviour is enabled.
    void Update()
    {
        
    }

    public void Move(InputAction.CallbackContext context){
        horizontalMovement = context.ReadValue<Vector2>().x;
        if(horizontalMovement == -1){
            sr.flipX = true;
        } else if(horizontalMovement == 1){
            sr.flipX = false;
        }
    }

    public void Correr(InputAction.CallbackContext context){
        keyShift = context.control.IsPressed();
    }

    public void Agachar(InputAction.CallbackContext context){
        keyControl = context.control.IsPressed();
    }
}
