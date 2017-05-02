using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InGamePanel : MonoBehaviour
{

    public Button Pause;
    public Transform LifesContainer;
    public Text ScoreText;
    public GameObject GameOverText;

    private List<GameObject> _lifesObject;

    private void Start()
    {
        EventManager.OnScoreUp += SetScore;
        Pause.onClick.AddListener(OnPauseButtonClick);
    }

    public void CreateLifesPanel(int hp)
    {
        _lifesObject = new List<GameObject>();
        AddHeart(hp);
    }


    public void UpdateHealth(int health)
    {
        if (_lifesObject.Count > health)
        {
            RemoveHeart(_lifesObject.Count - health);
        }
        else
        {
            AddHeart(health - _lifesObject.Count);
        }
    }

    private void RemoveHeart(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Destroy(_lifesObject.First());
            _lifesObject.RemoveAt(0);
        }
    }

    private void AddHeart(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject heartObject = Object.Instantiate(Resources.Load("Prefabs/Heart") as GameObject, LifesContainer);
            heartObject.transform.localScale = Vector3.one;
            _lifesObject.Add(heartObject);
        }
    }

    public void SetScore(int score)
    {
        ScoreText.text = score.ToString();
    }

    private void OnDestroy()
    {
        EventManager.OnScoreUp -= SetScore;
    }

    public void DisplayGameOver()
    {
        GameOverText.SetActive(true);
        LifesContainer.gameObject.SetActive(false);
    }

    private void OnPauseButtonClick()
    {
        EventManager.Pause();
        Pause.onClick.RemoveAllListeners();
        Pause.onClick.AddListener(OnUnPauseButtonClick);
    }

    private void OnUnPauseButtonClick()
    {
        EventManager.UnPause();
        Pause.onClick.RemoveAllListeners();
        Pause.onClick.AddListener(OnPauseButtonClick);

    }

}
