using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class KeoScript : MonoBehaviour
{
    public int typeVatPham = 1; //1: keo, 2: sach
    void Start()
    {
        DOTween.Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter2D(Collider2D obj) {
        if(obj.gameObject.layer == LayerMask.NameToLayer("player")) {
            GetComponent<BoxCollider2D>().enabled = false;
            if(typeVatPham == 1) {
                this.PostEvent(EventID.isAnKeo);
            }
            else 
                this.PostEvent(EventID.isAnCauHoi);
            transform.GetChild(0).GetComponent<Animator>().enabled = false;
            transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.Linear).OnComplete(() => {
                Destroy(this.gameObject);
            });
        }
    }
}
