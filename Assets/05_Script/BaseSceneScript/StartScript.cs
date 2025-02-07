using System.Collections;
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
    [SerializeField] TextMeshProUGUI _questDescription;
    int _selectedIndex = 0;
    int _storyQuestNumber = 0;
    int _challengeQuestNumber = 0;
    bool _storyMode = false;
    public void Phase(int phase)
    {
        _selectedIndex = phase;
        _animator.SetInteger("phase", phase);
        _animator2.SetInteger("phase", phase);
    }

    public void NextQuest()
    {
        if (_storyMode)
        {
            if (_storyQuestNumber < _storyStageData.template.Length)
                _storyQuestNumber++;
        }
        else
        {
            if(_challengeQuestNumber < _ChallengeStageData.template.Length)
                _challengeQuestNumber++;
        }
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
            if(_challengeQuestNumber != 0)
                _challengeQuestNumber--;
        }
    }
    public void PhaseDown()
    {
        if (_selectedIndex > 1)
        {
            _selectedIndex = 1;
            _animator.SetInteger("phase", 1);
            _animator2.SetInteger("phase", 1);
        }
        else
        {
            _selectedIndex = 0;
            _animator.SetInteger("phase", 0);
            _animator2.SetInteger("phase", 0);
        }
    }
    public IEnumerator ShowModeSelect()
    {
        yield return new WaitForSeconds(3f);

    }
}
