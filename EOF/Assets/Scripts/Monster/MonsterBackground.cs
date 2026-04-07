using UnityEngine;

public class MonsterBackground : MonoBehaviour
{
    private GameObject _currentBackground;
    public void SpawnMonster(GameObject _backgroundPrefab = null)
    {
        if (_currentBackground != null) Destroy(_currentBackground);
        
        if (_backgroundPrefab != null)
        {
            _currentBackground = Instantiate(_backgroundPrefab);
            _currentBackground.transform.position = Vector3.zero;
            _currentBackground.AddComponent<BackgroundFitter>();
        }
    }
}
