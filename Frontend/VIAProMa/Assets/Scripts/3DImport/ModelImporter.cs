#define MRTK_GLTF_IMPORTER_OFF

using UnityEngine;
using HoloToolkit.Unity;
using Siccity.GLTFUtility;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;

public class ModelImporter: Singleton<ModelImporter> {

    [SerializeField] private Shader GLTFshaderMetallic;
    [SerializeField] private Shader GLTFshaderMetallicTransparent;
    [SerializeField] private Shader GLTFshaderSpecular;
    [SerializeField] private Shader GLTFshaderSpecularTransparent;

    [SerializeField] private Shader shaderMetallic;
    [SerializeField] private Shader shaderMetallicTransparent;
    [SerializeField] private Shader shaderSpecular;
    [SerializeField] private Shader shaderSpecularTransparent;

    public GameObject InstantiateModel(string path)
    {
        //import into unity scene
        AnimationClip[] animClips;
        GameObject model = Importer.LoadFromFile(path, new ImportSettings(), out animClips);
        model.transform.position = Vector3.zero;
        model.transform.rotation = Quaternion.identity;
        model.transform.localScale = Vector3.one;

        //resize object according to mesh bounds
        Renderer[] rr = model.GetComponentsInChildren<Renderer>();
        Bounds bounds = rr[0].bounds;
        foreach (Renderer r in rr) { bounds.Encapsulate(r.bounds); }

        //MeshFilter[] ff = model.GetComponentsInChildren<MeshFilter>();
        //foreach (MeshFilter f in ff) { bounds.Encapsulate(f.mesh.bounds); }


        //add interactables and collider
        model.AddComponent<BoxCollider>();
        model.GetComponent<BoxCollider>().size = bounds.size;
        model.GetComponent<BoxCollider>().center = bounds.center;


        // Taking only the first clip for now. Should be pretty easy to extend it  to generalize
        if (animClips.Length > 0)
        {
            Animation anim = model.AddComponent<Animation>();
            animClips[0].legacy = true;
            animClips[0].wrapMode = WrapMode.Loop;
            anim.AddClip(animClips[0], animClips[0].name);
            anim.Play(animClips[0].name);
        }

        //changing the shader
        foreach (Renderer r in rr)
        {
            if (r.material.shader == GLTFshaderMetallic)
            {
                r.material.shader = shaderMetallic;
            }
            else if (r.material.shader == GLTFshaderMetallicTransparent)
            {
                r.material.shader = shaderMetallicTransparent;
            }
            else if (r.material.shader == GLTFshaderSpecular)
            {
                r.material.shader = shaderSpecular;
            }
            else if (r.material.shader == GLTFshaderSpecularTransparent)
            {
                r.material.shader = shaderSpecularTransparent;
            }
        }

        model.transform.localScale = model.transform.localScale / (bounds.size.magnitude * 4f);


        model.transform.rotation = this.gameObject.transform.rotation;
        model.transform.eulerAngles += new Vector3(-90, -180, 0);
        //model.transform.position = this.gameObject.transform.position;

        //use the center of the bounding box to set the object
        model.transform.position = model.transform.position + (this.gameObject.transform.position - model.transform.TransformPoint(model.GetComponent<BoxCollider>().center));
        model.transform.position = model.transform.position - this.gameObject.transform.forward * 0.1f;

        model.AddComponent<NearInteractionGrabbable>();
        model.AddComponent<ObjectManipulator>();
        model.GetComponent<ObjectManipulator>().HostTransform = model.transform;

        model.name = System.IO.Path.GetFileNameWithoutExtension(path);

        

        return model;
    }
}