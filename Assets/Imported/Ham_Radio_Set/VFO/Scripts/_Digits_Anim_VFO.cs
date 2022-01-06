using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _Digits_Anim_VFO : MonoBehaviour {

	public GameObject Digits_01;
	public GameObject Digits_02;
	public GameObject Digits_03;
	public GameObject Digits_04;
	public GameObject Digits_05;
	public GameObject Digits_06;
	public GameObject Digits_07;

	void Start () {

		StartCoroutine(Disp());
		Digits_01.GetComponent<MeshRenderer> ().enabled = false;
		Digits_02.GetComponent<MeshRenderer> ().enabled = false;
		Digits_03.GetComponent<MeshRenderer> ().enabled = false;
		Digits_04.GetComponent<MeshRenderer> ().enabled = false;
		Digits_05.GetComponent<MeshRenderer> ().enabled = false;
		Digits_06.GetComponent<MeshRenderer> ().enabled = false;
		Digits_07.GetComponent<MeshRenderer> ().enabled = false;
	}

	IEnumerator Disp()
	{
		while(true)
		{
			Digits_01.GetComponent<MeshRenderer> ().enabled = true;
			Digits_02.GetComponent<MeshRenderer> ().enabled = false;
			Digits_03.GetComponent<MeshRenderer> ().enabled = false;
			Digits_04.GetComponent<MeshRenderer> ().enabled = false;
			Digits_05.GetComponent<MeshRenderer> ().enabled = false;
			Digits_06.GetComponent<MeshRenderer> ().enabled = false;
			Digits_07.GetComponent<MeshRenderer> ().enabled = false;
			yield return new WaitForSeconds(1.0f);
			Digits_01.GetComponent<MeshRenderer> ().enabled = false;
			Digits_02.GetComponent<MeshRenderer> ().enabled = true;
			Digits_03.GetComponent<MeshRenderer> ().enabled = false;
			Digits_04.GetComponent<MeshRenderer> ().enabled = false;
			Digits_05.GetComponent<MeshRenderer> ().enabled = false;
			Digits_06.GetComponent<MeshRenderer> ().enabled = false;
			Digits_07.GetComponent<MeshRenderer> ().enabled = false;
			yield return new WaitForSeconds(1.0f);
			Digits_01.GetComponent<MeshRenderer> ().enabled = false;
			Digits_02.GetComponent<MeshRenderer> ().enabled = false;
			Digits_03.GetComponent<MeshRenderer> ().enabled = true;
			Digits_04.GetComponent<MeshRenderer> ().enabled = false;
			Digits_05.GetComponent<MeshRenderer> ().enabled = false;
			Digits_06.GetComponent<MeshRenderer> ().enabled = false;
			Digits_07.GetComponent<MeshRenderer> ().enabled = false;
			yield return new WaitForSeconds(1.0f);
			Digits_01.GetComponent<MeshRenderer> ().enabled = false;
			Digits_02.GetComponent<MeshRenderer> ().enabled = false;
			Digits_03.GetComponent<MeshRenderer> ().enabled = false;
			Digits_04.GetComponent<MeshRenderer> ().enabled = true;
			Digits_05.GetComponent<MeshRenderer> ().enabled = false;
			Digits_06.GetComponent<MeshRenderer> ().enabled = false;
			Digits_07.GetComponent<MeshRenderer> ().enabled = false;
			yield return new WaitForSeconds(1.0f);
			Digits_01.GetComponent<MeshRenderer> ().enabled = false;
			Digits_02.GetComponent<MeshRenderer> ().enabled = false;
			Digits_03.GetComponent<MeshRenderer> ().enabled = false;
			Digits_04.GetComponent<MeshRenderer> ().enabled = false;
			Digits_05.GetComponent<MeshRenderer> ().enabled = true;
			Digits_06.GetComponent<MeshRenderer> ().enabled = false;
			Digits_07.GetComponent<MeshRenderer> ().enabled = false;
			yield return new WaitForSeconds(1.0f);
			Digits_01.GetComponent<MeshRenderer> ().enabled = false;
			Digits_02.GetComponent<MeshRenderer> ().enabled = false;
			Digits_03.GetComponent<MeshRenderer> ().enabled = false;
			Digits_04.GetComponent<MeshRenderer> ().enabled = false;
			Digits_05.GetComponent<MeshRenderer> ().enabled = false;
			Digits_06.GetComponent<MeshRenderer> ().enabled = true;
			Digits_07.GetComponent<MeshRenderer> ().enabled = false;
			yield return new WaitForSeconds(1.0f);
			Digits_01.GetComponent<MeshRenderer> ().enabled = false;
			Digits_02.GetComponent<MeshRenderer> ().enabled = false;
			Digits_03.GetComponent<MeshRenderer> ().enabled = false;
			Digits_04.GetComponent<MeshRenderer> ().enabled = false;
			Digits_05.GetComponent<MeshRenderer> ().enabled = false;
			Digits_06.GetComponent<MeshRenderer> ().enabled = false;
			Digits_07.GetComponent<MeshRenderer> ().enabled = true;
			yield return new WaitForSeconds(1.0f);
			yield return null;
	}
}
}
