using Fusion;
using UnityEngine;

public class PhysxBall : NetworkBehaviour
{
    [Networked] private TickTimer life { get; set; }
    public float power;

    public void Init(Vector3 forward, float powerInput)
    {
        life = TickTimer.CreateFromSeconds(Runner, 5.0f);
        power = powerInput;
        GetComponent<Rigidbody>().velocity = forward * power;
    }

    public override void FixedUpdateNetwork()
    {
        if (life.Expired(Runner))
            Runner.Despawn(Object);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<PlayerBola>())
        {
            Vector3 dir = (this.transform.position - collision.transform.position).normalized;
            collision.collider.GetComponent<PlayerBola>().AddImpact(dir);
        }
    }
}
