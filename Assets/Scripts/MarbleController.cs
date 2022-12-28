using UnityEngine;
using Fusion;

[RequireComponent(typeof(Rigidbody), typeof(NetworkRigidbody))]
public class MarbleController : NetworkBehaviour
{
	[Networked(OnChanged = nameof(OnBoostCooldownChanged))] TickTimer BoostCooldown { get; set; }

	[SerializeField] private NetworkObject _cubePrefab;
	[SerializeField] float _movementSpeed = 5000f;
	[SerializeField] float _boostFactor = 3000f;
	[SerializeField] float _boostCooldownInSeconds = 2f;
	[SerializeField] float _areaOfInterestRadius = 5f;

	private int _index;
	private bool _canCall;
	private Rigidbody _body;
	private BoostAnimation _boostAnim;

	private void Awake()
	{
		_boostAnim = GetComponent<BoostAnimation>();
		_body = GetComponent<Rigidbody>();
	}

	public override void Spawned()
	{
		Runner.SetPlayerAlwaysInterested(Object.InputAuthority, Object, true);
		_index = Runner.LocalPlayer*100;
	}

	public override void FixedUpdateNetwork()
	{
		Runner.AddPlayerAreaOfInterest(Object.InputAuthority, transform.position, _areaOfInterestRadius);
		if (GetInput<InputData>(out var input))
		{
			UpdateRespawn(input);
			UpdateMovement(input);
			UpdateSpawn(input);
		}
	}

	void UpdateMovement(InputData input)
	{
		float speed = _movementSpeed * Runner.DeltaTime;
		if (input.GetButton(ButtonFlag.BOOST) && (BoostCooldown.IsRunning == false || BoostCooldown.Expired(Runner)))
		{
			speed += _boostFactor;
			BoostCooldown = TickTimer.CreateFromSeconds(Runner, _boostCooldownInSeconds);
		}

		if (input.GetButton(ButtonFlag.FORWARD))
		{
			_body.AddForce(Vector3.forward * speed);
			_canCall = true;
		}
		else if (input.GetButton(ButtonFlag.BACKWARD))
		{
			_body.AddForce(Vector3.back * speed);
			if (_canCall && Object.HasInputAuthority)
				RPC_Test(_index++, Runner.IsClient);
			_canCall = false;
		}

		if (input.GetButton(ButtonFlag.LEFT))
		{
			_body.AddForce(Vector3.left * speed);
		}
		else if (input.GetButton(ButtonFlag.RIGHT))
		{
			_body.AddForce(Vector3.right * speed);
		}
	}

	[Rpc(sources: RpcSources.All, targets: RpcTargets.All, Channel = RpcChannel.Unreliable, InvokeLocal = true, InvokeResim = true, TickAligned = true)]
	void RPC_Test(int i, NetworkBool isClient, RpcInfo info = default)
	{
		Debug.Log($"Test RPC {i} called from {(isClient ? "Client" : "Host")} in tick {info.Tick} running on {(Runner.IsClient ? "Client" : "Host")} in tick {Runner.Simulation.Tick} (resim:{Runner.IsResimulation})\n");
	}

	void UpdateRespawn(InputData input)
	{
		if (transform.position.y < -10f || input.GetButton(ButtonFlag.RESPAWN))
		{
			Vector3 respawnPoint = Vector3.up * 3f;
			transform.position = respawnPoint;
			_body.position = respawnPoint;
			_body.velocity = Vector3.zero;
			_body.angularVelocity = Vector3.zero;
		}
	}

	void UpdateSpawn(InputData input)
	{
		if (input.GetButton(ButtonFlag.SPAWN))
			Object.Runner.Spawn(_cubePrefab, transform.position + Vector3.up, transform.rotation);
	}

	public static void OnBoostCooldownChanged(Changed<MarbleController> changed)
	{
		changed.Behaviour.OnBoostCooldownChanged();
	}

	public void OnBoostCooldownChanged()
	{
		_boostAnim.StartBoostAnimation();
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(transform.position, _areaOfInterestRadius);
	}
}