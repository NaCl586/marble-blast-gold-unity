using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoadingManager : MonoBehaviour
{
    [Header("Loading")]
    public Slider loadingSlider;
    public TextMeshProUGUI loadingText;

    AsyncOperation opGame;
    AsyncOperation opSkybox;
    bool cancelRequested;

    void Start()
    {
        loadingText.text = MissionInfo.instance.levelName;
        StartCoroutine(LoadAsync());
    }

    IEnumerator LoadAsync()
    {
        // 1️⃣ Load GAME first
        opGame = SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive);
        opGame.allowSceneActivation = false;

        while (opGame.progress < 0.9f)
        {
            if (isCancelling)
                yield break;

            loadingSlider.value = Mathf.Clamp01(opGame.progress / 0.9f) * 0.5f;
            yield return null;
        }

        // Activate Game
        opGame.allowSceneActivation = true;
        while (!opGame.isDone)
            yield return null;

        // 2️⃣ Load SKYBOX second
        opSkybox = SceneManager.LoadSceneAsync(MissionInfo.instance.skybox, LoadSceneMode.Additive);
        opSkybox.allowSceneActivation = false;

        while (opSkybox.progress < 0.9f)
        {
            if (isCancelling)
                yield break;

            loadingSlider.value = 0.5f + Mathf.Clamp01(opSkybox.progress / 0.9f) * 0.5f;
            yield return null;
        }

        // Activate Skybox
        opSkybox.allowSceneActivation = true;
        while (!opSkybox.isDone)
            yield return null;
    }



    bool isCancelling;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            CancelLoading();
    }

    public void CancelLoading()
    {
        if (isCancelling) return;

        isCancelling = true;

        // Stop the coroutine cleanly
        StopAllCoroutines();

        // Load PlayMission in Single mode
        SceneManager.LoadScene("PlayMission", LoadSceneMode.Single);
    }
}
