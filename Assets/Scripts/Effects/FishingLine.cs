using UnityEngine;

public class FishingLine : MonoBehaviour
{
	[SerializeField] private Transform endPoint;

	private LineRenderer _lineRenderer;

	private void Awake()
	{
		_lineRenderer = GetComponent<LineRenderer>();
	}

	private void Update()
	{
		if (_lineRenderer == null || endPoint == null) return;

		_lineRenderer.positionCount = 2;
		_lineRenderer.SetPosition(0, transform.position);
		_lineRenderer.SetPosition(1, endPoint.position);
	}
}