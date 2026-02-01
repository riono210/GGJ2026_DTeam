using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

/// <summary>
/// VideoPlayerの再生が終了した際に自動的にタイトルシーンへ戻るスクリプト。
/// </summary>
public class VideoSceneReturner : MonoBehaviour
{
    [Header("遷移設定")]
    [Tooltip("戻りたいシーン名（例: TitleScene）")]
    [SerializeField] private string targetSceneName = "TitleScene";

    [Header("オプション")]
    [Tooltip("動画終了後に少し余韻（待機時間）を入れる場合は秒数を入力")]
    [SerializeField] private float delayAfterVideo = 0f;

    private VideoPlayer videoPlayer;
    private bool isTransitioning = false;

    private void Start()
    {
        // オブジェクトに付いているVideoPlayerを取得
        videoPlayer = GetComponent<VideoPlayer>();

        if (videoPlayer != null)
        {
            // 動画の再生終了イベント（loopPointReached）を登録
            videoPlayer.loopPointReached += OnVideoEnd;
        }
    }

    // 動画が最後まで再生された時に自動で呼ばれる
    private void OnVideoEnd(VideoPlayer vp)
    {
        if (isTransitioning) return;
        isTransitioning = true;

        if (delayAfterVideo > 0)
        {
            Invoke(nameof(LoadTargetScene), delayAfterVideo);
        }
        else
        {
            LoadTargetScene();
        }
    }

    private void LoadTargetScene()
    {
        SceneManager.LoadScene(targetSceneName);
    }
}