using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using i5.VIAProMa.Utilities;
using i5.VIAProMa.Visualizations.Minimap;
using Microsoft.MixedReality.Toolkit.UI;

namespace i5.VIAProMa.Visualizations.Minimap
{
    public class MinimapHeaderController : MonoBehaviour
    {
        public Text title;
        private TextMeshPro pro;
        private Transform cube;

        public Dropdown dropdown;
        public Image show_color;
        public Image show_colorBox;
        public InputField input_Text;
        public Slider input_ColorR;
        public Slider input_ColorG;
        public Slider input_ColorB;

        public Slider input_ColorBoxR;
        public Slider input_ColorBoxG;
        public Slider input_ColorBoxB;

        public InputField input_Size;
        public TMP_FontAsset[] fonts;
        public string[] fontNames;

        void Start()
        {
            title.text = "[None]";
            title.color = Color.red;
            dropdown.options.Clear();
            for (int i = 0; i < fontNames.Length; i++)
            {
                dropdown.options.Add(new Dropdown.OptionData(fontNames[i]));
            }
            dropdown.transform.GetChild(0).GetComponent<Text>().text = fontNames[0];

        }


        private void Update()
        {
            show_color.color = new Color(input_ColorR.value, input_ColorG.value, input_ColorB.value);
            show_colorBox.color = new Color(input_ColorBoxR.value, input_ColorBoxG.value, input_ColorBoxB.value);

            if (Input.GetMouseButtonDown(0) && !transform.GetChild(0).gameObject.activeSelf)
            {
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2f, Screen.height / 2f)), out hit, 10f))
                {
                    Debug.Log("Hit:" + hit.transform.name);
                    if (hit.transform.TryGetComponent<MinimapHeaderText>(out MinimapHeaderText t))
                    {
                        pro = t.target;
                        cube = t.bg;
                        title.text = "[" + pro.name + "]";
                        title.color = Color.green;
                        transform.GetChild(0).gameObject.SetActive(true);
                        input_ColorR.value = pro.color.r;
                        input_ColorG.value = pro.color.g;
                        input_ColorB.value = pro.color.b;
                        input_Size.text = pro.fontSize.ToString();
                        input_Text.text = pro.text;

                        input_ColorBoxR.value = cube.GetComponent<MeshRenderer>().material.color.r;
                        input_ColorBoxG.value = cube.GetComponent<MeshRenderer>().material.color.g;
                        input_ColorBoxB.value = cube.GetComponent<MeshRenderer>().material.color.b;


                        Debug.Log("Set Header!");
                    }
                }
            }
        }

        public void Apply()
        {
            if (pro == null)
            {
                return;
            }
            pro.text = input_Text.text;
            pro.color = show_color.color;
            pro.fontSize = float.Parse(input_Size.text);
            pro.font = fonts[dropdown.value];

            cube.GetComponent<MeshRenderer>().material.SetColor("_Color", show_colorBox.color);

            //Debug.Log("应用");
        }

        public void Close()
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}