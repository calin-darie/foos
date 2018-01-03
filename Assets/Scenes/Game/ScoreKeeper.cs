using System;
using UnityEngine;
using UnityEngine.UI;

public class ScoreKeeper : MonoBehaviour
{
    public Transform YellowScoreTriggerGameObject;
    public Transform BlackScoreTriggerGameObject;
    Text _scoreText;

    int _yellowScore;
    int _blackScore;

    private int YellowScore
    {
        get { return _yellowScore; }
        set
        {
            _yellowScore = value;
            UpdateScoreText();
        }
    }

    int BlackScore
    {
        get { return _blackScore; }
        set
        {
            _blackScore = value; 
            UpdateScoreText();
        }
    }

    private void UpdateScoreText()
    {
        _scoreText.text = string.Format("yellow {0} - {1} black", YellowScore, BlackScore);
    }

    void Start ()
    {
        _scoreText = gameObject.GetComponentInChildren<Text>();
	    InitScore();
	    InitScore(YellowScoreTriggerGameObject, () => YellowScore ++ );
	    InitScore(BlackScoreTriggerGameObject, () => BlackScore ++ );
	}

    private void InitScore()
    {
        YellowScore = 0;
        BlackScore = 0;
    }

    private void InitScore(Transform scoreTriggerGameObject, Action incrementScore)
    {
        var scoreTrigger = scoreTriggerGameObject.GetComponent<ScoreTrigger>();
        scoreTrigger.Scored += incrementScore;
    }

    // Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0))
	    {
	        InitScore();
        }
	}
}
