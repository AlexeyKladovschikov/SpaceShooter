using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public float SpeedX = 0.01f;
    public float SpeedY = 0.1f;
    public bool Fade;
    public float FadeFrom;
    public float FadeTo;
    public float FadeTime;

    private bool _isInitialized;
    private ITargetable _target;
    private Material _material;

    public void Initialize(ITargetable target)
    {
        _target = target;
        _material = GetComponent<Renderer>().sharedMaterial;
        _isInitialized = true;
        _material.DOFade(FadeFrom, 0f);

        if (Fade)
        {
            _material.DOFade(FadeTo, FadeTime)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
                .Play();
        }
    }
	
	void Update ()
    {   
	    if (_isInitialized)
	    {
            float y = Mathf.Repeat(Time.time * SpeedY, 1);
            float x = Mathf.Repeat(_target.Position.x * SpeedX, 1);
            Vector2 offset = new Vector2(x, y);
            _material.SetTextureOffset("_MainTex", offset);
	    }

    }
}
