using DIG.GBLXAPI;
using UnityEngine;
using UnityEngine.UI;

public class TimerExample : MonoBehaviour
{
    public float ElapsedTime { get; private set; }

    [SerializeField] private Text _timerText = null;
    [SerializeField] private Button _timerButton = null;

    private Text _timerButtonText;

    private bool _running = false;

    private void Awake()
    {
        _timerButtonText = _timerButton.GetComponentInChildren<Text>();
    }

    private void Start()
    {
        ElapsedTime = 0f;
        GBLXAPI.Timers.ResetSlot(1);
    }

    private void Update()
    {
        if(!_running) { return; }

        ElapsedTime += Time.deltaTime;

        _timerText.text = ElapsedTime.ToString("0.00");
    }

    public void OnTimerButtonPressed()
    {
        _running = !_running;

        if(_running)
        {
            ElapsedTime = 0f;
            GBLXAPI.Timers.ResetSlot(1);

            _timerButtonText.text = "Stop";

            GBL_Interface.SendTimerStarted();
        }
        else
        {
            _timerButtonText.text = "Start";

            GBL_Interface.SendTimerStopped();
        }
    }
}
