using UnityEngine;
using TMPro; // TextMeshPro用

public class ScoreManager : MonoBehaviour
{
    [Header("UIの設定")]
    public TextMeshProUGUI scoreText;

    [Header("現在のスコア（ここをインスペクターでいじってください）")]
    public int currentScore = 0;

    [Header("表示の形式（例：000000）")]
    public string scoreFormat = "D6"; // D6は「6桁の整数」という意味

    void Update()
    {
        // テキストがアサインされているか確認
        if (scoreText != null)
        {
            // インスペクターの currentScore をテキストに反映
            // .ToString("D6") と書くと、100 が "000100" になります
            scoreText.text = currentScore.ToString(scoreFormat);
        }
    }
}