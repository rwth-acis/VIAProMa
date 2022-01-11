using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;

namespace GuidedTour
{
    public class TextPlacer : MonoBehaviour
    {
        [SerializeField] private GuidedTourManager man;
        private bool metActiveTask = false;

        // Start is called before the first frame update
        void Start()
        {
            drawSectionBoard();
        }

        internal void drawSectionBoard()
        {
            bool metActiveSec = false;
            metActiveTask = false;
            StringBuilder strb = new StringBuilder();
            Debug.Log(man.Sections == null);
            foreach (TourSection sec in man.Sections)
            {
                bool activeSecIsCurrent = false;
                if (sec.Name == man.ActiveSection.Name)
                {
                    metActiveSec = true;
                    activeSecIsCurrent = true;
                }
                setSectionColor(strb, sec, metActiveSec, activeSecIsCurrent, false);
                strb.Append("<size=15>");
                strb.Append(sec.Name);
                strb.Append("</size=15>\n");
                setSectionColor(strb, sec, metActiveSec, activeSecIsCurrent, true);
                if (activeSecIsCurrent)
                {
                    foreach (AbstractTourTask task in sec.Tasks)
                    {
                        setTaskColor(strb, task, false);
                        strb.Append("<indent=10><size=10>");
                        strb.Append(task.Name);
                        if (man.ActiveTask.Id == task.Id)
                        {
                            strb.Append("<indent=15><size=8>\n");
                            strb.Append(task.Description);
                            strb.Append("</indent=15></size=8>");
                        }
                        strb.Append("</size=10></indent=10>\n");
                        setTaskColor(strb, task, false);
                    }
                }
            }
            transform.GetChild(0).GetComponent<TMP_Text>().text = strb.ToString();
        }

        private void setSectionColor(StringBuilder strb, TourSection section, bool metActiveSec, bool activeIsCurrent, bool isClosingTag)
        {
            if (!isClosingTag)
            {
                if (activeIsCurrent)
                {
                    strb.Append("<color=blue>");
                }
                else if (!metActiveSec)
                {
                    strb.Append("<color=green>");
                }
                else
                {
                    strb.Append("<color=white>");
                }
            }
            else
            {
                if (activeIsCurrent)
                {
                    strb.Append("</color=blue>");
                }
                else if (!metActiveSec)
                {
                    strb.Append("</color=green>");
                }
                else
                {
                    strb.Append("</color=white>");
                }
            }
        }

        private void setTaskColor(StringBuilder strb, AbstractTourTask task, bool isClosingTag)
        {
            bool activeIsCurrent = false;
            if (man.ActiveTask.Id == task.Id)
            {
                activeIsCurrent = true;
                metActiveTask = true;
            }
            if (!isClosingTag)
            {
                if (activeIsCurrent)
                {
                    strb.Append("<color=blue>");
                }
                else if (!metActiveTask)
                {
                    strb.Append("<color=green>");
                }
                else
                {
                    strb.Append("<color=white>");
                }
            }
            else
            {
                if (activeIsCurrent)
                {
                    strb.Append("</color=blue>");
                }
                else if (!metActiveTask)
                {
                    strb.Append("</color=green>");
                }
                else
                {
                    strb.Append("</color=white>");
                }
            }
        }
    }
}