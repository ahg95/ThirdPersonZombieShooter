using UnityEngine;

public class TestArmAnimationController : MonoBehaviour
{
    [SerializeField]
    ArmAnimationController _armAnimationController;

    [SerializeField, Range(0, 1)]
    float _weight;

    private void OnValidate()
    {
        _armAnimationController.SetIKWeight(_weight);
    }
}
