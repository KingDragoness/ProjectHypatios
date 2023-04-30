using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientLightController : MonoBehaviour
{


    private Color _startingAmbientColor;

    private AmbientLightVolume _currentVolume;
    private float _speed = 1f;

    public static AmbientLightController Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _startingAmbientColor = RenderSettings.ambientLight;
    }

    private void Update()
    {
        if (Time.timeScale == 0) return;

        if (_currentVolume != null)
        {
            Color c = Color.Lerp(RenderSettings.ambientLight, _currentVolume.ambientColor, Time.deltaTime * _speed * 0.1f);
            RenderSettings.ambientLight = c;
        }
        else
        {
            Color c = Color.Lerp(RenderSettings.ambientLight, _startingAmbientColor, Time.deltaTime * _speed * 0.1f);
            RenderSettings.ambientLight = c;
        }
    }

    public void TriggerVolume(AmbientLightVolume _target)
    {
        _currentVolume = _target;
        _speed = _target.entrySpeed;
    }

    public void ExitVolume(AmbientLightVolume _target)
    {
        if (_target == _currentVolume)
        {
            _speed = _target.exitSpeed;
            _currentVolume = null;
        }
    }

}
