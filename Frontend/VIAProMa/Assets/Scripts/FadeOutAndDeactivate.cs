using UnityEngine;

namespace i5.VIAProMa
{
    public class FadeOutAndDeactivate : MonoBehaviour
    {
        [SerializeField] private float fadeTime = 5f;

        public float FadeTime
        {
            get { return fadeTime; }
            set { fadeTime = value; }
        }

        private float time = 0f;
        Renderer[] rends;

        private void Awake()
        {
            rends = GetComponentsInChildren<Renderer>();
        }

        private void OnEnable()
        {
            time = 0f;
        }

        private void Update()
        {
            foreach (Renderer rend in rends)
            {
                rend.material.color =
                    new Color(
                        rend.material.color.r,
                        rend.material.color.g,
                        rend.material.color.b,
                        Mathf.Lerp(1, 0, time / fadeTime)
                        );
            }


            time += Time.deltaTime;
            if (time > fadeTime)
            {
                gameObject.SetActive(false);
            }
        }
    }
}