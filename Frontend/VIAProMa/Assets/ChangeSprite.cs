using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSprite : MonoBehaviour
{
    [SerializeField] private GameObject icon;
    [SerializeField] public Sprite play;
    [SerializeField] public Sprite stop;

    // Start is called before the first frame update
    void Start()
    {
        icon.GetComponent<SpriteRenderer>().sprite = play;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleSprite()
    {
        if (TimerWindow.timerOn == true)
        {
            icon.GetComponent<SpriteRenderer>().sprite = stop;
        }
        else
        {
            icon.GetComponent<SpriteRenderer>().sprite = play;
        }
    }
}
