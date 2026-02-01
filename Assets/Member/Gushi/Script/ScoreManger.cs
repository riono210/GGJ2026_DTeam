using UnityEngine;
using TMPro; // TextMeshPro用

public class ScoreManager : MonoBehaviour
{
    [Header("UIの設定")]
    public TextMeshProUGUI scoreText;

    [Header("現在のステータス")]
    [Range(0, 3)] public int currentLives = 3;          // 残機（最大3）
    [Range(0, 5000)] public float currentDistance = 0f; // 進んだ距離（最大5000）

    [Header("表示の形式")]
    public string scoreFormat = "D6"; // 6桁の整数表示

    // 最大スコア 999,999 にするための重み設定
    private const int DISTANCE_WEIGHT = 150; // 5000m * 150 = 750,000点
    private const int LIFE_WEIGHT = 83333;   // 3機 * 83333 = 249,999点

    void Update()
    {
        // テキストがアサインされているか確認
        if (scoreText != null)
        {
            // 残機と距離からスコアを算出
            long total = ((long)currentDistance * DISTANCE_WEIGHT) + (currentLives * LIFE_WEIGHT);

            // 999,999を超えないように制限
            if (total > 999999) total = 999999;

            // 文字列に変換して反映
            scoreText.text = total.ToString(scoreFormat);
        }
    }
}