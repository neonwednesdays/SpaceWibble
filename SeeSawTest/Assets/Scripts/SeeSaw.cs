﻿using UnityEngine;
using System.Collections;

public class SeeSaw : MonoBehaviour {
	public GameObject seesaw;

	private Vector3 centerPoint;//location of the pivot
	public float rotationPower;//multiplicative power of rotation blah
	public Vector3 axis;

	private ArrayList leftObjects;
	private ArrayList rightObjects;
	private string[] tagsThatAffectSeesaw;

	public enum seesawTypes {evenRotation, weightBased, weightDifference};
	public seesawTypes seesawType = seesawTypes.evenRotation;
	// Use this for initialization
	void Start () {
		centerPoint = seesaw.transform.position;
		tagsThatAffectSeesaw = new string[]{"AffectSeesaw", "Player"};
		leftObjects = new ArrayList();
		rightObjects = new ArrayList();
	}
	
	// Update is called once per frame
	void Update () {
//		Debug.Log ("Left Size: " + leftObjects.Count + " vs Right Size: " + rightObjects.Count);
		if (leftObjects.Count > 0 || rightObjects.Count > 0) {
				float leftWeight = 0;
				float rightWeight = 0;
				foreach (GameObject gameobj in leftObjects) {
						leftWeight = leftWeight + getDist (centerPoint, gameobj.transform.position);
				}
				foreach (GameObject gameobj in rightObjects) {
						rightWeight = rightWeight + getDist (centerPoint, gameobj.transform.position);
				}
				Debug.Log ("Left Weight: " + leftWeight + " vs Right Weight: " + rightWeight);

				axis = Vector3.zero;
				float weightPower = 0;
				switch (seesawType) {
				case seesawTypes.evenRotation://rotates the same amount regardless of weight
//						print ("evenrot");
						if (Mathf.Abs (leftWeight) > Mathf.Abs (rightWeight)) {//left heavy
								axis = new Vector3 (-1, 0, 0);
						} else {//right heavy
								axis = new Vector3 (1, 0, 0);
						}
						weightPower = 1;
						break;
				case seesawTypes.weightBased://rotates the heaviest side by amount based on weight
						if (Mathf.Abs (leftWeight) > Mathf.Abs (rightWeight)) {//left heavy
								axis = new Vector3 (-1, 0, 0);
								weightPower = Mathf.Abs (leftWeight);
						} else {//right heavy
								axis = new Vector3 (1, 0, 0);
								weightPower = Mathf.Abs (rightWeight);
						}
						break;
				case seesawTypes.weightDifference://rotates the heaviest side by the difference between both sides
//						print ("weightdiff");
						if (Mathf.Abs (leftWeight) > Mathf.Abs (rightWeight)) {//left heavy
								axis = new Vector3 (-1, 0, 0);
								weightPower = Mathf.Abs (leftWeight - rightWeight);
						} else {//right heavy
								axis = new Vector3 (1, 0, 0);
								weightPower = Mathf.Abs (rightWeight - leftWeight);
						}
						break;
				}
			if(Mathf.Abs(weightPower) < .5){
				weightPower = 0;
			}
			seesaw.transform.RotateAround (centerPoint, axis, weightPower * rotationPower * Time.deltaTime);
		}
	}

	void OnTriggerEnter(Collider col){
		if (isAffectsSeesaw(col.transform.tag)) {
			int dir = getObjDirection(col.gameObject.transform.position);
			if(dir > 0){//right 
				rightObjects.Add(col.gameObject);
			}else if(dir < 0){
				leftObjects.Add (col.gameObject);
			}
		}
	}
	//
	void OnTriggerExit(Collider col){
		if (isAffectsSeesaw(col.transform.tag)) {
			int dir = getObjDirection(col.gameObject.transform.position);
			if(dir > 0){//right 
				rightObjects.Remove(col.gameObject);
			}else if(dir < 0){
				leftObjects.Remove(col.gameObject);
			}
		}
	}

	//gets the distance, in case we like switch formulas or normalize these things
	float getDist(Vector3 a, Vector3 b){
		return (b.z - a.z);
	}

	//returns 1 if right of seesaw center, -1 if left
	int getObjDirection(Vector3 objPoint){
		//Vector3 objPos = objPoint.gameObject.transform.position;
		float dist = getDist(objPoint, centerPoint);
		if(dist != 0){
			int dir = dist > 0 ? -1:1;
			return dir;
		}else{
			return 0;
		}
	}
	//is this one of the tags that affects the seesaw
	bool isAffectsSeesaw(string tagName){
		foreach(string item in tagsThatAffectSeesaw){
			if(tagName == item){
				return true;
			}
		}
		return false;
	}
}


