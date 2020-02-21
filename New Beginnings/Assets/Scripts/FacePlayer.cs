using UnityEngine;
using System.Collections;

public class FacePlayer : MonoBehaviour
{
	[Header("Turn to face player speed")]
	public float turnSpeed = 1.0f;

	[Header("If set, will only rotate left/right")]
	public bool rotateOnlyLR = true;

	private Player player = null;

	void Awake()
	{
		player = FindObjectOfType<Player>();
	}//Awake

	void FaceTowardsPlayer()
	{
		Vector3 lookPos = player.transform.position - transform.position;
		if(rotateOnlyLR)
			lookPos.y = 0;
		
		Quaternion rotation = Quaternion.LookRotation(lookPos);
		transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * turnSpeed);
	}//FacePlayer



	void Update()
	{
		FaceTowardsPlayer();
	}//
}
