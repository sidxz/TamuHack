using UnityEngine;
using System.Collections;
using Leap;

public class MovementController : MonoBehaviour {
	public GameObject bookRead;
	Controller leapController;
	bool handOpenedLF=false,handOpenedTF=false;
	Vector zAxis = Vector.Zero;
	bool readingABook=false;
	bool closable = false;

	GameObject carrObject;
	public GameObject next;

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

	bool isHandOpened(Hand h)	{
		return (h.GrabStrength < 0.8f);
	}

	void onHandOpen(Hand h) {
		carrObject = null;
		zAxis = Vector.Zero;
	}

	void onHandClose(Hand h) {
		RaycastHit hit;
		if (Physics.SphereCast (new Ray (transform.position + transform.forward * 1.0f, transform.forward), 1.0f, out hit)) {
			if(hit.collider.gameObject.CompareTag("pickable")){
				carrObject = hit.collider.gameObject;
				//carrObject.transform.Translate (0, 2.0f, 0,Space.World);
				if (Vector.Zero==zAxis)
					zAxis = Vector.ZAxis;
			}
		}
	}

	void flipPage(Hand h)	{
		float handX = h.PalmPosition.x;
		if (handX > 3.0f) {
			AutoFlip other = (AutoFlip)next.GetComponent (typeof(AutoFlip));
			other.FlipLeftPage ();
		} else if(handX < -3.0f) {
			AutoFlip other = (AutoFlip)next.GetComponent (typeof(AutoFlip));
			other.FlipRightPage ();
		}
		float angle = 90.0f * h.Rotation.z;
		if (60 > angle) {
			closable = true;
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

	void moveCarrObject(Hand h)	{
		if (null != carrObject) {
			if (transform.position.z >= 0) {
				carrObject.transform.position = new Vector3 (transform.position.x, 3.0f, transform.position.z - 1.0f);
			} else {
				carrObject.transform.position = new Vector3 (transform.position.x, 3.0f, transform.position.z - 1.0f);
			}
			checkZAxis (h);
		}
	}
	void checkZAxis(Hand h)	{
		//float angleRadians = Vector.ZAxis.AngleTo (zAxis);
		float angle = 90.0f * h.Rotation.z;
		if (60 < angle) {
			Debug.Log (" The angle is : " + angle);
			bookRead.SetActive(true);
			readingABook = true;
			//float fadeTime = GameObject.Find ("EventSystem").GetComponent<Fading> ().BeginFade (1);
			//new WaitForSeconds (fadeTime);
		} else {
			bookRead.SetActive(false);
		}
	}

	void closeBook(Hand h)	{
		//float angleRadians = Vector.ZAxis.AngleTo (zAxis);
		float angle = 90.0f * h.Rotation.z;
		if (60 < angle) {
			Debug.Log (" The angle is : " + angle);
			bookRead.SetActive(false);
			readingABook = false;
			closable = false;
			//float fadeTime = GameObject.Find ("EventSystem").GetComponent<Fading> ().BeginFade (1);
			//new WaitForSeconds (fadeTime);
		} 
	}
	// Update is called once per frame
	void FixedUpdate () {
		Hand h = getForemostHand ();
		if (readingABook) {
			flipPage (h);
			if (closable)
				closeBook (h);
		}
		else if (h != null) {
			move (h);
			handOpenedTF = isHandOpened (h);
			rotateY (h);
			handCallBacks (h);
			moveCarrObject (h);
		}
		handOpenedLF = handOpenedTF;
	}
}
