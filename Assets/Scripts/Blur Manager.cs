using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlurManager : MonoBehaviour
{
    [SerializeField] private Material blurMaterial;
    [SerializeField] private Camera blurCamera;
    void Start()
    {
        if (blurCamera.targetTexture != null)
        {
            blurCamera.targetTexture.Release();
        }
        blurCamera.targetTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32, 1);
        blurMaterial.SetTexture("_MainTex", blurCamera.targetTexture);

    }
}
