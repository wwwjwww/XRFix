// Here're the fixed code lines from Assets/Scripts/ResetCenterOfMass.cs:

using UnityEngine;

public class ResetCenterOfMass : MonoBehaviour
{
    
    [SerializeField] private GameObject _gobj6;
    private GameObject _a6;
    private float _timeLimit = 5f;
    private float _timer;
    private bool _instantiateGobj;

    private void Awake()
    {
        _gobj6 = Instantiate(_gobj6);
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        if (_instantiateGobj && _timer >= _timeLimit)
        {
            Destroy(_a6);
            _timer = 0;
            _instantiateGobj = false;
        }
        else if (!_instantiateGobj && _timer >= _timeLimit)
        {
            _a6 = Instantiate(_gobj6);
            _timer = 0;
            _instantiateGobj 
    }
}
