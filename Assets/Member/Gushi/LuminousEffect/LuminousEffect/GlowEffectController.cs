using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

/// <summary>
/// オブジェクトを発光（Glow）させる演出を制御するスクリプト
/// ボタン入力などの外部イベントから StartGlow() を呼び出すことで動作します。
/// </summary>
public class GlowEffectController : MonoBehaviour
{
    [Header("発光の設定")]
    [ColorUsage(true, true)] // HDRカラー対応
    [SerializeField] private Color glowColor = Color.cyan;
    [SerializeField] private float maxIntensity = 3.0f; // 最大輝度
    [SerializeField] private float duration = 0.6f;    // 演出時間

    [Header("アニメーション")]
    [SerializeField] private AnimationCurve pulseCurve = new AnimationCurve(
        new Keyframe(0f, 0f),
        new Keyframe(0.15f, 1f), // 素早く最大に
        new Keyframe(1f, 0f)      // ゆっくり消える
    );

    private Material targetMat;
    private Color originalColor;
    private string targetProperty = "_Color";
    private bool isAnimating = false;

    void Awake()
    {
        // 描画コンポーネントの取得
        Renderer ren = GetComponent<Renderer>();
        if (ren == null) ren = GetComponentInChildren<Renderer>();
        
        if (ren != null)
        {
            targetMat = ren.material;
            
            // シェーダーのプロパティ名を自動判別
            if (targetMat.HasProperty("_BaseColor")) targetProperty = "_BaseColor";
            else if (targetMat.HasProperty("_MainColor")) targetProperty = "_MainColor";
            else if (targetMat.HasProperty("_GlowColor")) targetProperty = "_GlowColor";
            
            originalColor = targetMat.GetColor(targetProperty);
        }
    }

    /// <summary>
    /// 発光演出を開始します（ボタン等のOnClickから呼び出し可能）
    /// </summary>
    public void StartGlow()
    {
        if (targetMat == null || isAnimating) return;
        StartCoroutine(GlowRoutine());
    }

    private IEnumerator GlowRoutine()
    {
        isAnimating = true;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float curveValue = pulseCurve.Evaluate(t);

            // HDRカラーの計算：色 * 輝度
            Color current = Color.Lerp(originalColor, glowColor * maxIntensity, curveValue);
            targetMat.SetColor(targetProperty, current);

            yield return null;
        }

        targetMat.SetColor(targetProperty, originalColor);
        isAnimating = false;
    }

    // デバッグ用
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (!isAnimating)
            {
                StartGlow();
            }
        }
    }
}
