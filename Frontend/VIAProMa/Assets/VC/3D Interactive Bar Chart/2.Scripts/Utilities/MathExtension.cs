using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Uitility.VittorCloud
{
    public static class MathExtension
    {

        public static int Round(this int i, int range)
        {

            double roundNum = 0;

            double nearest = 0;


            nearest = Mathf.Pow(10, GetNumOfDigits(i) - 1);

            if (i < 0)
                nearest *= -1;

            //    Debug.Log("nearest of : " + i + "  is : " + nearest);
            if (i > nearest * 5 && range > 5 * nearest)
            {

                nearest = nearest * 10;
            }


            roundNum = Math.Round((double)i / nearest) * nearest;
            //   Debug.Log("roundNum of : " + i + "  is : " + roundNum);



            if (roundNum == i)
            {
                if (roundNum < 5 * nearest)
                {

                    roundNum = 0;
                }
                else
                {

                    roundNum = 5 * nearest;

                }

            }


            //Debug.Log(" Final roundNum of : " + i + "  is : " + roundNum);


            return (int)roundNum;
        }

        public static int GetNumOfDigits(this Int32 n) =>
            n == 0 ? 0 : 1 + (int)Math.Log10(Math.Abs(n));

        public static float GetMaxValue(List<float> list)
        {

            float max = 0;


            for (int i = 0; i < list.Count; i++)
            {
                if (i == 0)
                {
                    max = list[i];
                }
                if (list[i] > max)
                {
                    max = list[i];
                }
            }

            //  Debug.Log("max=" + max);
            return max;

        }

        public static float GetMinValue(List<float> list)
        {

            float min = list[0];


            for (int i = 0; i < list.Count; i++)
            {

                if (list[i] < min)
                {
                    min = list[i];
                }
            }

            //    Debug.Log("Min=" + min);
            return min;

        }
    }
}
