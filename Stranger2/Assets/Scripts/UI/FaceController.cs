using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FaceController : MonoBehaviour
{
    private GameObject _player;
    private PlayerController _playerController;
    private PlayerRenderer _playerRenderer;

    private GameObject faceImage;
    private Image faceImageComponent;
    private GameObject angerImage;
    private Image angerImageComponent;

    public Sprite[] faceSprites = new Sprite[4];

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
            _playerController = _player.GetComponent<PlayerController>();
            _playerRenderer = _player.GetComponent<PlayerRenderer>();
        faceImage = transform.GetChild(0).gameObject;
            faceImageComponent = faceImage.GetComponent<Image>();
        angerImage = transform.GetChild(1).gameObject;
            angerImageComponent = angerImage.GetComponent<Image>();
    }

    void Update()
    {
        if (!GameController.pauseOn)
        {
            if (_playerRenderer.isCarryWall && _playerController.angerGauge <= 1.01f)
            {
                _playerController.angerGauge += Time.deltaTime * 0.01f * _playerController.imPatience; // 기본 1초에 1% 증가 * 증가율 조정(imPatience)
                // if (0 <= _playerController.angerGauge && _playerController.angerGauge <= 1f)
                // {
                    angerImageComponent.fillAmount = _playerController.angerGauge;
                    int tmp = Mathf.FloorToInt(angerImageComponent.fillAmount * 4f); // 0~0.24 > 0, 0.25 ~ 0.49 > 1, 0.5 ~ 0.74 > 2, 0.75 ~ 1 > 3
                    switch (tmp)
                    {
                        case 0:
                            faceImageComponent.sprite = faceSprites[0];
                            break;
                        case 1:
                            faceImageComponent.sprite = faceSprites[1];
                            break;
                        case 2:
                            faceImageComponent.sprite = faceSprites[2];
                            break;
                        case 3:
                            faceImageComponent.sprite = faceSprites[3];
                            break;
                        default:
                            break;
                    }
                // }
            }
            else if (!_playerRenderer.isCarryWall && _playerController.angerGauge >= -0.01f)
            {
                angerImageComponent.fillAmount = _playerController.angerGauge;
                _playerController.angerGauge -= Time.deltaTime * 0.01f * _playerController.imPatience; // 기본 1초에 1% 증가 * 감소율 조정(imPatience)
                faceImageComponent.sprite = faceSprites[0];
            }
        }
    } // End of Update

} // End of Script
