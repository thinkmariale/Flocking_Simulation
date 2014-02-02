using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 *  Flocking simulation class:
 *	By: Maria Alejandra Montenegro
 *		 2013
 *		 
 *  Made with the help of sources:
 * 		http://libcinder.org/docs/dev/flocking_chapter3.html
 * 		http://gamedev.tutsplus.com/tutorials/implementation/the-three-simple-rules-of-flocking-behaviors-alignment-cohesion-and-separation/
 * 		
 * */

public class Flocking : MonoBehaviour {
	// simulation variables
	Vector3 separationTotal = Vector3.zero;
	Vector3 cohesionTotal   = Vector3.zero;
	Vector3 alignmentTotal  = Vector3.zero;
	Vector3 boundTotal      = Vector3.zero;
	
	//Moving Cloud
	public List<GameObject> birdsList; //all birds in sim
	public MovingCloud cloud;     // cloud birds in
	
	private Birds bird;
	int separationCount = 0;
	int cohesionCount   = 0;
	int alignmentCount  = 0;
	int boundCount      = 0;
	
	void Awake() {
		bird = GetComponent<Birds>();
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	}
	//Physics should ve applyed every frame
	void FixedUpdate() {
		
		Vector3 newVel = Vector3.zero;
		// calculate flocking
		Flock();
		newVel += separationTotal*bird.separationW + cohesionTotal*bird.cohesionW 
			+ alignmentTotal*bird.alignmentW + boundTotal*bird.boundW;
		
		newVel = newVel*bird.speed;
		newVel = rigidbody.velocity + newVel;
		
		//newVel.y = 0.0f; // as in MovingCloud , we not using Y axis
		rigidbody.velocity = Limit(newVel, bird.maxSpeed);	
		
		//float base_ = separationCount+cohesionCount+alignmentCount;
		//float x = separationCount / base_;
		//float y = cohesionCount /base_;
		//float z = alignmentCount/base_;
		//Color color = new Color(rigidbody.velocity.x,rigidbody.velocity.y,rigidbody.velocity.z);
		//Debug.Log(color);
		//renderer.material.color = Color.Lerp(renderer.material.color, color,Mathf.PingPong(Time.time, 2));
		
	}
	
	//Function that loops through all birds and determines and calculates 
	//if separation,cohesion, alignment, or none should be applied.
	void Flock() {
		
		// keep record how many birds do what and velocity sum
		separationCount = 0;
		cohesionCount   = 0;
		alignmentCount  = 0;
		boundCount      = 0;
		
		Vector3 separationDif = Vector3.zero;
		Vector3 cohesionDif   = Vector3.zero;
		Vector3 alignmentDif  = Vector3.zero;
		Vector3 boundDif      = Vector3.zero;
		
		//loop all birds
		for(int i = 0;i<birdsList.Count;i++) { 
			if (birdsList[i] == null) continue;
			
			GameObject b_ = birdsList[i];
			Vector3 direction = b_.transform.position - transform.position;
			float distance = direction.magnitude; //distance to neighbor
			
			if(distance > 0) { //not itself
				// Separation
				// avoid crowding neighbors // calculate separation if less than threshhold
				if(distance < bird.separationRadius) {
					direction.Normalize();
					direction = direction / distance; //weight by distance
					separationDif += direction;
					separationCount++;
				}
			
				// Cohesion  (avoid isolation!)
				// street towards average pos. of neighbors
				if(distance < bird.cohesionRadius) {
					cohesionDif += b_.transform.position;
					cohesionCount++;
				}
				
				// Alignment
				// street towards average heading of neighbors if bird is considered neighbor (bc in rad) then allign
				if(distance < bird.alignmentRadius) {
					alignmentDif += b_.rigidbody.velocity;
					alignmentCount++; //neighbor count :)
				}
				
				// Bounds
				// calculate bounds of "Our World" specified in MovingCloud.
				//Make sure we DONT get out of "Our World"
				Bounds bounds = new Bounds(new Vector3(cloud.transform.position.x,cloud.transform.position.y,cloud.transform.position.z), new Vector3(cloud.moveBound.x, 30f, cloud.moveBound.y));
				if (distance < bird.cohesionRadius && !bounds.Contains(b_.transform.position))
				{
					Vector3 dif = transform.position - cloud.transform.position;
					if (dif.magnitude > 0)
					{
						boundDif += cloud.transform.position;
						boundCount++;
					}
				}
			} //if not self
		}//all birds
		
		// end
		separationTotal = separationCount > 0 ? separationDif / separationCount : separationDif;
		cohesionTotal = cohesionCount > 0 ? Steer(cohesionDif / cohesionCount, false) : cohesionDif;
		alignmentTotal = alignmentCount > 0 ? Limit(alignmentDif / alignmentCount, bird.maxSteer) : alignmentDif;
		boundTotal = boundCount > 0 ? Steer(boundDif / boundCount, false) : boundDif;
	
	}
	
	// Function that makes limit of the magnitude 
	//of a vector to the specified max (keep it controlled)
	Vector3 Limit (Vector3 vec, float max){
		if(vec.magnitude > max)
			return vec.normalized * max;
		else 
			return vec;
	}
	
	
	//Function controls steering of bird path
	Vector3 Steer(Vector3 target, bool slowDown)
	{
		// the steering vector
		Vector3 steer = Vector3.zero;
		Vector3 targetDirection = target - transform.position;
		float targetDistance = targetDirection.magnitude;

		transform.LookAt(target);

		if (targetDistance > 0)
		{
			// move towards the target
			targetDirection.Normalize();

			// we have two options for speed:
			if (slowDown && targetDistance < 100f * bird.speed)
			{
				targetDirection *= (bird.maxSpeed * targetDistance / (100f * bird.speed));
				targetDirection *= bird.speed;
			}
			else
				targetDirection *= bird.maxSpeed;

			// set steering vector
			steer = targetDirection - rigidbody.velocity;
			steer = Limit(steer, bird.maxSteer);
		}

		return steer;
	}

	
}
