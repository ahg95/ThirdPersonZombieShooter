using UnityEngine;

public class TestCrossHair : MonoBehaviour
{
    [SerializeField]
    CrossHair _crossHair;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            _crossHair.RegisterShot();
    }
}
