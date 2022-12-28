using UnityEngine;
using Fusion;

public class Player : NetworkBehaviour, IPlayerLeft
{
    public static Player Local { get; set; }

    [SerializeField] private Transform _cameraTarget;

    public override void Spawned()
    {
        if( Object.HasInputAuthority )
        {
            Local = this;
            PlayerCamera camera = FindObjectOfType<PlayerCamera>();
            camera.CameraTarget = _cameraTarget;
        }
    }

    public void PlayerLeft( PlayerRef player )
    {
        if( player == Object.InputAuthority )
        {
            Runner.Despawn( Object );
        }
    }
}