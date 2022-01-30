using System.Collections;
using System.Collections.Generic;
using UnityEngine.Windows.Speech;
using System.Linq;
using UnityEngine;

public class AppearByVoiceCommand : MonoBehaviour
{
    KeywordRecognizer keywordRecognizer;
    public Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();

    [SerializeField] private string command;
    [SerializeField] private GameObject toAppear;
    [SerializeField] private bool flip = false;

    private void Start()
    {
        keywords.Add(command, () =>
        {
            Transform cam = Camera.main.transform;
            toAppear.transform.position = cam.position + cam.forward;
            toAppear.transform.LookAt(cam);
            if (flip) toAppear.transform.Rotate(Vector3.up, 180, Space.Self);
            toAppear.SetActive(true);
        });

        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray<string>(), ConfidenceLevel.Low);

        keywordRecognizer.OnPhraseRecognized += OnPhraseRecognized;

        keywordRecognizer.Start();
    }

    private void OnPhraseRecognized(PhraseRecognizedEventArgs e)
    {
        Debug.Log(e.text);
        if(keywords.TryGetValue(e.text, out System.Action action))
        {
            action.Invoke();
        }
    }


}
