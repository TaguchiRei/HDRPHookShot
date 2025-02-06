using UnityEngine;
using UnityEngine.UI;

public class StartScript : MonoBehaviour
{
    [SerializeField] Image _image;

    void StartButtonPush()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
}
