using UnityEngine;
using UnityEngine.UI;
using Fusion;
using System.Collections;

public class PlayerBola : NetworkBehaviour
{
    // 102
    [Networked(OnChanged = nameof(OnBallSpawned))]
    public NetworkBool spawned { get; set; }


    [SerializeField] 
    private Ball _prefabBall;
    [SerializeField] 
    private PhysxBall _prefabPhysxBall;

    private Vector3 _forward;

    private NetworkCharacterControllerPrototype _cc;
    [Networked] 
    private TickTimer delay { get; set; }

    //105
    private Material _material;
    Material material
    {
        get
        {
            if (_material == null)
                _material = GetComponentInChildren<MeshRenderer>().material;
            return _material;
        }
    }

    //106
    private Text _messages;

    // Hold power
    [Networked]
    private TickTimer HoldButton { get; set; }
    [Networked(OnChanged = nameof(OnCharging))]
    public NetworkBool isPowering { get; set; } = false;
    private float powerCharge = 1;

    // Sprind
    [Networked]
    private TickTimer SprintCD { get; set; }
    public float sprintSpent = 0;

    // Create wall
    [SerializeField]
    private GameObject _wallObj;
    [Networked]
    public NetworkButtons ButtonPrevs { get; set; }


    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterControllerPrototype>();
    }

    private void Update()
    {
        if (Object.HasInputAuthority && Input.GetKeyDown(KeyCode.R))
        {
            RPC_SendMessage("Hey Mate!");
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            data.direction.Normalize();
            _cc.Move(data.direction * Runner.DeltaTime);


            // LShift down, Sprind
            if ((data.buttons & NetworkInputData.SHIFT) != 0)
            {
                if (SprintCD.ExpiredOrNotRunning(Runner))
                {
                    sprintSpent += Runner.DeltaTime;
                    _cc.maxSpeed = 5;
                    _cc.acceleration = 50;
                }   
            }
            else    // LShift up, normal move
            {
                // Normal move speed
                _cc.maxSpeed = 2;
                _cc.acceleration = 10;

                // Set sprintCD if were used
                if (sprintSpent > 0)
                {
                    SprintCD = TickTimer.CreateFromSeconds(Runner, sprintSpent);
                    sprintSpent = 0;
                }
            }

            // Space down, Jump
            if ((data.buttons & NetworkInputData.SPACE) != 0)
            {
                _cc.Jump();
            }


            if (data.direction.sqrMagnitude > 0)
                _forward = data.direction;

            if (delay.ExpiredOrNotRunning(Runner))
            {
                // Left mouse down
                if ((data.buttons & NetworkInputData.MOUSEBUTTON1) != 0)
                {
                    delay = TickTimer.CreateFromSeconds(Runner, 0.5f);

                    Runner.Spawn(_prefabBall,
                    transform.position + _forward, Quaternion.LookRotation(_forward),
                    Object.InputAuthority, (Runner, o) => {
                        // Initialize the Ball before synchronizing it
                        o.GetComponent<Ball>().Init();
                    });
                    spawned = !spawned;
                }
                // Right mouse down
                else if ((data.buttons & NetworkInputData.MOUSEBUTTON2) != 0)
                {
                    if (!isPowering)
                    {
                        HoldButton = TickTimer.CreateFromSeconds(Runner, 1f);

                        if (!isPowering)
                        {
                            isPowering = true;
                            powerCharge = 1;
                        }
                    }
                    else
                    {
                        powerCharge += Runner.DeltaTime;

                        if (HoldButton.Expired(Runner))
                        {
                            Debug.Log("holdbutton expire? " + HoldButton.Expired(Runner));
                            ShootBall();
                        }
                    }
                    
                }
                // Right mouse up
                else if ((data.buttons & NetworkInputData.MOUSEBUTTON2) == 0 && isPowering)
                {
                    ShootBall();
                }
            }
        }
    }

    public override void Render()
    {
        if (isPowering)
        {
            material.color = Color.Lerp(material.color, Color.red, Time.deltaTime);
        }
        else
        {
            material.color = Color.Lerp(material.color, Color.white, Time.deltaTime);
        }
    }

    public void ShootBall()
    {
        delay = TickTimer.CreateFromSeconds(Runner, 0.5f);

        Runner.Spawn(_prefabPhysxBall,
          transform.position + _forward,
          Quaternion.LookRotation(_forward),
          Object.InputAuthority,
          (runner, o) =>
          {
              o.GetComponent<PhysxBall>().Init(10 * _forward, powerCharge);
          });

        spawned = !spawned;

        isPowering = false;
    }

    public static void OnBallSpawned(Changed<PlayerBola> changed)
    {
        changed.Behaviour.material.color = Color.blue;
    }

    public static void OnCharging(Changed<PlayerBola> changed)
    {
        changed.Behaviour.material.color = Color.white;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void RPC_SendMessage(string message, RpcInfo info = default)
    {
        if (_messages == null)
            _messages = FindObjectOfType<Text>();
        if (info.Source == Runner.Simulation.LocalPlayer)
            message = $"You said: {message}\n";
        else
            message = $"Some other player said: {message}\n";
        _messages.text += message;
    }

    public void AddImpact(Vector3 dir)
    {
        Vector3 impact = new Vector3(dir.x, 0f, dir.z);
        impact.Normalize();

        if (impact.magnitude > 0.1)
        {
            StartCoroutine(StartImpacc(impact, .2f));
        }
    }

    public IEnumerator StartImpacc(Vector3 impactDir, float duration)
    {
        while (duration > 0f)
        {
            //_cc.Move(dir * Runner.DeltaTime);
            this.transform.Translate(impactDir * Runner.DeltaTime, Space.World);
            duration -= Time.deltaTime;
            yield return null;
        }
    }
}
