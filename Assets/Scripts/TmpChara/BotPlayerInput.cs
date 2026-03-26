using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(PlayerInput))]
public class BotPlayerInput : MonoBehaviour
{
    [Header("Movements")]
    public bool canJump;
    public bool followsBallX;
    public bool followsBallZ;
    [Header("Attack")]
    public bool canAttack;
    public float delayAttack;
    [Header("Block")]
    public bool canBlock;
    public float delayBlock;

    private Ball _ball;
    private PlayerInput _playerInput;
    
    private bool _hasAttacked;
    private bool _hasBlocked;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _ball = FindFirstObjectByType<Ball>();
        
        StartCoroutine(AttackLoop());
        StartCoroutine(BlockLoop());
    }

    // Update is called once per frame
    void Update()
    {
        _playerInput.holdsJump = canJump;
        var toBall = _ball.transform.position - transform.position;
        toBall.y = 0;
        toBall.Normalize();
        if (followsBallZ)
        {
            _playerInput.currentDirection.y = toBall.z;
        }
        else
        {
            _playerInput.currentDirection.y = 0;
        }
        if (followsBallX)
        {
            _playerInput.currentDirection.x = toBall.x;
        }
        else
        {
            _playerInput.currentDirection.x = 0;
        }
        _playerInput.currentDirection.Normalize();

        if (canAttack && !_hasAttacked)
        {
            _playerInput.pressedAttack = true;
            StartCoroutine(AttackLoop());
        }
        else
        {
            _playerInput.pressedAttack = false;
        }
        
        if (canBlock && !_hasBlocked)
        {
            _playerInput.pressedBlock = true;
            StartCoroutine(BlockLoop());
        }
        else
        {
            _playerInput.pressedBlock = false;
        }
    }

    private IEnumerator AttackLoop()
    {
        _hasAttacked = true;
        yield return new WaitForSeconds(Mathf.Max(delayAttack, 0.1f));
        _hasAttacked = false;
    }
    
    private IEnumerator BlockLoop()
    {
        _hasBlocked = true;
        yield return new WaitForSeconds(Mathf.Max(delayBlock, 0.1f));
        _hasBlocked = false;
    }
}
