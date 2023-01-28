using i5.VIAProMa.ResourceManagagement;
using i5.VIAProMa.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

namespace i5.VIAProMa.Shelves.Visualizations
{
    public class VisualizationInstantiator : MonoBehaviour, IMixedRealityPointerHandler
    {
        [SerializeField] private GameObject visualizationPrefab;

        private BoundingBoxStateController boxStateController;
        private ObjectManipulator handler;
        private GameObject initiatedGameObject;

        private GameObject UndoRedoManagerGameObject;
        private UndoRedoManager UndoRedoManager;

        private void Awake()
        {
            if (visualizationPrefab == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(visualizationPrefab));
            }
            UndoRedoManagerGameObject = GameObject.Find("UndoRedo Manager");
            UndoRedoManager = UndoRedoManagerGameObject.GetComponent<UndoRedoManager>();
        }

        public void OnPointerClicked(MixedRealityPointerEventData eventData)
        {
        }

        public void OnPointerDown(MixedRealityPointerEventData eventData)
        {

           ResourceManager.Instance.SceneNetworkInstantiate(visualizationPrefab, transform.position, transform.rotation, (instance) =>
            {
                boxStateController = instance.GetComponentInChildren<BoundingBoxStateController>();
                if (boxStateController == null)
                {
                    SpecialDebugMessages.LogComponentNotFoundError(this, nameof(BoundingBoxStateController), instance);
                }
                boxStateController.BoundingBoxActive = true;
                handler = instance.GetComponentInChildren<ObjectManipulator>();
                handler.OnPointerDown(eventData);
                initiatedGameObject = instance;
            });

            ICommand initiate = new InitiateObjectCommand(initiatedGameObject);
            UndoRedoManager.Execute(initiate);

        }

        public void OnPointerDragged(MixedRealityPointerEventData eventData)
        {
            handler.OnPointerDragged(eventData);
        }

        public void OnPointerUp(MixedRealityPointerEventData eventData)
        {
            handler.OnPointerUp(eventData);
            boxStateController.BoundingBoxActive = false;
        }
    }
}