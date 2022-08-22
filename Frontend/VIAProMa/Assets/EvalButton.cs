using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VIAProMa.LiteratureSearch
{
    public class EvalButton : MonoBehaviour
    {
        // Start is called before the first frame update
        public async void OnClick()
        {

            string[] dois = new string[] {
                "10.1007/978-3-030-87595-4_19",
                "10.1109/MCG.2019.2897927",
                "10.1145/2984751.2984776",
                "10.1002/asi.21419",
                "10.1109/BDVA.2015.7314296",
                "10.1145/3131085.3131097",
                "10.1007/s41465-019-00141-8",
                "10.1002/asi.4630240406",
                "10.1177/0165551520962775",
                "10.1002/asi.5090140103"
            };

            List<Paper> list = new List<Paper>();

            foreach(string doi in dois)
            {
                list.Add(await Communicator.GetPaper(doi)); 
            }

            PaperController.Instance.ShowResults(list, this.transform);

            Destroy(this.gameObject);
        }
    }

}
