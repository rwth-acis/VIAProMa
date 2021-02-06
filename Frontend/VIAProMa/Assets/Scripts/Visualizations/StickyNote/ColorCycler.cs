using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorCycler : MonoBehaviour
{
    [SerializeField] private Renderer ColorTag;
    [SerializeField] private Renderer Dashline;
	
	private Color[] cols = {Color.red, Color.yellow, Color.green, Color.cyan, Color.blue, Color.magenta, Color.black, Color.grey, Color.white};
	private short index = 0;
	
	public void cycle(){
		Color newCol = cols[(index++)%cols.Length];
		ColorTag.material.color = newCol;
		Dashline.material.color = newCol;
	}

	public void colorSet(Color a){
		ColorTag.material.color = a;
		Dashline.material.color = a;
	}
}
