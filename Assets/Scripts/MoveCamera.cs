 	using UnityEngine;
using System.Collections;

public class MoveCamera : MonoBehaviour {

	public float cameraSpeed = 1.0f;
	public float scrollSpeed = 1.0f;
	public float minZoom, maxZoom;
	
	// Update is called once per frame
	void Update () {
		if(Camera.main.orthographic)
		{
			float camSize = Camera.main.orthographicSize;
			float scrollInput = -Input.GetAxis("Mouse ScrollWheel");

			if(camSize + scrollInput < minZoom) 
			{
				camSize = minZoom;
			}
			else if (camSize + scrollInput > maxZoom)
			{
				camSize = maxZoom;
			}
			else
				camSize += scrollInput;

			Camera.main.orthographicSize = camSize;
		}
		else
		{
			Vector3 oldPos = Camera.main.transform.position;
			Camera.main.transform.position = new Vector3(oldPos.x, oldPos.y, oldPos.z + (scrollSpeed * Input.GetAxis("Mouse ScrollWheel")));
		}

		transform.position += Vector3.up * Input.GetAxis("Vertical") * cameraSpeed;
		transform.position += Vector3.right * Input.GetAxis("Horizontal") * cameraSpeed;
	}
}
