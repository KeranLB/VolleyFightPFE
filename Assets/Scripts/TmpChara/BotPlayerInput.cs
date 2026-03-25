using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class BotPlayerInput : MonoBehaviour
{
    public bool jumps;
    public bool followsBallX;
    public bool followsBallZ;
    public float delayAttack;
    public float delayBlock;

    private Ball _ball;
    private PlayerInput _playerInput;
    private Vector3 _forward;
    private Vector3 _right;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _ball = FindFirstObjectByType<Ball>();
        
        _forward = transform.forward;
        _right = transform.right;

        StartCoroutine(AttackLoop());
        StartCoroutine(BlockLoop());
    }

    // Update is called once per frame
    void Update()
    {
        _playerInput.holdsJump = jumps;
        var toBall = _ball.transform.position - transform.position;
        toBall.y = 0;
        toBall.Normalize();
        if (followsBallZ)
        {
            _playerInput.currentDirection.y = toBall.z;
        }
        if (followsBallX)
        {
            _playerInput.currentDirection.x = toBall.x;
        }
        _playerInput.currentDirection.Normalize();
    }

    private IEnumerator AttackLoop()
    {
        while(true)
        {
            yield return new WaitForSeconds(delayAttack);
            _playerInput.pressedAttack = true;
        }
    }
    
    private IEnumerator BlockLoop()
    {
        while(true)
        {
            yield return new WaitForSeconds(delayBlock);
            _playerInput.pressedBlock = true;
            
        }
    }
}
