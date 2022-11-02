using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum DOLL_STATE
{
    NONE,
    IDLE,
    TURN,
    SEE,

    _MAX
}

public class DollScript : MonoBehaviour
{
    public GameScene gs_scp;
    public static DOLL_STATE state = DOLL_STATE.NONE;
    public Text[] text;
    int textOn = 0;
    Animator ani;
    float changetime = 0, changetime_MAX = 0;

    Ray2D shootRayL; // 레이
    Ray2D shootRayR; // 레이
    LineRenderer ray_lrL;
    LineRenderer ray_lrR;
    Vector3 rayL_dir = Vector2.up;
    Vector3 rayR_dir = Vector2.up;
    int _layerMask;
    float distanceL = 0;
    float distanceR = 0;
    bool _israyleftL = false;
    bool _israyleftR = true;
    float isSee = 0;
    float isSee_MAX;

    //tmp
    RaycastHit2D tmp_Hit;
    Vector3 tmp_V;

    private void Start()
    {
        ani = GetComponent<Animator>();
        ray_lrL = transform.GetChild(0).GetComponent<LineRenderer>();
        ray_lrR = transform.GetChild(1).GetComponent<LineRenderer>();
        _layerMask = (1 << LayerMask.NameToLayer("Wall")) + (1 << LayerMask.NameToLayer("Unit"));
        StopShoot();
        ResetTime(0.05f, 1f);
        state = DOLL_STATE.IDLE;
    }

    void StartShoot()
    {
        ray_lrL.enabled = true;
        ray_lrR.enabled = true;
        gs_scp.SetUnitsBeforePosition();
        isSee = 0;
        isSee_MAX = Random.Range(2f, 12f);
        SoundManager.Instance.Play(null, 3);
    }
    void StopShoot()
    {
        ray_lrL.enabled = false;
        ray_lrR.enabled = false;
        rayL_dir = Vector2.down;
        rayR_dir = Vector2.down;
        distanceL = 0;
        distanceR = 0;
        SoundManager.Instance.Stop(3);
    }
    void Shoot()
    {
        CheckHit(ref shootRayL, ref ray_lrL, ref distanceL, ref rayL_dir, ref _israyleftL);
        CheckHit(ref shootRayR, ref ray_lrR, ref distanceR, ref rayR_dir, ref _israyleftR);
        isSee += Time.deltaTime;
    }
    void CheckHit( ref Ray2D shootRay, ref LineRenderer ray_lr, ref float distance, ref Vector3 ray_dir, ref bool _israyleft)
    {
        tmp_Hit = Physics2D.Raycast(shootRay.origin, shootRay.direction, distance, _layerMask);
        shootRay.origin = transform.position;
        shootRay.direction = ray_dir;

        // is ray target
        if (tmp_Hit.collider != null)
        {
            // check ray tag 
            if (tmp_Hit.transform.gameObject.layer == LayerMask.NameToLayer("Unit"))
            {
                tmp_Hit.transform.GetComponent<UnitScript>().CheckMove();
            }
            distance = tmp_Hit.distance;
        }
        else
        {
            distance = 1000;
        }
        tmp_V = ray_dir * distance + ray_lr.GetPosition(0);
        tmp_V.z = -1;
        ray_lr.SetPosition(1, tmp_V);

        //각도 변경.
        if (_israyleft)
        {
            ray_dir.x -= Random.Range(0.1f, 1f) * Time.deltaTime;
            if (ray_dir.x <= -1)
                _israyleft = false;
        }
        else
        {
            ray_dir.x += Random.Range(0.1f, 1f) * Time.deltaTime;
            if (ray_dir.x >= 1)
                _israyleft = true;
        }

        ray_lr.transform.rotation = Quaternion.Euler(ray_dir);
    }
    void SetState(DOLL_STATE _state)
    {
        if (state == _state)
            return;

        switch (_state)
        {
            case DOLL_STATE.TURN:
                if (state == DOLL_STATE.IDLE)
                    ani.SetBool("isTurn", true);
                break;
            case DOLL_STATE.IDLE:
                if (state == DOLL_STATE.SEE)
                    ani.SetBool("isTurn", false);
                break;
        }
        state = _state;
    }

    void ResetTime(float min, float max)
    {
        changetime = 0;
        changetime_MAX = Random.Range(min, max);
    }
    bool CheckTime()
    {
        if (changetime >= changetime_MAX)
        {
            ResetTime(0.05f, 1f);
            return true;
        }
        return false;
    }

    bool AnimatorIsPlaying()
    {
        if (ani.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            return false;
        }
        return true;
    }

    public void FuncUpdate()
    {
        if (GameScene.GetGameState() == INGAME_STATE.GAME)
        {
            changetime += Time.deltaTime;
            if (state == DOLL_STATE.IDLE)
            {
                if (CheckTime())
                {
                    if (!TextOn())
                    {
                        StartShoot();
                        SetState(DOLL_STATE.TURN);
                    }
                }
            }
            else if (state == DOLL_STATE.TURN)
            {
                if (!AnimatorIsPlaying())
                {
                    StartShoot();
                    SetState(DOLL_STATE.SEE);
                }
            }
            else if (state == DOLL_STATE.SEE)
            {
                if (isSee >= isSee_MAX)
                {
                    if (!AnimatorIsPlaying())
                    {
                        ResetTime(0.05f, 1f);
                        StopShoot();
                        TextOff();
                        SetState(DOLL_STATE.IDLE);
                    }
                }
                else
                {
                    Shoot();
                }

            }
        }
    }

    bool TextOn()
    {
        if (textOn >= text.Length)
        {
            return false;
        }
        SoundManager.Instance.Play(gs_scp.Mugunghua_as[textOn], 2);
        text[textOn++].gameObject.SetActive(true);
        return true;
    }

    void TextOff()
    {
        textOn = 0;
        for (int i = 0; i < text.Length; i++)
            text[i].gameObject.SetActive(false);
    }
}
