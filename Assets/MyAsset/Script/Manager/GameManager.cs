using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : SingletonPattern_IsA_Mono<GameManager>
{
    private void Start()
    {
        Setting_OnDemandRendering(3);
    }

    public void Setting_OnDemandRendering(int _interval)
    {
        Application.targetFrameRate = 60;
        OnDemandRendering.renderFrameInterval = _interval;  // 60 / _interval fps
    }
}
