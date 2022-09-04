using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _Display_Anim_HRT : MonoBehaviour {

	public GameObject Display_01;
	public GameObject Display_02;
	public GameObject Display_03;
	public GameObject Display_04;
	public GameObject Display_05;

	void Start () {

		StartCoroutine(Disp());
		Display_01.GetComponent<MeshRenderer> ().enabled = false;
		Display_02.GetComponent<MeshRenderer> ().enabled = false;
		Display_03.GetComponent<MeshRenderer> ().enabled = false;
		Display_04.GetComponent<MeshRenderer> ().enabled = false;
		Display_05.GetComponent<MeshRenderer> ().enabled = false;

	}

	IEnumerator Disp()
	{
		while(true)
		{
			Display_01.GetComponent<MeshRenderer> ().enabled = true;
			Display_02.GetComponent<MeshRenderer> ().enabled = false;
			Display_03.GetComponent<MeshRenderer> ().enabled = false;
			Display_04.GetComponent<MeshRenderer> ().enabled = false;
			Display_05.GetComponent<MeshRenderer> ().enabled = false;
			yield return new WaitForSeconds(1);
			Display_01.GetComponent<MeshRenderer> ().enabled = false;
			Display_02.GetComponent<MeshRenderer> ().enabled = true;
			Display_03.GetComponent<MeshRenderer> ().enabled = false;
			Display_04.GetComponent<MeshRenderer> ().enabled = false;
			Display_05.GetComponent<MeshRenderer> ().enabled = false;
			yield return new WaitForSeconds(1);
			Display_01.GetComponent<MeshRenderer> ().enabled = false;
			Display_02.GetComponent<MeshRenderer> ().enabled = false;
			Display_03.GetComponent<MeshRenderer> ().enabled = true;
			Display_04.GetComponent<MeshRenderer> ().enabled = false;
			Display_05.GetComponent<MeshRenderer> ().enabled = false;
			yield return new WaitForSeconds(0.25f);
			Display_01.GetComponent<MeshRenderer> ().enabled = false;
			Display_02.GetComponent<MeshRenderer> ().enabled = false;
			Display_03.GetComponent<MeshRenderer> ().enabled = false;
			Display_04.GetComponent<MeshRenderer> ().enabled = true;
			Display_05.GetComponent<MeshRenderer> ().enabled = false;
			yield return new WaitForSeconds(1);
			Display_01.GetComponent<MeshRenderer> ().enabled = false;
			Display_02.GetComponent<MeshRenderer> ().enabled = false;
			Display_03.GetComponent<MeshRenderer> ().enabled = false;
			Display_04.GetComponent<MeshRenderer> ().enabled = false;
			Display_05.GetComponent<MeshRenderer> ().enabled = true;
			yield return new WaitForSeconds(0.05f);

			yield return null;
	}
}
}
