using UnityEngine;
using System.Collections;
using Leap;

public class MovementController : MonoBehaviour {
	Controller leapController;
	bool handOpenedLF=false,handOpenedTF=false;
	GameObject carrObject;

	// Use this for initialization
	void Start () {
		leapController = new Controller ();
	}

	Hand getForemostHand(){
		Frame f = leapController.Frame ();
		if (f.Hands.Count > 0) {
			return f.Hands [0];
		} else {
			return null;
		}
	}

	void move(Hand h){
		if (h.PalmPosition.z > 3.0f) {
			transform.position -= transform.forward * 0.1f;
		} else if (h.PalmPosition.z < -3.0f) {
			transform.position += transform.forward * 0.1f;
		}
	}

	void rotateY(Hand h){
		float handX = h.PalmPosition.x;
		if (Mathf.Abs (handX) > 3.0f) {
			//transform.RotateAround (Vector3.up, handX * 0.03f);
			transform.Rotate (0, handX * 0.03f, 0);
		}
	}

	void roatateX(Hand h){
		float handY = h.PalmPosition.y;
		if (Mathf.Abs (handY) > 8.0f) {
			//transform.RotateAround (Vector3.up, handX * 0.03f);
			transform.Rotate (-handY * 0.003f,0, 0);
		}
	}

	bool isHandOpened(Hand h)	{
		//Debug.Log (h.GrabStrength);

		return (h.GrabStrength != 1);
	}

	void onHandOpen(Hand h) {
		carrObject = null;
	}

	void onHandClose(Hand h) {
		RaycastHit hit;
		if (Physics.SphereCast (new Ray (transform.position + transform.forward * 2.0f, transform.forward), 2.0f, out hit)) {
			if(hit.collider.gameObject.CompareTag("pickable")){
				carrObject = hit.collider.gameObject;
			}
		}
	}

	void handCallBacks(Hand h)	{
		if (handOpenedTF && !handOpenedLF) {
			onHandOpen (h);
		}
		if (!handOpenedTF && handOpenedLF) {
			onHandClose (h);
		}
	}

	void moveCarrObject()	{
		if (null != carrObject) {
			Vector3 targetPos = transform.position + new Vector3 (transform.forward.x, 0, transform.forward.z) * 2.0f;
			Vector3 deltaVec = targetPos - carrObject.transform.position;
			if (deltaVec.magnitude > 0.1f) {
				carrObject.GetComponent<Rigidbody> ().velocity = deltaVec * 5.0f;
			} else {
				carrObject.GetComponent<Rigidbody> ().velocity = Vector3.zero;
			}
		}
	}
	// Update is called once per frame
	void FixedUpdate () {
		Hand h = getForemostHand ();
		if (h != null) {
			move (h);
			handOpenedTF = isHandOpened (h);
			//Debug.Log (handOpenedTF);
			//roatateX (h);
			rotateY (h);
			handCallBacks (h);
			moveCarrObject ();
		}
		handOpenedLF = handOpenedTF;
	}
}
