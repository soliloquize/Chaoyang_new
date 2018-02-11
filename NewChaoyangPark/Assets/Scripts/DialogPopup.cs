using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class DialogPopup : MonoBehaviour {

    void Awake()
    {
        this.transform.localScale = Vector3.zero;
    }

	void Start () {
        Sequence popup = DOTween.Sequence();
        popup.Append(this.transform.DOScale(1.2f, 0.1f))
            .Append(this.transform.DOScale(1f, 0.1f));
        Invoke("Destorythis", 1f);
	}

    void Destorythis()
    {
        Destroy(this.gameObject);
    }

}
