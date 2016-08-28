using UnityEngine;
using System.Collections;

public class MenuEvents : MonoBehaviour
{
    public void OnPlayEvent()
    {
           
    }

    public void OnQuitEvent()
    {
        GameController.Instance.RequestQuitGame();
    }
}
