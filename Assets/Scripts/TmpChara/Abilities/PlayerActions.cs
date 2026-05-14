using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerActions : MonoBehaviour
{
    #region Metrics
    [Header("Common")]
    [SerializeField] private float _cooldownBetweenActions;
    [Header("Attack")]
    [SerializeField] private float _attackDuration;
    [Header("Block")]
    [SerializeField] private float _blockDuration;
    #endregion

    #region State
    private bool _isReady;
    private Coroutine _cooldownCoroutine;
    private Coroutine _attackCoroutine;
    private Coroutine _blockCoroutine;
    #endregion

    #region Dependencies

    private PlayerInput _playerInput;

    [Header("Dependencies")]
    [SerializeField] private AttackZone _attackZone;
    [SerializeField] private BlockZone _blockZone;
    [SerializeField] private Animator _animator;

    #endregion

    #region Abilities
    
    [Header("Abilities")] 
    public PlayerAbility attack1;
    public PlayerAbility attack2;
    public PlayerAbility block;
    public PlayerAbility activeAbility;
    
    #endregion

    #region Delegates

    public event Action OnPlayerActionStart;
    public event Action OnPlayerActionEnd;

    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _isReady = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerInput.pressedAttack1 && _isReady)
        {
            // _attackCoroutine = StartCoroutine(AttackCoroutine());
            activeAbility = attack1;
            _animator.SetBool("InAttack", true);
        }
        else if (_playerInput.pressedAttack2 && _isReady)
        {
            // _blockCoroutine = StartCoroutine(BlockCoroutine());
            activeAbility = attack2;
            _animator.SetBool("InAttack", true);
        }
        else if (_playerInput.pressedBlock && _isReady)
        {
            // _blockCoroutine = StartCoroutine(BlockCoroutine());
            activeAbility = block;
            _animator.SetBool("InAttack", true);
        }
    }

    public void EmitActionStart()
    {
        OnPlayerActionStart?.Invoke();
    }

    public void EmitActionEnd()
    {
        OnPlayerActionEnd?.Invoke();
    }

    private IEnumerator AttackCoroutine()
    {
        _isReady = false;
        _attackZone.Activate();
        yield return new WaitForSeconds(_attackDuration);
        _attackZone.Deactivate();
        _attackCoroutine = null;
        _cooldownCoroutine = StartCoroutine(CooldownCoroutine());
    }

    private IEnumerator BlockCoroutine()
    {
        _isReady = false;
        _blockZone.Activate();
        yield return new WaitForSeconds(_blockDuration);
        _blockZone.Deactivate();
        _blockCoroutine = null;
        _cooldownCoroutine = StartCoroutine(CooldownCoroutine());
    }

    private IEnumerator CooldownCoroutine()
    {
        _isReady = false;
        yield return new WaitForSeconds(_cooldownBetweenActions);
        _isReady = true;
        _cooldownCoroutine = null;
    }

    public void CancelAction()
    {
        StopAllCoroutines();
        _attackCoroutine = null;
        _attackZone.Deactivate();
        _blockCoroutine = null;
        _blockZone.Deactivate();
        _cooldownCoroutine = StartCoroutine(CooldownCoroutine());
    }

}
