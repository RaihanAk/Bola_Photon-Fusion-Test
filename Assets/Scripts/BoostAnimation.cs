using System.Collections;
using UnityEngine;

public class BoostAnimation : MonoBehaviour
{
	Renderer _renderer;
	MaterialPropertyBlock _propertyBlock;

	[ColorUsage(false, true)] [SerializeField]
	Color _normalColor;

	[ColorUsage(false, true)] [SerializeField]
	Color _boostColor;

	private void Awake()
	{
		_renderer = GetComponentInChildren<Renderer>();
		_propertyBlock = new MaterialPropertyBlock();
		_renderer.GetPropertyBlock(_propertyBlock);
	}

	public void StartBoostAnimation()
	{
		StopAllCoroutines();
		StartCoroutine(BoostColorRoutine());
	}

	IEnumerator BoostColorRoutine()
	{
		Color color = _boostColor;

		float time = 0f;
		while (time < 1f)
		{
			time += Time.deltaTime;
			color = Color.Lerp(_boostColor, _normalColor, time);
			_propertyBlock.SetColor("_Color", color);
			_renderer.SetPropertyBlock(_propertyBlock);
			yield return null;
		}

		_propertyBlock.SetColor("_Color", _normalColor);
		_renderer.SetPropertyBlock(_propertyBlock);
	}
}