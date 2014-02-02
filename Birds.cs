using UnityEngine;
using System.Collections;

/*
 *  Birds class for flocking simulation:
 *	By: Maria Alejandra Montenegro
 *		 2013
	
 * */
public class Birds: MonoBehaviour {
	
	//speed and steer
	public float speed = 5f;
	public float maxSpeed = 10f;
	public float maxSteer = 0.03f;
	
	// flock radius
	public float separationRadius = 0.5f;
	public float cohesionRadius   = 10f;
	public float alignmentRadius  = 6f;
	
	// flock weight influence
	public float separationW = 1f;
	public float cohesionW   = 1f;
	public float alignmentW  = 1f;
	public float boundW      = 1.2f;
	
	public GameObject poopPreFab;
	private float poopTime;
	private float poopCounter = 0;
	
	// Use this for initialization
	void Start () {
		if (poopPreFab == null)
		{
			// end early
			Debug.Log("ERROR, no poop samples."); 
			return;
		}
		
		renderer.material.color = new Color(Random.Range(0.6f,1.0f),Random.Range(0.6f,1.0f),Random.Range(1.6f,1.0f));
		poopTime = Random.Range(8,25);
	}
	
	// Update is called once per frame
	void Update () {
		poopCounter += Time.deltaTime;
		//print (poopCounter);
		
		  if(poopTime <= poopCounter){
			
			GameObject poopInstance = (GameObject)GameObject.Instantiate(poopPreFab);
			BirdsPoop temp = poopInstance.GetComponent<BirdsPoop>();
			temp.transform.position = transform.position;
            temp.rigidbody.AddForce(transform.forward * 8);
			Destroy(temp.gameObject,temp.lifeSpam);
			poopCounter = 0;
			
		}
	}
}
