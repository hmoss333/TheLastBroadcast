using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _Lamp_Anim_AC : MonoBehaviour {

	public GameObject Display_01;

	void Start () {

		StartCoroutine(Disp());
		Display_01.GetComponent<MeshRenderer> ().enabled = false;

	}

	IEnumerator Disp()
	{
		while(true)
		{
			Display_01.GetComponent<MeshRenderer> ().enabled = true;
			yield return new WaitForSeconds(1);
			Display_01.GetComponent<MeshRenderer> ().enabled = false;
			yield return new WaitForSeconds(1);

			yield return null;
	}
}
}
