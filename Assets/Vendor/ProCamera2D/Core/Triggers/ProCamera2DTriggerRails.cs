using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    public enum TriggerRailsMode
    {
        Disable,
        Enable
    }
    
    public class ProCamera2DTriggerRails : BaseTrigger
    {
        [HideInInspector]
        public ProCamera2DRails ProCamera2DRails;

        [Tooltip("Choose if you want this trigger to enable or disable the rails.")]
        public TriggerRailsMode Mode; 

        [Tooltip("The time it should take to transition from/to rails.")]
        public float TransitionDuration = 1f;
        
        override protected void Start()
        {
            base.Start();

            if (ProCamera2D == null)
                return;
                
            if (ProCamera2DRails == null)
                ProCamera2DRails = FindObjectOfType<ProCamera2DRails>();

            if (ProCamera2DRails == null)
                Debug.LogWarning("Rails extension couldn't be found on ProCamera2D. Please enable it to use this trigger.");
        }

        protected override void EnteredTrigger()
        {
            base.EnteredTrigger();
            
            if(Mode == TriggerRailsMode.Enable)
                ProCamera2DRails.EnableTargets(TransitionDuration);
            else
                ProCamera2DRails.DisableTargets(TransitionDuration);
        }

        #if UNITY_EDITOR
        int _drawGizmosCounter;

        override protected void OnDrawGizmos()
        {
            // HACK to prevent Unity bug on startup: http://forum.unity3d.com/threads/screen-position-out-of-view-frustum.9918/
            _drawGizmosCounter++;
            if (_drawGizmosCounter < 5 && UnityEditor.EditorApplication.timeSinceStartup < 60f)
                return;

            base.OnDrawGizmos();
            
            if (_gizmosDrawingFailed)
                return;

            float cameraDepthOffset = Vector3D(ProCamera2D.transform.localPosition) + Mathf.Abs(Vector3D(ProCamera2D.transform.localPosition)) * Vector3D(ProCamera2D.transform.forward);

            Gizmos.DrawIcon(
                VectorHVD(
                    Vector3H(transform.position), 
                    Vector3V(transform.position), 
                    cameraDepthOffset), 
                Mode == TriggerRailsMode.Enable ? "ProCamera2D/gizmo_icon_rails_enable.png" : "ProCamera2D/gizmo_icon_rails_disable.png", 
                false);
        }
        #endif
    }
}