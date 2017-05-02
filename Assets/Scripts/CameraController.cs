using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform _target;

    [SerializeField]
    private Arena _arena;

    private float _worldScreenHeight;
    private float _worldScreenWidth;

    private FocusArea _focusArea;
    private CameraConfig _config;

    [Zenject.Inject]
    private void Inject(CameraConfig cameraConfig)
    {
        _config = cameraConfig;
    }

    void Start()
    {
        _worldScreenHeight = Camera.main.orthographicSize * 2f;
        _worldScreenWidth = _worldScreenHeight / Screen.height * Screen.width;
        _focusArea = CreateFocusArea();
        
    }

    void Update()
    {
        _focusArea.Update(_target.position);

        float posX = Mathf.Clamp(_focusArea.Center.x, _arena.Bounds.min.x + _worldScreenWidth / 2,
            _arena.Bounds.max.x - _worldScreenWidth / 2);

        float posY = Mathf.Clamp(_focusArea.Center.y, _arena.Bounds.min.y + _worldScreenHeight / 2,
            _arena.Bounds.max.y - _worldScreenHeight / 2);

        transform.position = new Vector3(posX, posY, transform.position.z) ;
    }

    private FocusArea CreateFocusArea()
    {
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float cameraHeight = Camera.main.orthographicSize * 2;
        return new FocusArea(new Vector2(cameraHeight * screenAspect * _config.BorderMove, cameraHeight * _config.BorderMove));
    }

    public class FocusArea
    {
        public Vector2 Center;
        private float _left;
        private float _right;
        private float _top;
        private float _bottom;

        public FocusArea(Vector2 size)
        {
            _left = - size.x / 2;
            _right =  size.x / 2;
            _bottom = - size.y / 2;
            _top = size.y / 2;
            Center = new Vector2((_left + _right) / 2, (_top + _bottom) / 2);
        }

        public void Update(Vector2 targetPosition)
        {
            float shiftX = 0;

            if (targetPosition.x < _left)
            {
                shiftX = targetPosition.x - _left;
            }
            else if (targetPosition.x > _right)
            {
                shiftX = targetPosition.x - _right;
            }

            _left += shiftX;
            _right += shiftX;

            float shiftY = 0;
            if (targetPosition.y < _bottom)
            {
                shiftY = targetPosition.y - _bottom;
            }
            else if (targetPosition.y > _top)
            {
                shiftY = targetPosition.y - _top;
            }

            _top += shiftY;
            _bottom += shiftY;
            Center = new Vector2((_left + _right) / 2, (_top + _bottom) / 2);
        }
    }

}
