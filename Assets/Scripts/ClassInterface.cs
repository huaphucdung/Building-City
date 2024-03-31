using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnitModule
{
    void DoInitialize();
    void DoModule();

    void DoDestroy();
}