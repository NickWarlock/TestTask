using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider hpBar;

    public void updateBar(float hp)
    {
        hpBar.value = hp;
    }
}
