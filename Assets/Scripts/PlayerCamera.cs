using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
	public Transform CameraTarget;

	private void LateUpdate()
	{
		if (CameraTarget == null)
		{
			return;
		}

		Vector3 targetPosition = CameraTarget.position;
		targetPosition.y = Mathf.Max(targetPosition.y, 0f);
		transform.position = targetPosition;
	}
}