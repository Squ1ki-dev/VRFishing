using UnityEngine;

namespace Code.Services.Input
{
    public interface IInputHandler
{
    Vector3 GetControllerForward();
    Vector3 GetControllerVelocity();
    void InitializeInputDevice();
    bool IsTriggerPressed();
    void ValidateInputDevice();
}
}

