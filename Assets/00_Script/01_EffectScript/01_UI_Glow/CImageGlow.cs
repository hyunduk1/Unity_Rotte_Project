using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CImageGlow : MonoBehaviour
{
    /// <summary> 発光色のシェーダーパラメータ </summary>
    private static readonly int PROPERTY_EMISSION_COLOR = Shader.PropertyToID("_EmissionColor");

    /// <summary> 発光色（HDR） </summary>
    [ColorUsage(false, true)]
    public Color EmissionColor;

    /// <summary> マテリアル </summary>
    private Material _mat;

    /// <summary> マテリアルアクセサ </summary>
    private Material Mat
    {
        get
        {
            if (_mat != null)
            {
                return _mat;
            }
            // マテリアルがない時に呼び出されたら初期化
            Initialize();
            return _mat;
        }
    }

    /// <summary>
    /// 初期化
    /// </summary>
    private void Initialize()
    {
        Image image = GetComponent<Image>();
        // UI/Glowシェーダーでマテリアル生成
        Shader uiGlowShader = Shader.Find("UI/Glow");
        if (image.material == null || image.material.shader != uiGlowShader)
        {
            _mat = new Material(uiGlowShader);
        }
        else
        {
            _mat = new Material(image.material);
        }
        _mat.name = "UI-Glow (Instance)";
        image.material = _mat;
    }

    /// <summary>
    /// 更新
    /// </summary>
    private void Update()
    {
        Mat.SetColor(PROPERTY_EMISSION_COLOR, EmissionColor);
    }

    /// <summary>
    /// Inspector上の値が変更されたときに呼び出し
    /// </summary>
    private void OnValidate()
    {
        Mat.SetColor(PROPERTY_EMISSION_COLOR, EmissionColor);
    }

    /// <summary>
    /// 削除時に呼び出し
    /// </summary>
    private void OnDestroy()
    {
        // 生成したマテリアルがリークしないように明示的に削除
        if (_mat != null)
        {
            if (Application.isPlaying)
            {
                Destroy(_mat);
            }
            else
            {
                DestroyImmediate(_mat);
            }
        }
    }
}