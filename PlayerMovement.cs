using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    PlayerControls playerControls;
    PlayerMain player;

    [SerializeField]
    Rigidbody2D rb;

//    Coroutine _CheckMovingCoroutine = null;
    Coroutine _UseStaminaCoroutine = null;

   // Vector2 previousPosition = new Vector2(0, 0);

    [SerializeField]
    float moveSpeed = 5;

    public bool IsSprinting;
    //Reminder, currently Sprinting without moving drains stamina
//    public bool isMoving;

    private void Awake()
    {
        playerControls = new PlayerControls();
        player = this.gameObject.GetComponent<PlayerMain>();

        playerControls.Controls.Move.performed += _ => Move(playerControls.Controls.Move.ReadValue<Vector2>());
        playerControls.Controls.Move.canceled += _ => Move(playerControls.Controls.Move.ReadValue<Vector2>());
        playerControls.Controls.Sprint.performed += _ => Sprint();
        playerControls.Controls.Sprint.canceled += _ => StopSprinting();
    }

    void Move(Vector2 direction)
    {
        rb.velocity = direction * moveSpeed;
    }

    //FIXME
//   void MovingCheck(Vector2 currentPosition)
//    {
//        Debug.Log(previousPosition);

//        if (previousPosition != currentPosition)
//        {
//            isMoving = true;

//            Sprint();
//       }
//        else if (previousPosition == currentPosition)
//        {
//            isMoving = false;
//        }
//
//        previousPosition = currentPosition;
//    }

    void Sprint()
    {
//        if (_CheckMovingCoroutine != null)
//        {
//            StopCoroutine(CheckMoving());
//        }

//        _CheckMovingCoroutine = StartCoroutine(CheckMoving());

//        CheckMoving();

        if (player.CurrentStamina > 0) //&& isMoving == true)
        {
            IsSprinting = true;
            moveSpeed *= 1.5f;

            if (_UseStaminaCoroutine != null)
            {
                StopCoroutine(_UseStaminaCoroutine);
            }

            _UseStaminaCoroutine = StartCoroutine(UseStamina());
        }
    }

    void StopSprinting()
    {
        IsSprinting = false;
        moveSpeed = 5;

        StopCoroutine(UseStamina());
        _UseStaminaCoroutine = null;
        CallStam();
    }

    async void CallStam()
    {
        await Task.Delay(player.StamRegenWaitTime);

        player.SetStamBool();
    }

    IEnumerator UseStamina()
    {
        while (IsSprinting == true && player.CurrentStamina !>= 0)
        {
            yield return new WaitForSeconds(0.10f);

            player.CurrentStamina -= 0.5f;
        }

        if (IsSprinting == false)
        {
            StopSprinting();
        }

        //FIXME
//       else
//        {
//            yield return new WaitForSeconds(2.75f);

//           StopCoroutine(player._currentStaminaRegenaration);
//       }

        _UseStaminaCoroutine = null;
    }

//    IEnumerator CheckMoving()
//    {
//        while (playerControls.Controls.Sprint.IsPressed())
//        {
//            yield return new WaitForSeconds(0.1f);

//            MovingCheck(new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.y));
//        }

//        _CheckMovingCoroutine = null;
//    }

    private void Update()
    {
        if (player.CurrentStamina <= 0)
        {
            player.CurrentStamina = 0;
            StopSprinting();
        }

//        if (playerControls.Controls.Sprint.IsPressed())
//        {
//            MovingCheck(new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.y));
//        }
    }
    private void OnEnable()
    {
        playerControls.Controls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Controls.Disable();
    }

}
