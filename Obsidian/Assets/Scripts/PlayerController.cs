﻿using UnityEngine;

[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(PlayerMotor))]

public class PlayerController : MonoBehaviour {

	[SerializeField]
	private float speed = 5f;
	[SerializeField]
	private float lookSensitivity = 3f;

	[SerializeField]
	private float thrusterForce = 1000f;

	// Creates header in inspector
	[Header("Spring settings:")]
	[SerializeField]
	private float jointSpring = 20f;
	[SerializeField]
	private float jointMaxForce = 40f;

	private PlayerMotor motor;
	private ConfigurableJoint joint;

	void Start() {
		motor = GetComponent<PlayerMotor>();
		joint = GetComponent<ConfigurableJoint>();

		SetJointSettings(jointSpring);
	}

	void Update() {
		// Calculate movement velocity as a 3D vector
		float _xMove = Input.GetAxis("Horizontal");
		float _zMove = Input.GetAxis("Vertical");

		Vector3 _moveHorizontal = transform.right * _xMove; // (+1/-1, 0, 0)
		Vector3 _moveVertical = transform.forward * _zMove; // (0, 0, +1/-1)

		// Final movement vector
		Vector3 _velocity = (_moveHorizontal + _moveVertical) * speed;

		// Apply movement
		motor.Move(_velocity);

		// Calculate rotation as a 3D vector (turning around)
		float _yRot = Input.GetAxisRaw("Mouse X");

		Vector3 _rotation = new Vector3(0f, _yRot, 0f) * lookSensitivity;

		// Apply rotation
		motor.Rotate(_rotation);

		// Calculate rotation as a 3D vector (turning around)
		float _xRot = Input.GetAxisRaw("Mouse Y");

		float _cameraRotationX = _xRot * lookSensitivity;

		// Apply rotation
		motor.RotateCamera(_cameraRotationX);

		// Calculate thruster force based on player input
		Vector3 _thrusterForce = Vector3.zero;

		if (Input.GetButton("Jump")) {
			_thrusterForce = Vector3.up * thrusterForce;
			SetJointSettings(0f);
		}

		// Apply thruster force
		motor.ApplyThruster(_thrusterForce);
	}

	// Struct
	private void SetJointSettings(float _jointSpring) {
		joint.yDrive = new JointDrive { 
			positionSpring = jointSpring, 
			maximumForce = jointMaxForce
		};
	}

}
