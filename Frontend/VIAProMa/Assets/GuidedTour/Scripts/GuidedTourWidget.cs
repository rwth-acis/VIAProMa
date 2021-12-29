using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GuidedTour
{

    public class GuidedTourWidget : MonoBehaviour
    {
        protected struct PageText
        {
            public PageText(string headline, string hintText)
            {
                Headline = headline;
                HintText = hintText;
            }

            public string Headline { get; }
            public string HintText { get; }
        }


        private PageText[] pages =
        {
       new PageText( "Guided Tour - Page 1", "Welcome!\n\nPress the yellow round button on the box labeled 'Main Menu' to start..." ),
       new PageText( "Guided Tour - Page 2", "Page 2" ),
       new PageText( "Guided Tour - Page 3", "Page 3" )
    };

        private int currentPage = 0;
        public GameObject widget;
        public Text headline;
        public Text hintText;


        // Start is called before the first frame update
        void Start()
        {
            CurrentPage = 0;
        }



        void UpdateTask(AbstractTourTask task)
        {
            
            headline.text = task.Name;
            hintText.text = task.Description;
            

        }



        public int CurrentPage
        {
            get { return currentPage; }
            set
            {
                if (value >= 0 && value < pages.Length)
                {
                    headline.text = pages[value].Headline;
                    hintText.text = pages[value].HintText;
                    currentPage = value;
                }

            }
        }


        public bool WidgetVisible
        {
            get { return widget.activeSelf; }
            set { widget.SetActive(WidgetVisible); }
        }

    }
}
