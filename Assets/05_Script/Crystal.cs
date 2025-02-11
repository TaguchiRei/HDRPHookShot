using UnityEngine;

public class Crystal : MonoBehaviour
{
    float _beamDmg = 0;
    [SerializeField] TextManager _textManager;

    private void Update()
    {
        _beamDmg += Time.deltaTime;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DMGZone") && _beamDmg > 5 && _textManager.PhaseChange)
        {
            _textManager.phase += 10000;
            _beamDmg = 0;
        }
    }
}
