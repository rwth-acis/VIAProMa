using UnityEngine;

namespace i5.VIAProMa.Utilities
{
    public static class ConversionUtilities
    {
        public static Vector3 ColorToVector3(Color color)
        {
            return new Vector3(color.r, color.g, color.b);
        }

        public static Vector4 ColorToVector4(Color color)
        {
            return new Vector4(color.r, color.g, color.b, color.a);
        }

        public static Color Vector3ToColor(Vector3 vector)
        {
            return new Color(vector.x, vector.y, vector.z);
        }

        public static Color Vector4ToColor(Vector4 vector)
        {
            return new Color(vector.x, vector.y, vector.z, vector.w);
        }
    }
}