using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CNVScript : MonoBehaviour
{
    // Start is called before the first frame update
    public int typeCNV = 1;//1 - Da, 2-Phan, 3-Thanh Chan
    private bool isVaCham = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter2D(Collider2D obj){
        if(obj.gameObject.layer == LayerMask.NameToLayer("player") && !isVaCham){
            GetComponent<BoxCollider2D>().enabled = false;
            //-tym
            if(typeCNV == 1 || typeCNV == 3) {
                GlobalInstance.Instance.gameManagerInstance.soundManager.PlayVapDa();
                obj.gameObject.GetComponent<Animator>().SetBool("isVaCham", true);  
                this.PostEvent(EventID.isVaChamCNV);
            } else {
                GlobalInstance.Instance.gameManagerInstance.soundManager.PlayDamPhan();
                this.PostEvent(EventID.isDongBang);
                Destroy(this.gameObject);
            }
            StartCoroutine(DelaySauVaCham1(obj.gameObject));
        }
    }
    IEnumerator DelaySauVaCham1(GameObject obj){
        yield return new WaitForSeconds(2.5f);
        obj.gameObject.GetComponent<Animator>().SetBool("isVaCham", false);
        StartCoroutine(DelaySauVaCham2());
    }
    IEnumerator DelaySauVaCham2(){
        yield return new WaitForSeconds(0.5f);
        GetComponent<BoxCollider2D>().enabled = true;
    }
}
