using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartScript : MonoBehaviour
{
    [SerializeField] Image _image;
    [SerializeField] Animator _animator;
    [SerializeField] Animator _animator2;
    [SerializeField] StageData _storyStageData;
    [SerializeField] StageData _ChallengeStageData;
    [SerializeField] TextMeshProUGUI _questTitle;
    [SerializeField] TextMeshProUGUI _questDescription;
    [SerializeField] TextMeshProUGUI _returnButtonTMP;

    int _selectedIndex = 0;
    int _storyQuestNumber = 0;
    int _challengeQuestNumber = 0;
    bool _storyMode = false;
    SceneChangeManager _sceneChangeManager = new();

    PlayerData _playerData;

    private void Start()
    {
        _playerData = FindAnyObjectByType<PlayerData>();
        Cursor.lockState = CursorLockMode.None;
    }

    public void Phase(int phase)
    {
        if (_animator.GetNextAnimatorStateInfo(0).fullPathHash == 0)
        {
            _returnButtonTMP.text = "–ß‚é";
            _selectedIndex = phase;
            _animator.SetInteger("phase", phase);
            _animator2.SetInteger("phase", phase);
            if (phase == 2)
            {
                _storyMode = true;
            }
            else if (phase == 3)
            {
                _storyMode = false;
            }
            TextChange();
        }
    }

    public void NextQuest()
    {
        if (_storyMode)
        {
            if (_storyQuestNumber < _storyStageData.template.Length - 1 && _storyQuestNumber < _playerData.SaveData.Progress)
                _storyQuestNumber++;
        }
        else
        {
            if (_challengeQuestNumber < _ChallengeStageData.template.Length - 1 && _challengeQuestNumber < _playerData.SaveData.Progress)
                _challengeQuestNumber++;
        }
        TextChange();
    }

    public void PrevQuest()
    {
        if (_storyMode)
        {
            if (_storyQuestNumber != 0)
                _storyQuestNumber--;
        }
        else
        {
            if (_challengeQuestNumber != 0)
                _challengeQuestNumber--;
        }
        TextChange();
    }

    public void PhaseDown()
    {
        if (_selectedIndex == 4 || _selectedIndex <= 1)
        {
            if (_selectedIndex == 0)
            {
                Debug.Log("A");
                Application.Quit();
            }
            else
            {
                _selectedIndex = 0;
                _animator.SetInteger("phase", 0);
                _animator2.SetInteger("phase", 0);
            }
            _returnButtonTMP.text = "I—¹";
        }
        else
        {
            _selectedIndex = 1;
            _animator.SetInteger("phase", 1);
            _animator2.SetInteger("phase", 1);
        }
    }
    public void Decision()
    {
        if (_storyMode)
        {
            _sceneChangeManager.SceneChange($"Story{_storyQuestNumber}");
        }
        else
        {
            _sceneChangeManager.SceneChange($"Challenge{_challengeQuestNumber}");
        }
    }
    void TextChange()
    {
        if (_storyMode)
        {
            _questDescription.text = _storyStageData.template[_storyQuestNumber].StageDescription;
            _questTitle.text = _storyStageData.template[_storyQuestNumber].StageName;
        }
        else
        {
            _questDescription.text = _ChallengeStageData.template[_challengeQuestNumber].StageDescription;
            _questTitle.text = _ChallengeStageData.template[_challengeQuestNumber].StageName;
        }
    }
}
