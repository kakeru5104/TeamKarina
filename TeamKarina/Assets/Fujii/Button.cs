using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;

public class Button : MonoBehaviour
{
    public List<TMP_Dropdown> dropdowns;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {


        foreach(var drop in dropdowns)
        {
            drop.value = 0;
        }
    }
}
