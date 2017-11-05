using UnityEngine;
using System.Collections;

public class BulletShellProvider : ObjectPool {

	public static BulletShellProvider instance;

	void Awake()
	{
		instance = this;
	}
}
