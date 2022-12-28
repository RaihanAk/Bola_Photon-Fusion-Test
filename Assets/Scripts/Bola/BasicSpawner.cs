using Fusion;
using Fusion.Sockets;
using Fusion.Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] 
    private NetworkPrefabRef _playerPrefab;
    
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    private NetworkRunner _runner;
    private bool _mouseButton0;
    private bool _mouseButton1;
    private bool _shift;
    private bool _space;

    private void Update()
    {
        _mouseButton0 = _mouseButton0 | Input.GetMouseButton(0);
        _mouseButton1 = _mouseButton1 || Input.GetMouseButton(1);
        _shift = _shift || Input.GetKey(KeyCode.LeftShift);
        _space = _space | Input.GetKey(KeyCode.Space);
    }

    public async void StartGame(GameMode mode, string sessionName = "TestRoom", string password = "")
    {
        Debug.Log("starting... with sessionName:" + sessionName + " and password:" + password);
        // Create the Fusion runner and let it know that we will be providing user input
        _runner = gameObject.AddComponent<NetworkRunner>(); 
        _runner.ProvideInput = true;

        //await _runner.JoinSessionLobby(SessionLobby.ClientServer, "TestLobby");

        // Add password protection
        var customProps = new Dictionary<string, SessionProperty>();

        customProps["pwd"] = (string)password;


        // Start or join (depends on gamemode) a session with a specific name
        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,    
            CustomLobbyName = "BolaLobby",
            SessionName = sessionName,
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneObjectProvider = gameObject.AddComponent<NetworkSceneManagerDefault>(),
            SessionProperties = customProps
        });

        if (mode == GameMode.Server)
        {
            
        }

        _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();
    }
    public void OnClickDC()
    {
        _runner.Shutdown(false, ShutdownReason.Ok);
        Destroy(gameObject.GetComponent<NetworkRunner>());
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {

    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();

        if (Input.GetKey(KeyCode.W)) 
            data.direction += Vector3.forward;

        if (Input.GetKey(KeyCode.S))
            data.direction += Vector3.back;

        if (Input.GetKey(KeyCode.A))
            data.direction += Vector3.left;

        if (Input.GetKey(KeyCode.D))
            data.direction += Vector3.right;

        if (_mouseButton0)
            data.buttons |= NetworkInputData.MOUSEBUTTON1;
        _mouseButton0 = false;

        if (_mouseButton1)
            data.buttons |= NetworkInputData.MOUSEBUTTON2;
        _mouseButton1 = false;

        if (_shift)
            data.buttons |= NetworkInputData.SHIFT;
        _shift = false;

        if (_space)
            data.buttons |= NetworkInputData.SPACE;
        _space = false;

        data.extraButtons.Set(ExtraButtonsEnum.WALL, Input.GetKey(KeyCode.Q));

        input.Set(data);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        // Create a unique position for the player
        Vector3 spawnPosition = new Vector3((player.RawEncoded % runner.Config.Simulation.DefaultPlayers) * 3, 1, 0);
        NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);
        
        // Keep track of the player avatars so we can remove it when they disconnect
        _spawnedCharacters.Add(player, networkPlayerObject);
        Debug.Log("spawned " + player.ToString());
        Debug.Log("localplayer is " + runner.Simulation.LocalPlayer);

        // Debug prop pwd
        Debug.Log("this session " + runner.SessionInfo.Name + " pwd: " + runner.SessionInfo.Properties["pwd"]);
        if (runner.SessionInfo.Properties["pwd"] != "")
        {

        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        // Find and remove the players avatar
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
        }
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
        
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
     
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        for (int i = 0; i < sessionList.Count; i++)
        {
            Debug.Log("SESSI " + i + ": " + sessionList[i].Name);
        }
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        
    }
}
