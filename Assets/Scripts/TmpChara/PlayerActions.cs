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

    #endregion

    #region Telemetry

    public static event Action<int> OnPlayerAttack;
    public static event Action<int> OnPlayerBlock;
    

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
        if (_playerInput.pressedAttack && _isReady)
        {
            OnPlayerAttack?.Invoke(gameObject.GetInstanceID());
            _attackCoroutine = StartCoroutine(AttackCoroutine());
        }
        else if (_playerInput.pressedBlock && _isReady)
        {
            OnPlayerBlock?.Invoke(gameObject.GetInstanceID());
            _blockCoroutine = StartCoroutine(BlockCoroutine());
        }
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
