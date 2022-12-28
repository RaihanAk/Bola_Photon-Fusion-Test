using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
	[SerializeField] private Player _playerPrefab;

	InputData _data;

	private void Update() 
	{
		// Check events like KeyDown or KeyUp in Unity's update. They might be missed otherwise because they're only true for 1 frame
		_data.ButtonFlags |= Input.GetKeyDown( KeyCode.R ) ? ButtonFlag.RESPAWN : 0;
		_data.ButtonFlags |= Input.GetKeyDown( KeyCode.Space ) ? ButtonFlag.BOOST : 0;
		_data.ButtonFlags |= Input.GetKeyDown( KeyCode.P ) ? ButtonFlag.SPAWN : 0;
	}

	public void OnInput(NetworkRunner runner, NetworkInput input)
	{
		// Persistent button flags like GetKey should be read when needed so they always have the actual state for this tick
		_data.ButtonFlags |= Input.GetKey( KeyCode.W ) ? ButtonFlag.FORWARD : 0;
		_data.ButtonFlags |= Input.GetKey( KeyCode.A ) ? ButtonFlag.LEFT : 0;
		_data.ButtonFlags |= Input.GetKey( KeyCode.S ) ? ButtonFlag.BACKWARD : 0;
		_data.ButtonFlags |= Input.GetKey( KeyCode.D ) ? ButtonFlag.RIGHT : 0;

		input.Set( _data );

		// Clear the flags so they don't spill over into the next tick unless they're still valid input.
		// Note that in this example there is no need to split these flags in two, except maybe for clarity.
		_data.ButtonFlags = 0;
	}

	public void OnConnectedToServer(NetworkRunner runner)
	{
		if(runner.Topology==SimulationConfig.Topologies.Shared)
			runner.Spawn( _playerPrefab, null, null, runner.LocalPlayer );
	}

	public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
	{
		if(runner.IsServer)
			runner.Spawn( _playerPrefab, null, null, player );
	}

	public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
	public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
	public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
	public void OnDisconnectedFromServer(NetworkRunner runner) { }
	public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
	public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
	public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
	public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
	public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
	public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
	public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
	public void OnSceneLoadDone(NetworkRunner runner) { }
	public void OnSceneLoadStart(NetworkRunner runner) { }
}
