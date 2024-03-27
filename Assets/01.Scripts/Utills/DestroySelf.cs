using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    [SerializeField] private float _time = 3;
    private float _currentime = 0;


    private void Update()
    {
        if (_time > _currentime)
        {
            _currentime += Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
