using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour {

    public float moveSpeed;
    public bool isTalking = false;
	void Start () {
		
	}

	void Update () {

        if (isTalking) { }
        else
        {
            if (Input.GetKey(KeyCode.W))
            {
                //transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, new Vector3(0f, 0f, 180f), 0.2f);
                transform.right = Vector3.Lerp(transform.right, new Vector3(-1f, 0f, 0f), 0.2f);
                transform.position += new Vector3(0f, 1, 0f) * moveSpeed;
            }
            if (Input.GetKey(KeyCode.S))
            {
                //transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, Vector3.zero, 0.2f);
                transform.right = Vector3.Lerp(transform.right, new Vector3(1f, 0f, 0f), 0.2f);
                transform.position += new Vector3(0f, -1, 0f) * moveSpeed;
            }
            if (Input.GetKey(KeyCode.A))
            {
                // transform.eulerAngles = Vector3.Lerp(transform.localEulerAngles, new Vector3(0f, 0f, 270f), 0.2f);
                transform.up = Vector3.Lerp(transform.up, new Vector3(1f, 0f, 0f), 0.2f);
                transform.position += new Vector3(-1, 0f, 0f) * moveSpeed;
            }
            if (Input.GetKey(KeyCode.D))
            {
                //transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, new Vector3(0f, 0f, 90f), 0.2f);
                transform.up = Vector3.Lerp(transform.up, new Vector3(-1f, 0f, 0f), 0.2f);
                transform.position += new Vector3(1, 0f, 0f) * moveSpeed;
            }
        }

    }
}
