using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _Lamp_2_Anim_VFO : MonoBehaviour {

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
			yield return new WaitForSeconds(0.5f);
			yield return null;
	}
}
}
