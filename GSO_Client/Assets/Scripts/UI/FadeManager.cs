using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    public static FadeManager instance;

    private int dotCount;
    private bool isLoadComplete = false; // 로딩 완료 플래그
    private void Awake()
    {
        if(instance == null)
            instance = this;
        FadeImage = gameObject.transform.Find("FadeImage").GetComponent<Image>();
        LoadingText = FadeImage.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    public Image FadeImage { get; private set; }
    public TextMeshProUGUI LoadingText { get; private set; }
    
    //Fade Var
    public float _fadeDuration = 1.0f;
    public float _textDuration = 0.5f; // 텍스트 변경 간격
    private bool isLoading = true;
    // Start is called before the first frame update
    
    void Start()
    {
        StartCoroutine(AnimateLoadingText());
        StartCoroutine(AsyncScene(Define.Scene.Forest));
    }
    
     public void StartFadeIn()
    { 
        StartCoroutine(AnimateLoadingText());
    }

    public void StartFadeOut()
    { 
        StartCoroutine(FadeOut());
    }
    
    private IEnumerator AnimateLoadingText()
    {
        dotCount = 0; // 점 갯수를 조정할 변수
        while (isLoading) // 로딩 상태가 true인 동안 반복
        {
            string dots = "";
            for (int i = 0; i < dotCount; i++)
            {
                dots += ".";
            }
            LoadingText.text = "Loading" + dots;
            dotCount = (dotCount + 1) % 4;
            yield return new WaitForSeconds(_textDuration);
        }

        // 조건 만족 시 페이드 아웃 실행
        StartCoroutine(FadeOut());
    }
    public void StopLoading() // 조건 만족 시 호출
    {
       isLoading = false; // 로딩 상태 종료
    }
    
    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        Color imageColor = FadeImage.color;
        Color textColor = LoadingText.color;

        while (elapsedTime < _fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            // 동일한 Alpha 값 적용
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / _fadeDuration);

            // 이미지와 텍스트의 Alpha를 같은 비율로 조정
            imageColor.a = alpha;
            FadeImage.color = imageColor;

            textColor.a = alpha;
            LoadingText.color = textColor;

            yield return null;
        }

        // 최종 Alpha 값 설정 (완전 불투명)
        imageColor.a = 1f;
        FadeImage.color = imageColor;

        textColor.a = 1f;
        LoadingText.color = textColor;

        StartCoroutine(AnimateLoadingText());
    }

    private IEnumerator FadeOut()
    {
        dotCount = PlayerPrefs.GetInt("dotCount");
        LoadingText.text = "Loading";
        for(int i=0; i<dotCount; i++)
            LoadingText.text += ".";
        float elapsedTime = 0f;

        Color imageColor = FadeImage.color;
        Color textColor = LoadingText.color;

        while (elapsedTime < _fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            // 동일한 Alpha 값 적용
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / _fadeDuration);

            // 이미지와 텍스트의 Alpha를 같은 비율로 조정
            imageColor.a = alpha;
            FadeImage.color = imageColor;

            textColor.a = alpha;
            LoadingText.color = textColor;

            yield return null;
        }

        // 최종 Alpha 값 설정 (완전 투명)
        imageColor.a = 0f;
        FadeImage.color = imageColor;

        textColor.a = 0f;
        LoadingText.color = textColor;
        
        Destroy(gameObject);
    }

    public void SetFadeData()
    {
        PlayerPrefs.SetInt("dotCount", dotCount);
    }

    public void GetFadeData()
    {
        dotCount = PlayerPrefs.GetInt("dotCount");
    }

    //로딩씬 인경우만 작동시키게
    public IEnumerator  AsyncScene(Define.Scene scene)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(Enum.GetName(typeof(Define.Scene), scene),LoadSceneMode.Additive);
        while (!operation.isDone)
        {
            yield return null;
        }
        
        Scene Gamescene = SceneManager.GetSceneByName("Forest");
        if (Gamescene.IsValid())
        {
            SceneManager.SetActiveScene(Gamescene);
            Debug.Log("Scene2 is now the active scene.");
        }
        
        Managers.Scene.UnLoadScene(Define.Scene.loading);
        
        C_EnterGame c_EnterGame = new C_EnterGame();
        //c_EnterGame.Credential =
        Managers.Network.Send(c_EnterGame);
        Debug.Log("Send c_EnterGame In GameScene");
        SetFadeData();
    }
    [ContextMenu("test")]
    public void SetLoadComplete() // 로딩 완료 조건을 만족시킬 함수
    {
        isLoadComplete = true;
    }
    
}
