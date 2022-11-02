using System.Collections;
using UnityEngine;

public enum UNIT_STATE
{
    IDLE,
    RUN,
    VICTORY_MOVE,
    VICTORY,
    DEAD,

    _MAX
}

public class UnitScript : MonoBehaviour
{
    //tmp
    float i, j;
    Direction.DIR_STATE tmp_Dir, tmp_Dir2;
    Transform tmp_Tns;
    UnitScript tmp_Us;
    Vector3 tmp_V, tmp_V2, tmp_V3;
    bool[] tmp_bool_Array = new bool[(int)UNIT_STATE._MAX - 1] { false, false, false, false };

    public class Direction
    {
        public enum DIR_STATE
        {
            LEFT,
            CENTER,
            RIGHT,

            _MAX
        }
        DIR_STATE state = DIR_STATE.CENTER;
        float changetime = 0, changetime_MAX = 0;

        //tmp
        float i;

        public DIR_STATE GetState()
        {
            return state;
        }

        void ResetTime()
        {
            changetime = changetime - changetime_MAX;
            changetime_MAX = Random.Range(0.3f, 0.8f);
        }
        bool CheckTime()
        {
            changetime += Time.deltaTime;
            if (changetime >= changetime_MAX)
            {
                return true;
            }
            return false;
        }
        public void ChangeState(DIR_STATE _dir)
        {
            ResetTime();
            state = _dir;
        }
        public void ChangeStateButNotList(DIR_STATE[] _dir)
        {
            ChangeState(_dir[Random.Range((int)0, (int)_dir.Length)]);
        }
        void CheckChangeRandomState()
        {
            if (CheckTime())
                ChangeState((DIR_STATE)Random.Range((int)0, (int)DIR_STATE._MAX));
        }

        public int ReturnStateToInt()
        {
            switch (state)
            {
                case DIR_STATE.LEFT:
                    i = -1;
                    break;
                case DIR_STATE.CENTER:
                    i = 0;
                    break;
                case DIR_STATE.RIGHT:
                    i = 1;
                    break;
                default:
                    Debug.LogError(string.Format("그런 state 없다 : {0}", state.ToString()));
                    return 0;
            }
            CheckChangeRandomState();
            return (int)i;
        }
    }

    UNIT_STATE state = UNIT_STATE.IDLE;
    Direction dir = new Direction();
    Rigidbody2D rig;
    Animator ani;
    bool isPlayer = false;
    CapsuleCollider2D col;
    Vector3 before_pos = Vector3.zero;
    public GameScene gs_scp;

