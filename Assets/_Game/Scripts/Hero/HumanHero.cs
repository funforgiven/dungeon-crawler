using UnityEngine;

public class HumanHero : Hero
{
    
    
    [Header("Dash")]
    [SerializeField] private float dashCooldown = 3f;
    [SerializeField] private float dashTime = 1f;
    [SerializeField] private float dashSpeed = 3f;
    
    [Header("Stab")]
    [SerializeField] private float stabCooldown = 3f;
    [SerializeField] private float stabTime = 1f;
    [SerializeField] private float stabSpeed = 3f;
    
    private DashState _dashState;
    private float _currentDashTime;
    private float _currentDashCooldown;

    private StabState _stabState;
    private float _currentStabTime;
    private float _currentStabCooldown;
    
    private Vector2 _savedVelocity;
    
    protected override void Update()
    {
        base.Update();
        
        switch (_dashState) 
        {
            case DashState.Ready:
                if (Input.GetKeyDown(KeyCode.Space) && _stabState != StabState.Stabbing)
                {
                    _savedVelocity = new Vector2(_inputHorizontal, _inputVertical).normalized;

                    if (_savedVelocity.magnitude > 0)
                    {
                        _dashState = DashState.Dashing;
                    }
                }
                break;
            case DashState.Dashing:
                _currentDashTime += Time.deltaTime;
                if(_currentDashTime >= dashTime)
                {
                    currentWeapon.GetComponent<Weapon>().Disable();
                    _currentDashTime = 0f;
                    _dashState = DashState.Cooldown;
                }
                break;
            case DashState.Cooldown:
                _currentDashCooldown += Time.deltaTime;
                if(_currentDashCooldown >= dashCooldown)
                {
                    _currentDashCooldown = 0f;
                    _dashState = DashState.Ready;
                }
                break;
        }
        
        switch (_stabState) 
        {
            case StabState.Ready:
                if(Input.GetKeyDown(KeyCode.Mouse1) && _dashState != DashState.Dashing)
                {
                    _savedVelocity = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
                    
                    var weapon = currentWeapon.GetComponent<Weapon>();
                    weapon.Enable();
                    var rotZ = Mathf.Atan2(_savedVelocity.y, _savedVelocity.x) * Mathf.Rad2Deg;
                    weapon.transform.rotation = Quaternion.Euler(0, 0, rotZ - 90f);
                        
                    _stabState = StabState.Stabbing;
                }
                break;
            case StabState.Stabbing:
                _currentStabTime += Time.deltaTime;
                if(_currentStabTime >= stabTime)
                {
                    currentWeapon.GetComponent<Weapon>().Disable();
                    _currentStabTime = 0f;
                    _stabState = StabState.Cooldown;
                }
                break;
            case StabState.Cooldown:
                _currentStabCooldown += Time.deltaTime;
                if(_currentStabCooldown >= stabCooldown)
                {
                    _currentStabCooldown = 0f;
                    _stabState = StabState.Ready;
                }
                break;
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (_dashState == DashState.Dashing || _stabState == StabState.Stabbing)
            _rigidbody.velocity = _savedVelocity.normalized * dashSpeed;
    }
}

public enum DashState 
{
    Ready,
    Dashing,
    Cooldown
}

public enum StabState 
{
    Ready,
    Stabbing,
    Cooldown
}
