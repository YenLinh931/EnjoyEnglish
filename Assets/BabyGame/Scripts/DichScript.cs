using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DichScript : MonoBehaviour
{
    private bool isVeDich = false;
    void Start()
    {
        this.RegisterListener(EventID.isVeDich, (param) => {
            isVeDich = true;
        });
    }

    void Update()
    {
        if(isVeDich) {
            isVeDich  = false;
            this.transform.GetChild(2).gameObject.SetActive(true);
        }
    }
}
