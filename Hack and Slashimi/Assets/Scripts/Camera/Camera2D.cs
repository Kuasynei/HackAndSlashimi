using UnityEngine;
using System.Collections;

public class Camera2D : MonoBehaviour 
{
	[SerializeField] CameraRail camRail;
	[SerializeField] Transform target;
	[SerializeField] float hLerpSpeed;
	[SerializeField] float vLerpSpeed;
	[SerializeField] float zLayer = -11;

	// Update is called once per frame
	void Update () 
	{
		if (camRail != null)
		{
			//Rail Movement
			Vector3 railPosition = camRail.ProjectPositionOnRail (target.position);

			//Horizontal Lerping
			Vector3 targetTransformH = new Vector3 (target.position.x, transform.position.y, zLayer);
			Vector3 currentTransformH = new Vector3 (transform.position.x, transform.position.y, zLayer);
			transform.position = Vector3.Lerp (currentTransformH, targetTransformH, hLerpSpeed * Time.deltaTime);

			//Vertical Lerping
			Vector3 targetTransformV = new Vector3 (transform.position.x, target.position.y / 2 + railPosition.y / 2, transform.position.z);
			Vector3 currentTransformV = new Vector3 (transform.position.x, transform.position.y, transform.position.z);
			transform.position = Vector3.Lerp (currentTransformV, targetTransformV, vLerpSpeed * Time.deltaTime);
		}
		else
		{
			//Horizontal Lerping
			Vector3 targetTransformH = new Vector3 (target.position.x, transform.position.y, zLayer);
			Vector3 currentTransformH = new Vector3 (transform.position.x, transform.position.y, zLayer);
			transform.position = Vector3.Lerp (currentTransformH, targetTransformH, hLerpSpeed * Time.deltaTime);

			//Vertical Lerping
			Vector3 targetTransformV = new Vector3 (transform.position.x, target.position.y, transform.position.z);
			Vector3 currentTransformV = new Vector3 (transform.position.x, transform.position.y, transform.position.z);
			transform.position = Vector3.Lerp (currentTransformV, targetTransformV, vLerpSpeed * Time.deltaTime);
		}
	}
}
