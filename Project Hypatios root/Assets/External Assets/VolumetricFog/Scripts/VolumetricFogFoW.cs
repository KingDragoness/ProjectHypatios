//------------------------------------------------------------------------------------------------------------------
// Volumetric Fog & Mist
// Created by Ramiro Oliva (Kronnect)
//------------------------------------------------------------------------------------------------------------------
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace VolumetricFogAndMist {


    public enum MASK_TEXTURE_BRUSH_MODE {
        AddFog = 0,
        RemoveFog = 1
    }


    public partial class VolumetricFog : MonoBehaviour {

        const int MAX_SIMULTANEOUS_TRANSITIONS = 10000;

        #region Fog of War settings

        [SerializeField]
        bool _fogOfWarEnabled;

        public bool fogOfWarEnabled {
            get { return _fogOfWarEnabled; }
            set {
                if (value != _fogOfWarEnabled) {
                    _fogOfWarEnabled = value;
                    FogOfWarInit();
                    UpdateMaterialProperties();
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        Vector3 _fogOfWarCenter;

        public Vector3 fogOfWarCenter {
            get { return _fogOfWarCenter; }
            set {
                if (value != _fogOfWarCenter) {
                    _fogOfWarCenter = value;
                    UpdateMaterialProperties();
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        Vector3 _fogOfWarSize = new Vector3(1024, 0, 1024);

        public Vector3 fogOfWarSize {
            get { return _fogOfWarSize; }
            set {
                if (value != _fogOfWarSize) {
                    if (value.x > 0 && value.z > 0) {
                        _fogOfWarSize = value;
                        UpdateMaterialProperties();
                        isDirty = true;
                    }
                }
            }
        }

        [SerializeField, Range(32, 2048)]
        int _fogOfWarTextureSize = 256;

        public int fogOfWarTextureSize {
            get { return _fogOfWarTextureSize; }
            set {
                if (value != _fogOfWarTextureSize) {
                    if (value > 16) {
                        _fogOfWarTextureSize = value;
                        FogOfWarUpdateTexture();
                        UpdateMaterialProperties();
                        isDirty = true;
                    }
                }
            }
        }

        [SerializeField, Range(0, 100)]
        float _fogOfWarRestoreDelay = 0;

        public float fogOfWarRestoreDelay {
            get { return _fogOfWarRestoreDelay; }
            set {
                if (value != _fogOfWarRestoreDelay) {
                    _fogOfWarRestoreDelay = value;
                    isDirty = true;
                }
            }
        }


        [SerializeField, Range(0, 25)]
        float _fogOfWarRestoreDuration = 2f;

        public float fogOfWarRestoreDuration {
            get { return _fogOfWarRestoreDuration; }
            set {
                if (value != _fogOfWarRestoreDuration) {
                    _fogOfWarRestoreDuration = value;
                    isDirty = true;
                }
            }
        }

        [SerializeField, Range(0, 1)]
        float _fogOfWarSmoothness = 1f;

        public float fogOfWarSmoothness {
            get { return _fogOfWarSmoothness; }
            set {
                if (value != _fogOfWarSmoothness) {
                    _fogOfWarSmoothness = value;
                    isDirty = true;
                }
            }
        }

        #endregion


        #region In-Editor fog of war painter

        [SerializeField]
        bool _maskEditorEnabled;

        public bool maskEditorEnabled {
            get { return _maskEditorEnabled; }
            set {
                if (value != _maskEditorEnabled) {
                    _maskEditorEnabled = value;
                }
            }
        }

        [SerializeField]
        MASK_TEXTURE_BRUSH_MODE _maskBrushMode = MASK_TEXTURE_BRUSH_MODE.RemoveFog;

        public MASK_TEXTURE_BRUSH_MODE maskBrushMode {
            get { return _maskBrushMode; }
            set {
                if (value != _maskBrushMode) {
                    _maskBrushMode = value;
                }
            }
        }


        [SerializeField, Range(1, 128)]
        int _maskBrushWidth = 20;

        public int maskBrushWidth {
            get { return _maskBrushWidth; }
            set {
                if (value != _maskBrushWidth) {
                    _maskBrushWidth = value;
                }
            }
        }

        [SerializeField, Range(0, 1)]
        float _maskBrushFuzziness = 0.5f;

        public float maskBrushFuzziness {
            get { return _maskBrushFuzziness; }
            set {
                if (value != _maskBrushFuzziness) {
                    _maskBrushFuzziness = value;
                }
            }
        }

        [SerializeField, Range(0, 1)]
        float _maskBrushOpacity = 0.15f;

        public float maskBrushOpacity {
            get { return _maskBrushOpacity; }
            set {
                if (value != _maskBrushOpacity) {
                    _maskBrushOpacity = value;
                }
            }
        }

        #endregion

        [SerializeField]
        Texture2D _fogOfWarTexture;

        public Texture2D fogOfWarTexture {
            get { return _fogOfWarTexture; }
            set {
                if (_fogOfWarTexture != value) {
                    if (value != null) {
                        if (value.width != value.height) {
                            Debug.LogError("Fog of war texture must be square.");
                        } else {
                            _fogOfWarTexture = value;
                            ReloadFogOfWarTexture();
                        }
                    }
                }
            }
        }


        Color32[] fogOfWarColorBuffer;

        struct FogOfWarTransition {
            public bool enabled;
            public int x, y;
            public float startTime, startDelay;
            public float duration;
            public byte initialAlpha;
            public byte targetAlpha;
        }

        FogOfWarTransition[] fowTransitionList;
        int lastTransitionPos;
        Dictionary<int, int> fowTransitionIndices;
        bool requiresTextureUpload;

        #region Fog Of War

        void FogOfWarInit() {
            if (fowTransitionList == null || fowTransitionList.Length != MAX_SIMULTANEOUS_TRANSITIONS) {
                fowTransitionList = new FogOfWarTransition[MAX_SIMULTANEOUS_TRANSITIONS];
            }
            if (fowTransitionIndices == null) {
                fowTransitionIndices = new Dictionary<int, int>(MAX_SIMULTANEOUS_TRANSITIONS);
            } else {
                fowTransitionIndices.Clear();
            }
            lastTransitionPos = -1;
            if (_fogOfWarTexture == null) {
                FogOfWarUpdateTexture();
            } else if (_fogOfWarEnabled && (fogOfWarColorBuffer == null || fogOfWarColorBuffer.Length == 0)) {
                ReloadFogOfWarTexture();
            }
        }

        /// <summary>
        /// Reloads the current contents of the fog of war texture
        /// </summary>
        public void ReloadFogOfWarTexture() {
            if (_fogOfWarTexture == null) return;
            _fogOfWarTextureSize = _fogOfWarTexture.width;
            fogOfWarColorBuffer = _fogOfWarTexture.GetPixels32();
            lastTransitionPos = -1;
            fowTransitionIndices.Clear();
            isDirty = true;
            fogOfWarEnabled = true;
        }


        void FogOfWarUpdateTexture() {
            if (!_fogOfWarEnabled || !Application.isPlaying)
                return;
            int size = GetScaledSize(_fogOfWarTextureSize, 1.0f);
            if (_fogOfWarTexture == null || _fogOfWarTexture.width != size || _fogOfWarTexture.height != size) {
                _fogOfWarTexture = new Texture2D(size, size, TextureFormat.Alpha8, false);
                _fogOfWarTexture.hideFlags = HideFlags.DontSave;
                _fogOfWarTexture.filterMode = FilterMode.Bilinear;
                _fogOfWarTexture.wrapMode = TextureWrapMode.Clamp;
                ResetFogOfWar();
            }
        }


        /// <summary>
        /// Updates fog of war transitions and uploads texture changes to GPU if required
        /// </summary>
        public void UpdateFogOfWar(bool forceUpload = false) {
            if (!_fogOfWarEnabled || _fogOfWarTexture == null)
                return;

            if (forceUpload) {
                requiresTextureUpload = true;
            }

            int tw = _fogOfWarTexture.width;
            for (int k = 0; k <= lastTransitionPos; k++) {
                FogOfWarTransition fw = fowTransitionList[k];
                if (!fw.enabled)
                    continue;
                float elapsed = Time.time - fw.startTime - fw.startDelay;
                if (elapsed > 0) {
                    float t = fw.duration <= 0 ? 1 : elapsed / fw.duration;
                    if (t < 0) t = 0; else if (t > 1f) t = 1f;
                    byte alpha = (byte)(fw.initialAlpha + (fw.targetAlpha - fw.initialAlpha) * t); // Mathf.Lerp(fw.initialAlpha, fw.targetAlpha, t);
                    int colorPos = fw.y * tw + fw.x;
                    fogOfWarColorBuffer[colorPos].a = alpha;
                    //fogOfWarTexture.SetPixel(fw.x, fw.y, fogOfWarColorBuffer[colorPos]);
                    requiresTextureUpload = true;
                    if (t >= 1f) {
                        fowTransitionList[k].enabled = false;
                        // Add refill slot if needed
                        if (fw.targetAlpha < 255 && _fogOfWarRestoreDelay > 0) {
                            AddFogOfWarTransitionSlot(fw.x, fw.y, fw.targetAlpha, 255, _fogOfWarRestoreDelay, _fogOfWarRestoreDuration);
                        }
                    }
                }
            }
            if (requiresTextureUpload) {
                requiresTextureUpload = false;
                _fogOfWarTexture.SetPixels32(fogOfWarColorBuffer);
                _fogOfWarTexture.Apply();

#if UNITY_EDITOR
                if (!Application.isPlaying) {
                    UnityEditor.EditorUtility.SetDirty(_fogOfWarTexture);
                }
#endif


            }
        }

        /// <summary>
        /// Instantly changes the alpha value of the fog of war at world position. It takes into account FogOfWarCenter and FogOfWarSize.
        /// Note that only x and z coordinates are used. Y (vertical) coordinate is ignored.
        /// </summary>
        /// <param name="worldPosition">in world space coordinates.</param>
        /// <param name="radius">radius of application in world units.</param>
        public void SetFogOfWarAlpha(Vector3 worldPosition, float radius, float fogNewAlpha) {
            SetFogOfWarAlpha(worldPosition, radius, fogNewAlpha, 1f);
        }


        /// <summary>
        /// Changes the alpha value of the fog of war at world position creating a transition from current alpha value to specified target alpha. It takes into account FogOfWarCenter and FogOfWarSize.
        /// Note that only x and z coordinates are used. Y (vertical) coordinate is ignored.
        /// </summary>
        /// <param name="worldPosition">in world space coordinates.</param>
        /// <param name="radius">radius of application in world units.</param>
        /// <param name="fogNewAlpha">target alpha value.</param>
        /// <param name="duration">duration of transition in seconds (0 = apply fogNewAlpha instantly).</param>
        public void SetFogOfWarAlpha(Vector3 worldPosition, float radius, float fogNewAlpha, float duration) {
            SetFogOfWarAlpha(worldPosition, radius, fogNewAlpha, true, duration, _fogOfWarSmoothness, _fogOfWarRestoreDelay, _fogOfWarRestoreDuration);
        }


        /// <summary>
        /// Changes the alpha value of the fog of war at world position creating a transition from current alpha value to specified target alpha. It takes into account FogOfWarCenter and FogOfWarSize.
        /// Note that only x and z coordinates are used. Y (vertical) coordinate is ignored.
        /// </summary>
        /// <param name="worldPosition">in world space coordinates.</param>
        /// <param name="radius">radius of application in world units.</param>
        /// <param name="fogNewAlpha">target alpha value.</param>
        /// <param name="duration">duration of transition in seconds (0 = apply fogNewAlpha instantly).</param>
        /// <param name="smoothness">border smoothness.</param>
        public void SetFogOfWarAlpha(Vector3 worldPosition, float radius, float fogNewAlpha, float duration, float smoothness) {
            SetFogOfWarAlpha(worldPosition, radius, fogNewAlpha, true, duration, smoothness, _fogOfWarRestoreDelay, _fogOfWarRestoreDuration);
        }

        /// <summary>
        /// Changes the alpha value of the fog of war at world position creating a transition from current alpha value to specified target alpha. It takes into account FogOfWarCenter and FogOfWarSize.
        /// Note that only x and z coordinates are used. Y (vertical) coordinate is ignored.
        /// </summary>
        /// <param name="worldPosition">in world space coordinates.</param>
        /// <param name="radius">radius of application in world units.</param>
        /// <param name="fogNewAlpha">target alpha value.</param>
        /// <param name="blendAlpha">if new alpha is combined with preexisting alpha value or replaced.</param>
        /// <param name="duration">duration of transition in seconds (0 = apply fogNewAlpha instantly).</param>
        /// <param name="smoothness">border smoothness.</param>
        /// <param name="fuzzyness">randomization of border noise.</param>
        /// <param name="restoreDelay">delay before the fog alpha is restored. Pass 0 to keep change forever.</param>
        /// <param name="restoreDuration">restore duration in seconds.</param>
        public void SetFogOfWarAlpha(Vector3 worldPosition, float radius, float fogNewAlpha, bool blendAlpha, float duration, float smoothness, float restoreDelay, float restoreDuration) {
            if (_fogOfWarTexture == null || fogOfWarColorBuffer == null || fogOfWarColorBuffer.Length == 0)
                return;

            float tx = (worldPosition.x - _fogOfWarCenter.x) / _fogOfWarSize.x + 0.5f;
            if (tx < 0 || tx > 1f)
                return;
            float tz = (worldPosition.z - _fogOfWarCenter.z) / _fogOfWarSize.z + 0.5f;
            if (tz < 0 || tz > 1f)
                return;

            int tw = _fogOfWarTexture.width;
            int th = _fogOfWarTexture.height;
            int px = (int)(tx * tw);
            int pz = (int)(tz * th);
            float sm = 0.0001f + smoothness;
            int colorBufferPos = pz * tw + px;
            byte newAlpha8 = (byte)(fogNewAlpha * 255);
            float tr = radius / _fogOfWarSize.z;
            int delta = (int)(th * tr);
            int deltaSqr = delta * delta;
            for (int r = pz - delta; r <= pz + delta; r++) {
                if (r > 0 && r < th - 1) {
                    for (int c = px - delta; c <= px + delta; c++) {
                        if (c > 0 && c < tw - 1) {
                            int distanceSqr = (pz - r) * (pz - r) + (px - c) * (px - c);
                            if (distanceSqr <= deltaSqr) {
                                colorBufferPos = r * tw + c;
                                Color32 colorBuffer = fogOfWarColorBuffer[colorBufferPos];
                                if (!blendAlpha) colorBuffer.a = 255;
                                distanceSqr = deltaSqr - distanceSqr;
                                float t = (float)distanceSqr / (deltaSqr * sm);
                                t = 1f - t;
                                if (t < 0) t = 0; else if (t > 1f) t = 1f;
                                byte targetAlpha = (byte)(newAlpha8 + (colorBuffer.a - newAlpha8) * t); // Mathf.Lerp(newAlpha8, colorBuffer.a, t);
                                if (targetAlpha < 255) {
                                    if (duration > 0) {
                                        AddFogOfWarTransitionSlot(c, r, colorBuffer.a, targetAlpha, 0, duration);
                                    } else {
                                        colorBuffer.a = targetAlpha;
                                        fogOfWarColorBuffer[colorBufferPos] = colorBuffer;
                                        //fogOfWarTexture.SetPixel(c, r, colorBuffer);
                                        requiresTextureUpload = true;
                                        if (restoreDelay > 0) {
                                            AddFogOfWarTransitionSlot(c, r, targetAlpha, 255, restoreDelay, restoreDuration);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Changes the alpha value of the fog of war within bounds creating a transition from current alpha value to specified target alpha. It takes into account FogOfWarCenter and FogOfWarSize.
        /// Note that only x and z coordinates are used. Y (vertical) coordinate is ignored.
        /// </summary>
        /// <param name="bounds">in world space coordinates.</param>
        /// <param name="fogNewAlpha">target alpha value.</param>
        /// <param name="duration">duration of transition in seconds (0 = apply fogNewAlpha instantly).</param>
        public void SetFogOfWarAlpha(Bounds bounds, float fogNewAlpha, float duration) {
            SetFogOfWarAlpha(bounds, fogNewAlpha, true, duration, _fogOfWarSmoothness, _fogOfWarRestoreDelay, _fogOfWarRestoreDuration);
        }

        /// <summary>
        /// Changes the alpha value of the fog of war within bounds creating a transition from current alpha value to specified target alpha. It takes into account FogOfWarCenter and FogOfWarSize.
        /// Note that only x and z coordinates are used. Y (vertical) coordinate is ignored.
        /// </summary>
        /// <param name="bounds">in world space coordinates.</param>
        /// <param name="fogNewAlpha">target alpha value.</param>
        /// <param name="duration">duration of transition in seconds (0 = apply fogNewAlpha instantly).</param>
        /// <param name="smoothness">border smoothness.</param>
        public void SetFogOfWarAlpha(Bounds bounds, float fogNewAlpha, float duration, float smoothness) {
            SetFogOfWarAlpha(bounds, fogNewAlpha, true, duration, smoothness, _fogOfWarRestoreDelay, _fogOfWarRestoreDuration);
        }


        /// <summary>
        /// Changes the alpha value of the fog of war within bounds creating a transition from current alpha value to specified target alpha. It takes into account FogOfWarCenter and FogOfWarSize.
        /// Note that only x and z coordinates are used. Y (vertical) coordinate is ignored.
        /// </summary>
        /// <param name="bounds">in world space coordinates.</param>
        /// <param name="fogNewAlpha">target alpha value.</param>
        /// <param name="blendAlpha">if new alpha is combined with preexisting alpha value or replaced.</param>
        /// <param name="duration">duration of transition in seconds (0 = apply fogNewAlpha instantly).</param>
        /// <param name="smoothness">border smoothness.</param>
        /// <param name="fuzzyness">randomization of border noise.</param>
        /// <param name="restoreDelay">delay before the fog alpha is restored. Pass 0 to keep change forever.</param>
        /// <param name="restoreDuration">restore duration in seconds.</param>
        public void SetFogOfWarAlpha(Bounds bounds, float fogNewAlpha, bool blendAlpha, float duration, float smoothness, float restoreDelay, float restoreDuration) {
            if (_fogOfWarTexture == null || fogOfWarColorBuffer == null || fogOfWarColorBuffer.Length == 0)
                return;

            Vector3 worldPosition = bounds.center;
            float tx = (worldPosition.x - _fogOfWarCenter.x) / _fogOfWarSize.x + 0.5f;
            if (tx < 0 || tx > 1f)
                return;
            float tz = (worldPosition.z - _fogOfWarCenter.z) / _fogOfWarSize.z + 0.5f;
            if (tz < 0 || tz > 1f)
                return;

            int tw = _fogOfWarTexture.width;
            int th = _fogOfWarTexture.height;
            int px = (int)(tx * tw);
            int pz = (int)(tz * th);
            int colorBufferPos = pz * tw + px;
            byte newAlpha8 = (byte)(fogNewAlpha * 255);
            float trz = bounds.extents.z / _fogOfWarSize.z;
            float trx = bounds.extents.x / _fogOfWarSize.x;
            float aspect1 = trx > trz ? 1f : trz / trx;
            float aspect2 = trx > trz ? trx / trz : 1f;
            int deltaz = (int)(th * trz);
            int deltazSqr = deltaz * deltaz;
            int deltax = (int)(tw * trx);
            int deltaxSqr = deltax * deltax;
            float sm = 0.0001f + smoothness;
            for (int r = pz - deltaz; r <= pz + deltaz; r++) {
                if (r > 0 && r < th - 1) {
                    int distancezSqr = (pz - r) * (pz - r);
                    distancezSqr = deltazSqr - distancezSqr;
                    float t1 = (float)distancezSqr * aspect1 / (deltazSqr * sm);
                    for (int c = px - deltax; c <= px + deltax; c++) {
                        if (c > 0 && c < tw - 1) {
                            int distancexSqr = (px - c) * (px - c);
                            colorBufferPos = r * tw + c;
                            Color32 colorBuffer = fogOfWarColorBuffer[colorBufferPos];
                            if (!blendAlpha) colorBuffer.a = 255;
                            distancexSqr = deltaxSqr - distancexSqr;
                            float t2 = (float)distancexSqr * aspect2 / (deltaxSqr * sm);
                            float t = t1 < t2 ? t1 : t2;
                            t = 1f - t;
                            if (t < 0) t = 0; else if (t > 1f) t = 1f;
                            byte targetAlpha = (byte)(newAlpha8 + (colorBuffer.a - newAlpha8) * t); // Mathf.Lerp(newAlpha8, colorBuffer.a, t);
                            if (targetAlpha < 255) {
                                if (duration > 0) {
                                    AddFogOfWarTransitionSlot(c, r, colorBuffer.a, targetAlpha, 0, duration);
                                } else {
                                    colorBuffer.a = targetAlpha;
                                    fogOfWarColorBuffer[colorBufferPos] = colorBuffer;
                                    //fogOfWarTexture.SetPixel(c, r, colorBuffer);
                                    requiresTextureUpload = true;
                                    if (restoreDelay > 0) {
                                        AddFogOfWarTransitionSlot(c, r, targetAlpha, 255, restoreDelay, restoreDuration);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Restores fog of war to full opacity
        /// </summary>
        /// <param name="worldPosition">World position.</param>
        /// <param name="radius">Radius.</param>
        public void ResetFogOfWarAlpha(Vector3 worldPosition, float radius) {
            if (_fogOfWarTexture == null || fogOfWarColorBuffer == null || fogOfWarColorBuffer.Length == 0)
                return;

            float tx = (worldPosition.x - _fogOfWarCenter.x) / _fogOfWarSize.x + 0.5f;
            if (tx < 0 || tx > 1f)
                return;
            float tz = (worldPosition.z - _fogOfWarCenter.z) / _fogOfWarSize.z + 0.5f;
            if (tz < 0 || tz > 1f)
                return;

            int tw = _fogOfWarTexture.width;
            int th = _fogOfWarTexture.height;
            int px = (int)(tx * tw);
            int pz = (int)(tz * th);
            float tr = radius / _fogOfWarSize.z;
            int delta = (int)(th * tr);
            int deltaSqr = delta * delta;
            for (int r = pz - delta; r <= pz + delta; r++) {
                if (r > 0 && r < th - 1) {
                    for (int c = px - delta; c <= px + delta; c++) {
                        if (c > 0 && c < tw - 1) {
                            int distanceSqr = (pz - r) * (pz - r) + (px - c) * (px - c);
                            if (distanceSqr <= deltaSqr) {
                                int colorBufferPos = r * tw + c;
                                Color32 colorBuffer = fogOfWarColorBuffer[colorBufferPos];
                                colorBuffer.a = 255;
                                fogOfWarColorBuffer[colorBufferPos] = colorBuffer;
                                //fogOfWarTexture.SetPixel(c, r, colorBuffer);
                                requiresTextureUpload = true;
                            }
                        }
                    }
                }
            }
        }



        /// <summary>
        /// Restores fog of war to full opacity
        /// </summary>
        public void ResetFogOfWarAlpha(Bounds bounds) {
            ResetFogOfWarAlpha(bounds.center, bounds.extents.x, bounds.extents.z);
        }

        /// <summary>
        /// Restores fog of war to full opacity
        /// </summary>
        public void ResetFogOfWarAlpha(Vector3 position, Vector3 size) {
            ResetFogOfWarAlpha(position, size.x * 0.5f, size.z * 0.5f);
        }

        /// <summary>
        /// Restores fog of war to full opacity
        /// </summary>
        /// <param name="position">Position in world space.</param>
        /// <param name="extentsX">Half of the length of the rectangle in X-Axis.</param>
        /// <param name="extentsZ">Half of the length of the rectangle in Z-Axis.</param>
        public void ResetFogOfWarAlpha(Vector3 position, float extentsX, float extentsZ) {
            if (_fogOfWarTexture == null || fogOfWarColorBuffer == null || fogOfWarColorBuffer.Length == 0)
                return;

            float tx = (position.x - _fogOfWarCenter.x) / _fogOfWarSize.x + 0.5f;
            if (tx < 0 || tx > 1f)
                return;
            float tz = (position.z - _fogOfWarCenter.z) / _fogOfWarSize.z + 0.5f;
            if (tz < 0 || tz > 1f)
                return;

            int tw = _fogOfWarTexture.width;
            int th = _fogOfWarTexture.height;
            int px = (int)(tx * tw);
            int pz = (int)(tz * th);
            float trz = extentsZ / _fogOfWarSize.z;
            float trx = extentsX / _fogOfWarSize.x;
            int deltaz = (int)(th * trz);
            int deltax = (int)(tw * trx);
            for (int r = pz - deltaz; r <= pz + deltaz; r++) {
                if (r > 0 && r < th - 1) {
                    for (int c = px - deltax; c <= px + deltax; c++) {
                        if (c > 0 && c < tw - 1) {
                            int colorBufferPos = r * tw + c;
                            Color32 colorBuffer = fogOfWarColorBuffer[colorBufferPos];
                            colorBuffer.a = 255;
                            fogOfWarColorBuffer[colorBufferPos] = colorBuffer;
                            //fogOfWarTexture.SetPixel(c, r, colorBuffer);
                            requiresTextureUpload = true;
                        }
                    }
                }
            }
        }


        public void ResetFogOfWar(byte alpha = 255) {
            if (_fogOfWarTexture == null || !isPartOfScene)
                return;
            int h = _fogOfWarTexture.height;
            int w = _fogOfWarTexture.width;
            int newLength = h * w;
            if (fogOfWarColorBuffer == null || fogOfWarColorBuffer.Length != newLength) {
                fogOfWarColorBuffer = new Color32[newLength];
            }
            Color32 opaque = new Color32(alpha, alpha, alpha, alpha);
            for (int k = 0; k < newLength; k++) {
                fogOfWarColorBuffer[k] = opaque;
            }
            _fogOfWarTexture.SetPixels32(fogOfWarColorBuffer);
            _fogOfWarTexture.Apply();
            lastTransitionPos = -1;
            fowTransitionIndices.Clear();
            isDirty = true;
        }

        /// <summary>
        /// Gets or set fog of war state as a Color32 buffer. The alpha channel stores the transparency of the fog at that position (0 = no fog, 1 = opaque).
        /// </summary>
        public Color32[] fogOfWarTextureData {
            get {
                return fogOfWarColorBuffer;
            }
            set {
                fogOfWarEnabled = true;
                fogOfWarColorBuffer = value;
                if (value == null || _fogOfWarTexture == null)
                    return;
                if (value.Length != _fogOfWarTexture.width * _fogOfWarTexture.height)
                    return;
                _fogOfWarTexture.SetPixels32(fogOfWarColorBuffer);
                _fogOfWarTexture.Apply();
            }
        }

        void AddFogOfWarTransitionSlot(int x, int y, byte initialAlpha, byte targetAlpha, float delay, float duration) {

            // Check if this slot exists
            int index;
            int key = y * 64000 + x;

            if (!fowTransitionIndices.TryGetValue(key, out index)) {
                index = -1;
                for (int k = 0; k <= lastTransitionPos; k++) {
                    if (!fowTransitionList[k].enabled) {
                        index = k;
                        fowTransitionIndices[key] = index;
                        break;
                    }
                }
            }
            if (index >= 0) {
                if (fowTransitionList[index].enabled && (fowTransitionList[index].x != x || fowTransitionList[index].y != y)) {
                    index = -1;
                }
            }

            if (index < 0) {
                if (lastTransitionPos >= MAX_SIMULTANEOUS_TRANSITIONS - 1)
                    return;
                index = ++lastTransitionPos;
                fowTransitionIndices[key] = index;
            }

            fowTransitionList[index].x = x;
            fowTransitionList[index].y = y;
            fowTransitionList[index].duration = duration;
            fowTransitionList[index].startTime = Time.time;
            fowTransitionList[index].startDelay = delay;
            fowTransitionList[index].initialAlpha = initialAlpha;
            fowTransitionList[index].targetAlpha = targetAlpha;
            fowTransitionList[index].enabled = true;
        }


        /// <summary>
        /// Gets the current alpha value of the Fog of War at a given world position
        /// </summary>
        /// <returns>The fog of war alpha.</returns>
        /// <param name="worldPosition">World position.</param>
        public float GetFogOfWarAlpha(Vector3 worldPosition) {
            if (fogOfWarColorBuffer == null || fogOfWarColorBuffer.Length == 0 || _fogOfWarTexture == null)
                return 1f;

            float tx = (worldPosition.x - _fogOfWarCenter.x) / _fogOfWarSize.x + 0.5f;
            if (tx < 0 || tx > 1f)
                return 1f;
            float tz = (worldPosition.z - _fogOfWarCenter.z) / _fogOfWarSize.z + 0.5f;
            if (tz < 0 || tz > 1f)
                return 1f;

            int tw = _fogOfWarTexture.width;
            int th = _fogOfWarTexture.height;
            int px = (int)(tx * tw);
            int pz = (int)(tz * th);
            int colorBufferPos = pz * tw + px;
            if (colorBufferPos < 0 || colorBufferPos >= fogOfWarColorBuffer.Length)
                return 1f;
            return fogOfWarColorBuffer[colorBufferPos].a / 255f;
        }


        #region Gizmos

        void OnDrawGizmosSelected() {
            if (_maskEditorEnabled && _fogOfWarEnabled && !Application.isPlaying) {
                Vector3 pos = _fogOfWarCenter;
                pos.y = -10;
                pos.y += _baselineHeight + _height * 0.5f;
                Vector3 size = new Vector3(_fogOfWarSize.x, 0.1f, _fogOfWarSize.z);
                for (int k = 0; k < 5; k++) {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireCube(pos, size);
                    pos.y += 0.5f;
                    Gizmos.color = Color.white;
                    Gizmos.DrawWireCube(pos, size);
                    pos.y += 0.5f;
                }
            }
        }

        #endregion


        #endregion
    }


}