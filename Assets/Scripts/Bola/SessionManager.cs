using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SessionManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public GameObject canvas;
    public InputField roomInputField;
    public InputField pwdInputField;

    public Transform sessiListParent;
    public GameObject sessiButton;
    public InputField sessiPwdInput;
    public Button sessiPwdSubmitButton;


    [SerializeField]
    private NetworkRunner _runner;
    [SerializeField]
    private BasicSpawner basicSpawner;

    private void Start()
    {
        canvas.SetActive(false);
        JoinLobby(_runner);
    }

    public void OnClickStartGame(int gameModeType = 0)
    {
        switch (gameModeType)
        {
            case 0:
                // Host
                StartGame(GameMode.Host, 
                    (roomInputField.text != "" ? roomInputField.text : "TestRoom"),
                    (pwdInputField.text != "" ? pwdInputField.text : "")
                    );
                break;
            case 1:
                StartGame(GameMode.Server,
                    (roomInputField.text != "" ? roomInputField.text : "TestRoom"),
                    (pwdInputField.text != "" ? pwdInputField.text : "")
                    );
                break;
        }
    }

    public void OnClickCheckPwd(Button sessi, InputField inputPwd, string sessiPwd)
    {
        Debug.Log("checking input:" + inputPwd.text + " pwd:" + sessiPwd);
        if (inputPwd.text == sessiPwd)
            sessi.interactable = true;

    }

    public void StartGame(GameMode mode, string sessionName = "TestRoom", string password = "")
    {
        basicSpawner.StartGame(mode, sessionName, password);
        
        //canvas.SetActive(false);
    }

    async void JoinLobby(NetworkRunner runner)
    {
        var result = await runner.JoinSessionLobby(SessionLobby.Custom, "BolaLobby");

        if (result.Ok)
        {
            Debug.Log("Succes joined ");
            canvas.SetActive(true);
        }
        else
        {
            Debug.LogError($"Failed to Start: {result.ShutdownReason}");
        }
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        if (sessionList.Count <= 0)
            return;

        Debug.Log("sessi updated: sessi:" + sessionList[0].Name);
        //clear
        //if (sessiListParent.childCount > 0)
        //{
        //    foreach (SessiListItem child in sessiListParent)
        //        Destroy(child.gameObject);
        //}


        foreach (var sessi in sessionList)
        {
            GameObject o = Instantiate(sessiButton, sessiListParent);

            o.GetComponent<SessiListItem>().joinSessionButton.onClick.AddListener(() => StartGame(
                GameMode.Client,
                sessi.Name));
            o.GetComponent<SessiListItem>().sessionText.text = "Join " + sessi.Name;

            if (sessi.Properties["pwd"] != "")
            {
                o.GetComponent<SessiListItem>().joinSessionButton.interactable = false;

                o.GetComponent<SessiListItem>().cekPwdButton.onClick.AddListener(() => OnClickCheckPwd(
                    o.GetComponent<Button>(),
                    o.GetComponentInChildren<InputField>(),
                    sessi.Properties["pwd"]));
            }
            else
            {
                o.GetComponent<SessiListItem>().cekPwdButton.gameObject.SetActive(false);
                o.GetComponent<SessiListItem>().sessiPwdInput.gameObject.SetActive(false);
            }
        }
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
        Debug.Log("succesfully dc ed");
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        
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

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        
    }
}
