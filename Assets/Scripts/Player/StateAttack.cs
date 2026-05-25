using UnityEngine;

public class StateAttack : StateMachineBehaviour
{
    private PlayerActions _playerActions;
    private PlayerAbility _ability;
    private PlayerMovement _playerMovement;
    private float _relativeTime;
    private bool _hasStarted;
    private bool _hasFinished;
    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _playerActions = animator.GetComponent<PlayerActions>();
        _ability = _playerActions.activeAbility;
        _playerMovement = animator.GetComponent<PlayerMovement>();
        _relativeTime = 0f;
        if (_ability.overridesMovement)
        {
            _playerMovement.isOverriden = true;
        }
        _hasStarted = false;
        _hasFinished = false;
        _playerActions.EmitActionStart();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_playerMovement.isFrozen) return;
        _relativeTime += Time.deltaTime;
        if (_ability.overridesMovement)
        {
            _playerMovement.overrideVelocity = _ability.EvaluateVelocity(_relativeTime);
        }
        if (!_hasStarted && _relativeTime >= _ability.startupTime)
        {
            _hasStarted = true;
            _ability.EnableEffect();
        }
        else if (!_hasFinished && _relativeTime >= _ability.activeTime+_ability.startupTime)
        {
            _hasFinished = true;
            _ability.DisableEffect();
        }
        else if (_relativeTime >= _ability.activeTime + _ability.startupTime + _ability.recoveryTime)
        {
            animator.SetBool("InAttack", false);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_ability.overridesMovement)
        {
            _playerMovement.isOverriden = false;
        }

        _playerActions.activeAbility = null;
        _playerActions.EmitActionEnd();
    }
}
