using UnityEngine;

public interface IFloatParameterTarget
{
    float GetParameterValue(string parameterId);
    void SetParameterValue(string parameterId, float value);
}
