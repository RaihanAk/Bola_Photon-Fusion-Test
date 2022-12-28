# Hello Fusion

## Setup

### Allow 'unsafe' Code
1. Edit -> Project Settings -> Player -> Other Settings
	=> enable "Allow 'unsafe' Code"

### Create an AppId
1. Go to https://dashboard.photonengine.com/
2. Select "Create A New App"
3. Select Photon Type Fusion, and enter the name, etc. and create.
4. Copy the created AppId.
5. Select Assets/Photon/Fusion/Resources/PhotonAppSettings.asset in the project.
6. Paste the AppId into "App Id Realtime"

## Program Flow

1. Connect Client / Create Server / Host -> NetworkDebugStart.cs
		This Instantiates a NetworkRunner prefab.
2. Spawn Player -> PlayerSpawner.cs: When the Spawned callback is called by fusion the local client sends an RPC to the server to be spawned. The RPC could also contain additional information, eg. the character or faction that was selected by the user.
					Input Authority is assigned to the corresponding PlayerRef
					(In Client Authority mode the local client can spawn itself)
3. Player.cs -> Spawned() is a callback invoked by Fusion.
			If  the Local client has Input authority we find the LocalPlayerSystem. This holds things like the Input Scripts, Cameras, Canvases, etc.

4. GameLoop
	4.1 InputHandler.cs -> Read the Users Input and save it to the inputContainer
	4.2 CharacterController.cs -> FixedNetworkedUpdate() -> Read the Input back and react.

5. Despawn -> "PlayerLeft" event in PlayerSpawner.cs


## InputData

Input events are split into two categories: "Actions" and "States"
* States are true as long as a button is pressed
* Actions are only true for one frame when the button was pressed.
	=> To detect Actions without loss of data we flip bits and detect those flips after receiving the input. 
	Packetloss at the wrong time would mean we miss the input entirely. By flipping the bit it is still sent with the next input and just detected a tick later.
	To preprocess the data we keep a copy of the previously received actions in Character.cs (see: void PreprocessInput(ref InputData input);)
	We need to save the previous actions in a [Networked] property so it can be rolled back for prediction and resimulation.

