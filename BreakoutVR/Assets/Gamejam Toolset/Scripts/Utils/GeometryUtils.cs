using UnityEngine;
using System.Collections;

public class GeometryUtils : MonoBehaviour {

	public static bool IsPointInsideCircle(Vector2 circleMiddle, float circleRadius, Vector2 point)
	{
		return Vector2.Distance(circleMiddle, point) <= circleRadius;
	}
}