    private void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        ani = transform.GetChild(0).GetComponent<Animator>();
        col = transform.GetComponent<CapsuleCollider2D>();
    }

    public bool GetIsPlayer() { return isPlayer; }
    public void SetIsPlayer(bool _isPlayer) { isPlayer = _isPlayer; }

    public Direction GetDir(){ return dir; }

    public UNIT_STATE GetState(){ return state; }
    void SetState(UNIT_STATE _state)
    {
        if (state == _state)
            return;

        state = _state;
        tmp_bool_Array[0] = false;
        tmp_bool_Array[1] = false;
        tmp_bool_Array[2] = false;
        tmp_bool_Array[3] = false;
        switch (state)
        {
            case UNIT_STATE.IDLE:
                tmp_bool_Array[0] = true;
                break;
            case UNIT_STATE.RUN:
                tmp_bool_Array[1] = true;
                break;
            case UNIT_STATE.VICTORY_MOVE:
                tmp_bool_Array[1] = true;
                break;
            case UNIT_STATE.VICTORY:
                tmp_bool_Array[2] = true;
                break;
            case UNIT_STATE.DEAD:
                tmp_bool_Array[3] = true;
                break;
        }
        ani.SetBool("isIdle", tmp_bool_Array[0]);
        ani.SetBool("isRun", tmp_bool_Array[1]);
        ani.SetBool("isVictory", tmp_bool_Array[2]);
        ani.SetBool("isDead", tmp_bool_Array[3]);
    }

    
    public void Move(float x, float y)
    {
        if (x == 0 && y == 0)
        {
            StopMove();
            return;
        }
        if (state == UNIT_STATE.DEAD || state == UNIT_STATE.VICTORY)
        {
            return;
        }
        tmp_V.x = x * Time.deltaTime;
        tmp_V.y = y * Time.deltaTime;
        rig.velocity = tmp_V;

        //방향
        tmp_V2 = transform.localScale;
        tmp_V3 = transform.GetChild(1).localScale;
        i = Mathf.Abs(tmp_V2.x);
        j = Mathf.Abs(tmp_V3.x);
        if (tmp_V.x < 0)
        {
            tmp_V2.x = -i;
            tmp_V3.x = -j;
        }
        else if (tmp_V.x > 0)
        {
            tmp_V2.x = i;
            tmp_V3.x = j;
        }
        transform.localScale = tmp_V2;
        transform.GetChild(1).localScale = tmp_V3;

        if (state == UNIT_STATE.IDLE || state == UNIT_STATE.RUN)
        {
            if (tmp_V != Vector3.zero)
                SetState(UNIT_STATE.RUN);
            else
                SetState(UNIT_STATE.IDLE);
        }
        tmp_V = Vector3.zero;
    }

    public void StopMove()
    {
        if (state != UNIT_STATE.RUN)
        {
            return;
        }
        rig.velocity = Vector3.zero;
        SetState(UNIT_STATE.IDLE);
    }

    public void SetBeforePos()
    {
        before_pos = transform.position;
    }
    public void CheckMove()
    {
        if (Vector3.Distance(before_pos, transform.position) >= 0.01f)
        {
            Dead();
            return;
        }
        before_pos = transform.position;
    }
    public void Dead()
    {
        if (state == UNIT_STATE.VICTORY_MOVE)
        {
            return;
        }
        SoundManager.Instance.Play(null, 1);
        gs_scp.liveunit_size--;
        gs_scp.SetTxtUI();
        SetState(UNIT_STATE.DEAD);
        col.enabled = false;
    }

    IEnumerator CountAttackDelay()
    {
        SetState(UNIT_STATE.VICTORY_MOVE);
        StopMove();
        for (int k = 0; k < 200; k++)
        {
            Move(0, GameScene.speed);
            yield return null;
        }
        col.enabled = false;
        SetState(UNIT_STATE.VICTORY);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (state != UNIT_STATE.DEAD)
        {
            if (state != UNIT_STATE.VICTORY)
                if (state != UNIT_STATE.VICTORY_MOVE)
                {
                    //공통.
                    if (collision.gameObject.layer == LayerMask.NameToLayer("Goal"))
                    {
                        StartCoroutine(CountAttackDelay());
                    }
                }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (state != UNIT_STATE.DEAD)
        {
            if (state != UNIT_STATE.VICTORY)
                if (state != UNIT_STATE.VICTORY_MOVE)
                {
                    if (!isPlayer)  //유닛 인공지능 체크.
                    {
                        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall") || collision.gameObject.layer == LayerMask.NameToLayer("Unit"))
                        {
                            tmp_Dir = Direction.DIR_STATE.CENTER;
                            i = transform.position.x;
                            tmp_Tns = collision.transform;
                            j = tmp_Tns.transform.position.x;

                            if (collision.gameObject.layer == LayerMask.NameToLayer("Unit"))
                                tmp_Us = tmp_Tns.GetComponent<UnitScript>();
                            // 오른쪽에 있을 경우
                            if (i < j)
                            {
                                tmp_Dir = Direction.DIR_STATE.LEFT;
                                tmp_Dir2 = Direction.DIR_STATE.RIGHT;
                            }
                            // 왼쪽에 있을 경우
                            else if (i > j)
                            {
                                tmp_Dir = Direction.DIR_STATE.RIGHT;
                                tmp_Dir2 = Direction.DIR_STATE.LEFT;
                            }
                            dir.ChangeState(tmp_Dir);
                            if (collision.gameObject.layer == LayerMask.NameToLayer("Unit"))
                                tmp_Us.dir.ChangeState(tmp_Dir2);
                        }
                    }
                }
        }
    }
}
