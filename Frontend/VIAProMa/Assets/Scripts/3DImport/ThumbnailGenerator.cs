using System.Collections;
using System.IO;
using Microsoft.MixedReality.Toolkit;
using Siccity.GLTFUtility;
using UnityEngine;

public class ThumbnailGenerator : MonoBehaviour
{
    [SerializeField] private Camera thumbCam;
    [SerializeField] private Transform spawnPos;


    public string GetThumbnailPath(string glbPath)
    {
        string filename = Path.GetFileNameWithoutExtension(glbPath);
        string pathToPNG = Path.Combine(Path.GetDirectoryName(glbPath), filename + ".png");

        // If file does not exists, generate thumbnail
        if (!File.Exists(pathToPNG))
        {
            GameObject model = Importer.LoadFromFile(glbPath);
            model.SetLayerRecursively(LayerMask.NameToLayer("Thumbnail"));
            model.transform.SetParent(spawnPos);
            model.transform.position = spawnPos.position;
            model.transform.rotation = spawnPos.rotation;
            model.transform.eulerAngles += new Vector3(-90, -180, 0);

            //resize object according to mesh bounds
            MeshFilter[] rr = model.GetComponentsInChildren<MeshFilter>();
            Bounds bounds = rr[0].mesh.bounds;
            foreach (MeshFilter r in rr) { bounds.Encapsulate(r.mesh.bounds); }

            model.transform.localScale = model.transform.localScale / (bounds.size.magnitude * 3.5f);

            StartCoroutine(GenerateThumbnail(pathToPNG, model));
        }

        return pathToPNG;
    }

    public void SetThumbnail(string pngPath, MeshRenderer renderer)
    {
        byte[] bytes = File.ReadAllBytes(pngPath);
        Texture2D thumbImg = new Texture2D(256, 256, TextureFormat.RGBA32, false);
        thumbImg.LoadImage(bytes);
        renderer.material.mainTexture = thumbImg;
    }

    private IEnumerator GenerateThumbnail(string pathToPNG, GameObject model)
    {
        yield return new WaitForEndOfFrame(); // wait for rendering

        RenderTexture.active = thumbCam.targetTexture;

        thumbCam.Render();

        Texture2D thumbImage = new Texture2D(thumbCam.targetTexture.width, thumbCam.targetTexture.height, TextureFormat.RGBA32, false);
        thumbImage.ReadPixels(new Rect(0, 0, thumbCam.targetTexture.width, thumbCam.targetTexture.height), 0, 0);
        thumbImage.Apply();

        byte[] bytes = thumbImage.EncodeToPNG();

        File.WriteAllBytes(pathToPNG, bytes);

        Destroy(model);
    }
}
