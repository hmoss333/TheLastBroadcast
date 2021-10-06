using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _Lamp_1_Anim_MS : MonoBehaviour {

	public GameObject Lamp;

	void Start () {

		StartCoroutine(Disp());
		Lamp.GetComponent<MeshRenderer> ().enabled = false;
	}

	IEnumerator Disp()
	{
		while(true)
		{
			Lamp.GetComponent<MeshRenderer> ().enabled = true;
			yield return new WaitForSeconds(0.1f);
			Lamp.GetComponent<MeshRenderer> ().enabled = false;
			yield return new WaitForSeconds(1.0f);
			yield return null;
	}
}
}
