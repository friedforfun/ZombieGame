using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHaveState 
{
    void SetState(BaseState state);

    BaseState GetState();
}
