﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    [System.Serializable]
    public class ProCamera2DParallaxLayer
    {
        #if PC2D_TK2D_SUPPORT
        public tk2dCamera ParallaxCameraTk2d;
        #endif

        public Camera ParallaxCamera;

        [RangeAttribute(0, 5)]
        public float Speed = 1.0f;

        public LayerMask LayerMask;

        [HideInInspector][System.NonSerialized]
        public Transform CameraTransform;
    }

    [ExecuteInEditMode]
    public class ProCamera2DParallax : BasePC2D
    {
        public List<ProCamera2DParallaxLayer> ParallaxLayers = new List<ProCamera2DParallaxLayer>();
        public bool ParallaxHorizontal = true;
        public bool ParallaxVertical = true;
        public Vector3 RootPosition = Vector3.zero;

        float _initialOrtographicSize;

        float[] _initialSpeeds;
        Coroutine _animateCoroutine;

        override protected void Start()
        {
            base.Start();

            if (ProCamera2D == null)
                return;

            if (Application.isPlaying)
            {
                // Find all parallax objects
                var parallaxObjs = FindObjectsOfType<ProCamera2DParallaxObject>();

                // Create dictionary that links Unity layers to ProCamera2D parallax layers
                var layersDic = new Dictionary<int, ProCamera2DParallaxLayer>();
                for (int i = 0; i <= 31; i++)
                {
                    foreach (var parallaxLayer in ParallaxLayers)
                    {
                        if (parallaxLayer.LayerMask == (parallaxLayer.LayerMask | (1 << i)))
                        {
                            layersDic[i] = parallaxLayer;
                        }
                    }
                }

                // Apply offset to parallax objects according to the parallax layer they're part of
                for (int i = 0; i < parallaxObjs.Length; i++)
                {
                    // Position
                    Vector3 parallaxObjPosition = parallaxObjs[i].transform.position - RootPosition;
                    float x = Vector3H(parallaxObjPosition) * layersDic[parallaxObjs[i].gameObject.layer].Speed;
                    float y = Vector3V(parallaxObjPosition) * layersDic[parallaxObjs[i].gameObject.layer].Speed;
                    parallaxObjs[i].transform.position = VectorHVD(x, y, Vector3D(parallaxObjPosition)) + RootPosition;
                }
            }

            foreach (var layer in ParallaxLayers)
            {
                if (layer.ParallaxCamera != null)
                {
                    layer.CameraTransform = layer.ParallaxCamera.transform;
                }
            }

            // We need this so we can toggle on/off the parallax
            _initialSpeeds = new float[ParallaxLayers.Count];
            for (int i = 0; i < _initialSpeeds.Length; i++)
            {
                _initialSpeeds[i] = ParallaxLayers[i].Speed;
            }

            if (ProCamera2D.GameCamera != null)
            {
                _initialOrtographicSize = ProCamera2D.GameCamera.orthographicSize;

                if (!ProCamera2D.GameCamera.orthographic)
                    enabled = false;
            }

            #if PC2D_TK2D_SUPPORT
            if (ProCamera2D.Tk2dCam != null)
            {
                _initialOrtographicSize = ProCamera2D.Tk2dCam.CameraSettings.orthographicSize / ProCamera2D.Tk2dCam.ZoomFactor;

                if (ProCamera2D.Tk2dCam.CameraSettings.projection != tk2dCameraSettings.ProjectionType.Orthographic)
                    enabled = false;
            }
            #endif
        }
        
        override protected void OnPostMoveUpdate(float deltaTime)
        {
            Move();
        }

        #if UNITY_EDITOR
        void LateUpdate()
        {
            if(!Application.isPlaying)
                Move();
        }
        #endif

        void Move()
        {
            Vector3 rootOffset = transform.position - RootPosition;

            for (int i = 0; i < ParallaxLayers.Count; i++)
            {
                if (ParallaxLayers[i].CameraTransform != null)
                {
                    // Position
                    float x = ParallaxHorizontal ? Vector3H(rootOffset) * ParallaxLayers[i].Speed : Vector3H(rootOffset);
                    float y = ParallaxVertical ? Vector3V(rootOffset) * ParallaxLayers[i].Speed : Vector3V(rootOffset);
                    ParallaxLayers[i].CameraTransform.position = RootPosition + VectorHVD(x, y, Vector3D(transform.position));

                    // Zoom
                    ParallaxLayers[i].ParallaxCamera.orthographicSize = _initialOrtographicSize + (ProCamera2D.GameCamera.orthographicSize - _initialOrtographicSize) * ParallaxLayers[i].Speed;
                    #if PC2D_TK2D_SUPPORT
                    if (ParallaxLayers[i].ParallaxCameraTk2d != null)
                        ParallaxLayers[i].ParallaxCameraTk2d.ZoomFactor = ProCamera2D.Tk2dCam.ZoomFactor;
                    #endif
                }
            }
        }

        public void ToggleParallax(bool value, float duration = 2f, EaseType easeType = EaseType.EaseInOut)
        {
            if (_initialSpeeds == null)
                return;

            if (_animateCoroutine != null)
                StopCoroutine(_animateCoroutine);

            _animateCoroutine = StartCoroutine(Animate(value, duration, easeType));
        }

        IEnumerator Animate(bool value, float duration, EaseType easeType)
        {
            var currentSpeeds = new float[ParallaxLayers.Count];
            for (int i = 0; i < currentSpeeds.Length; i++)
            {
                currentSpeeds[i] = ParallaxLayers[i].Speed;
            }

            var t = 0f;
            while (t <= 1.0f)
            {
                t += ProCamera2D.DeltaTime / duration;

                for (int i = 0; i < ParallaxLayers.Count; i++)
                {
                    if (value)
                        ParallaxLayers[i].Speed = Utils.EaseFromTo(currentSpeeds[i], _initialSpeeds[i], t, easeType);
                    else
                        ParallaxLayers[i].Speed = Utils.EaseFromTo(currentSpeeds[i], 1, t, easeType);
                }

                yield return ProCamera2D.GetYield();
            }
        }
    }
}