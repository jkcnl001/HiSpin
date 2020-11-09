using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUIBase
{
    IEnumerator Show(params int[] args);
    void Pause();
    void Resume();
    IEnumerator Close();
}
