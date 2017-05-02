using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : MonoBehaviour
{
    public Button PlayButton;
    public Button ExitButton;

    void Start()
    {
        PlayButton.onClick.AddListener(Play);
        ExitButton.onClick.AddListener(CloseApp);
    }

    private void Play()
    {
        EventManager.UnPause();
        PlayButton.enabled = false;
        ExitButton.enabled = false;
        FadeOut();
    }

    private void CloseApp()
    {
        Application.Quit();
    }

    private void FadeOut()
    {
        foreach (Image image in transform.GetComponentsInChildren<Image>())
        {
            image.DOFade(0f, 1f)
                .SetEase(Ease.OutExpo)
                .Play();
        }
        foreach (Text image in transform.GetComponentsInChildren<Text>())
        {
            image.DOFade(0f, 1f)
                .SetEase(Ease.OutExpo)
                .Play();
        }

        GetComponent<Image>().DOFade(0f, 1f)
             .SetEase(Ease.OutExpo)
             .Play().OnComplete(() => gameObject.SetActive(false));
    }
}
