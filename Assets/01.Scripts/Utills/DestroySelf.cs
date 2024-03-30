using UnityEngine;

enum DestroyType
{
    Trigger,
    Time
}
public class DestroySelf : MonoBehaviour
{
    [SerializeField] private DestroyType type;
    [SerializeField] private float _time = 3;
    private float _currentime = 0;


    private void Update()
    {
        if (type == DestroyType.Time)
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (type == DestroyType.Trigger)
        {
            Destroy(gameObject);
        }
    }

}
