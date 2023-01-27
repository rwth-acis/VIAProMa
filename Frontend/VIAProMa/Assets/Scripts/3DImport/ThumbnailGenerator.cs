using System.Collections;
using System.IO;
using Microsoft.MixedReality.Toolkit;
using Siccity.GLTFUtility;
using UnityEngine;
using System.Threading.Tasks;
using TMPro;

public class ThumbnailGenerator : MonoBehaviour
{
    [SerializeField] private GameObject thumbSetup;
    private int heightOffset;

    private void Start()
    {
        heightOffset = -100;
    }

    /// <summary>
    /// Sets thumbnail given a local .glb path.
    /// </summary>
    public void SetThumbnail(string glbPath, Renderer renderer)
    {
        string filename = Path.GetFileNameWithoutExtension(glbPath);
        string pathToPNG = Path.Combine(Path.GetDirectoryName(glbPath), filename + ".png");


        // If image does not exists, generate thumbnail
        if (!File.Exists(pathToPNG))
        {
            heightOffset += 100;
            GameObject spawnedThumbSetup = Instantiate(thumbSetup);
            spawnedThumbSetup.transform.position = Vector3.zero + new Vector3(0, heightOffset, 0);
            GameObject model = Importer.LoadFromFile(glbPath);
            model.SetLayerRecursively(LayerMask.NameToLayer("Thumbnail"));
            Camera mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
            mainCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("Thumbnail"));
            model.transform.position = Vector3.zero;
            model.transform.rotation = Quaternion.identity;
            model.transform.localScale = Vector3.one;

            //resize object according to mesh bounds
            Renderer[] rr = model.GetComponentsInChildren<Renderer>();
            Bounds bounds = rr[0].bounds;
            foreach (Renderer r in rr) { bounds.Encapsulate(r.bounds); }

            model.transform.SetParent(spawnedThumbSetup.transform.GetChild(0));
            model.transform.localPosition = Vector3.zero;
            model.transform.localRotation = Quaternion.identity;
            model.transform.localScale = Vector3.one;

            model.transform.eulerAngles += new Vector3(-90, -180 + 45, 0);

            // Add BoxCollider
            model.AddComponent<BoxCollider>();
            model.GetComponent<BoxCollider>().size = bounds.size;
            model.GetComponent<BoxCollider>().center = bounds.center;

            model.transform.localScale = model.transform.localScale / (bounds.size.magnitude * 3.5f);

            StartCoroutine(GenerateThumbnail(pathToPNG, renderer, spawnedThumbSetup));
        }
        else
        {
            byte[] bytes = File.ReadAllBytes(pathToPNG);
            Texture2D thumbImg = new Texture2D(256, 256, TextureFormat.RGBA32, false);
            thumbImg.LoadImage(bytes);
            renderer.material.mainTexture = thumbImg;
        }

        
    }

    //Generates thumbnail
    private IEnumerator GenerateThumbnail(string pathToPNG, Renderer renderer, GameObject spawnedThumbSetup)
    {
        yield return new WaitForEndOfFrame(); // wait for rendering

        Camera thumbCam = spawnedThumbSetup.GetComponentInChildren<Camera>();
        thumbCam.cullingMask |= 1 << LayerMask.NameToLayer("Thumbnail");

        // make camera look at the center of the boxcollider (scaled to global position)
        BoxCollider col = spawnedThumbSetup.GetComponentInChildren<BoxCollider>();

        //use the center of the bounding box to set the camera
        thumbCam.transform.position = thumbCam.transform.position + (col.transform.position - col.transform.TransformPoint(col.center));
        
        thumbCam.transform.LookAt(col.transform.TransformPoint(col.center));


        RenderTexture.active = thumbCam.targetTexture;

        thumbCam.Render();

        Texture2D thumbImage = new Texture2D(thumbCam.targetTexture.width, thumbCam.targetTexture.height, TextureFormat.RGBA32, false);
        thumbImage.ReadPixels(new Rect(0, 0, thumbCam.targetTexture.width, thumbCam.targetTexture.height), 0, 0);
        thumbImage.Apply();

        heightOffset -= 100;

        byte[] bytes = thumbImage.EncodeToPNG();

        File.WriteAllBytes(pathToPNG, bytes);

        Destroy(spawnedThumbSetup);

        bytes = File.ReadAllBytes(pathToPNG);
        Texture2D thumbImg = new Texture2D(256, 256, TextureFormat.RGBA32, false);
        thumbImg.LoadImage(bytes);
        thumbImg.Apply();
        renderer.material.SetTexture("_MainTex", thumbImg);
        
    }
}
