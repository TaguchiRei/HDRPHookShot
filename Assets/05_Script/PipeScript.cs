using UnityEngine;

public class PipeScript : MonoBehaviour
{
    [SerializeField] int _pipeHP = 20;
    bool _active = false;
    public TextManager TextManager;
    public void PipeChange(int i)
    {
        if (_active)
        {
            _pipeHP -= i;
            if (_pipeHP < 0)
            {
                GetComponent<Animator>().SetBool("ColorChange", false);
                TextManager.phase++;
                _active = false;
                Debug.Log("Break");
            }
        }
    }

    public void PipeActivate()
    {
        _active = true;
        GetComponent<Animator>().SetBool("ColorChange", true);
    }
}
