using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TouchScript.Gestures;
using TouchScript.Hit;

public class ModelGestureManager : MonoBehaviour {

	public TapGesture singleTap;
	public TapGesture doubleTap;
	public FlickGesture flickGesture;
	public TransformGesture transformGesture;
	private float startY = 180.0f;

	public Camera mainCamera;
	public GameObject model2;
	private Animator _animator;

	/*
	private Vector3 firstpoint; //change type on Vector3
	private Vector3 secondpoint;
	private float xAngle = 0.0f; //angle for axes x for rotation
	private float yAngle = 0.0f;
	private float xAngTemp = 0.0f; //temp variable for angle
	*/

	private float accelerometerUpdateInterval = 1.0f / 60.0f;
	// The greater the value of LowPassKernelWidthInSeconds, the slower the filtered value will converge towards current input sample (and vice versa).
	private float lowPassKernelWidthInSeconds  = 2.0f;
	// This next parameter is initialized to 2.0 per Apple's recommendation
	private float shakeDetectionThreshold = 4.0f;
	private float lowPassFilterFactor;
	private Vector3 lowPassValue  = Vector3.zero;
	private Vector3 acceleration;
	private Vector3 deltaAcceleration;

	void Start() {
		_animator = this.GetComponent<Animator> ();

		/*
		//Initialization our angles of model
		xAngle = 180.0f;
		yAngle = 0.0f;
		this.transform.rotation = Quaternion.Euler(yAngle, xAngle, 0.0f);
		*/

		lowPassFilterFactor = accelerometerUpdateInterval / lowPassKernelWidthInSeconds;
		shakeDetectionThreshold *= shakeDetectionThreshold;
		lowPassValue = Input.acceleration;

		flickGesture.Flicked += (object sender, System.EventArgs e) => 
		{
			model2.SetActive(true);
			this.gameObject.SetActive(false);
		};

		transformGesture.Transformed += (object sender, System.EventArgs e) => 
		{
			mainCamera.transform.position /= transformGesture.DeltaScale;
			this.transform.Rotate(new Vector3(0,1,0), transformGesture.DeltaPosition.x*-150.0f);
		};

		singleTap.Tapped += (object sender, System.EventArgs e) => 
		{
			PlaySingleTapAnimation();
		};

		doubleTap.Tapped += (object sender, System.EventArgs e) => 
		{
			PlayDoubleTapAnimation();
		};

	}

	void Update() {

		//Check count touches
		/*if(Input.touchCount > 0) {
			//Touch began, save position
			if(Input.GetTouch(0).phase == TouchPhase.Began) {
				firstpoint = Input.GetTouch(0).position;
				xAngTemp = xAngle;
			}

			//Move finger by screen
			if(Input.GetTouch(0).phase==TouchPhase.Moved) {
				secondpoint = Input.GetTouch(0).position;
				//Mainly, about rotate model. For example, for Screen.width rotate on 180 degree
				xAngle = xAngTemp + (secondpoint.x - firstpoint.x) * 180.0f / Screen.width;
				//Rotate camera
				this.transform.rotation = Quaternion.Euler(yAngle, xAngle, 0.0f);
			}
		}*/
			
		acceleration = Input.acceleration;
		lowPassValue = Vector3.Lerp(lowPassValue, acceleration, lowPassFilterFactor);
		deltaAcceleration = acceleration - lowPassValue;
		if (deltaAcceleration.sqrMagnitude >= shakeDetectionThreshold)
		{
			// Perform "shaking actions" here, with suitable guards in the if check above, if necessary to not fire again if they're already being performed.
			PlayShakedAnimation();
		}
	}

	private void PlaySingleTapAnimation(){
		_animator.SetTrigger ("SingleTap");
	}

	private void PlayDoubleTapAnimation(){
		_animator.SetTrigger ("DoubleTap");
	}

	private void PlayShakedAnimation(){
		_animator.SetTrigger ("Shaked");
	}
}
