using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum INGAME_STATE
{
    NONE,
    INTRO,
    GAME,
    PAUSE,
    VICTORY,
    GAMEOVER,

    _MAX
}

public class GameScene : MonoBehaviour
{
    public CinemachineVirtualCamera vcam;
    public DollScript doll_scp;

    public GameObject intro_obj;
    public GameObject victory_obj;
    public GameObject gameover_obj;

    public GameObject unitAmong_prefab;
    public Transform unit_parent;
    List<GameObject> unitObj_lst;
    List<UnitScript> unitScp_lst;
    int unit_size = 50;
    public int liveunit_size = 50;

    GameObject player_obj;
    PlayerScript player_scp;
    int player_id;
    public static float speed = 50;

    float time;
    public Text timer_txt;
    public Text txtUI;

    public FixedJoystick joystick;
    public Animator Intro_ani;
    public AudioClip[] Mugunghua_as;

    static INGAME_STATE game_state = INGAME_STATE.NONE;
    public static INGAME_STATE GetGameState() { return game_state; }
    public static void SetGameState(INGAME_STATE _state)
    {
        switch (_state)
        {
            case INGAME_STATE.GAME:
                GameManager.Instance.Setting_OnDemandRendering(1);
                break;
            case INGAME_STATE.VICTORY:
            case INGAME_STATE.GAMEOVER:
                GameManager.Instance.Setting_OnDemandRendering(3);
                break;
        }
        game_state = _state;
    }

    public void Retry() { SceneManager.LoadScene("GameScene"); }
    public void GoHome() { SceneManager.LoadScene("TitleScene"); }

    //tmp
    float i, j;
    GameObject tmp_Obj;
    TextMesh tmp_Tm;

    bool AnimatorIsPlaying(Animator ani)
    {
        if (ani.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            return false;
        }
        return true;
    }
    
    private void Start()
    {
        intro_obj.SetActive(true);
        player_id = UnityEngine.Random.Range(1, unit_size + 1);
        unitObj_lst = new List<GameObject>();
        unitScp_lst = new List<UnitScript>();
        for (i = 0; i < unit_size; i++)
        {
            tmp_Obj = Instantiate(unitAmong_prefab, unit_parent);
            tmp_Obj.name = string.Format("unitAmong_{0:D2}", (int)i + 1);
            tmp_Tm = tmp_Obj.transform.GetChild(1).GetComponent<TextMesh>();
            tmp_Tm.text = string.Format("{0:D2}", (int)i + 1);
            tmp_Tm.color = Color.white;
            tmp_Obj.transform.position = new Vector3(i / 10 + 0.7f, i % 10, 0);

            if (i + 1 == player_id)
            {
                tmp_Obj.AddComponent<PlayerScript>();
                tmp_Obj.transform.GetChild(1).GetComponent<TextOutline>().outlineColor = Color.red;
                vcam.Follow = tmp_Obj.transform;

                player_obj = tmp_Obj;
                player_scp = player_obj.GetComponent<PlayerScript>();
            }

            unitObj_lst.Add(tmp_Obj);
            unitScp_lst.Add(tmp_Obj.GetComponent<UnitScript>());
            unitScp_lst[(int)i].gs_scp = this;
        }

        time = 5 * 60;
        SetTxtUI();
        SetGameState(INGAME_STATE.INTRO);
    }

    public void SetUnitsBeforePosition()
    {
        foreach (var item in unitScp_lst)
        {
            item.SetBeforePos();
        }
    }

    public void SetTxtUI()
    {
        txtUI.text = string.Format("생존자: {0}명\n사망자: {1}명", liveunit_size, unit_size - liveunit_size);
    }

    void SetTimer()
    {
        time -= Time.deltaTime;
        if (time <= 0)
        {
            time = 0;
            for (i = 0; i < unitScp_lst.Count; i++)
                unitScp_lst[(int)i].Dead();
        }
        timer_txt.text = string.Format("{0:D2}:{1:D2}", (int)(time / 60), (int)(time % 60));
    }

    private void FixedUpdate()
    {
        if (GetGameState() == INGAME_STATE.INTRO)
        {
            if (Intro_ani.GetInteger("state") <= 0)
            {
                intro_obj.SetActive(false);
                SetGameState(INGAME_STATE.GAME);
            }
            else
            {
                if (!AnimatorIsPlaying(Intro_ani))
                    Intro_ani.SetInteger("state", Intro_ani.GetInteger("state") - 1);
            }
        }
        else if (GetGameState() == INGAME_STATE.GAME)
        {
            SetTimer();
            doll_scp.FuncUpdate();
            if (player_scp.unit_scp.GetState() != UNIT_STATE.VICTORY_MOVE)
                player_scp.Move(joystick.Horizontal * speed, joystick.Vertical * speed);
            foreach (var item in unitScp_lst)
            {
                if (!item.GetIsPlayer())
                {
                    if (item.GetState() != UNIT_STATE.VICTORY_MOVE)
                    {
                        i = UnityEngine.Random.Range(0, 30 * (unit_size + 1 - liveunit_size));
                        if (i > 0 && (DollScript.state == DOLL_STATE.TURN || DollScript.state == DOLL_STATE.SEE))
                        {
                            item.StopMove();
                        }
                        else
                        {
                            j = 50;
                            item.Move(item.GetDir().ReturnStateToInt() * speed, speed - j + speed * 0.8f + UnityEngine.Random.Range(-j, j));
                        }
                    }
                }
                else
                {
                    if (item.GetState() == UNIT_STATE.VICTORY)
                    {
                        StartCoroutine(EndGameUI(INGAME_STATE.VICTORY));
                    }
                    else if (item.GetState() == UNIT_STATE.DEAD)
                    {
                        StartCoroutine(EndGameUI(INGAME_STATE.GAMEOVER));
                    }
                }
            }
        }
    }

    WaitForSeconds VictorymoveUp = new WaitForSeconds(1f);
    IEnumerator EndGameUI(INGAME_STATE _state)
    {
        yield return VictorymoveUp;
        switch (_state)
        {
            case INGAME_STATE.VICTORY:
                victory_obj.SetActive(true);
                SetGameState(INGAME_STATE.VICTORY);
                break;
            case INGAME_STATE.GAMEOVER:
                gameover_obj.SetActive(true);
                SetGameState(INGAME_STATE.GAMEOVER);
                break;
        }
    }
}
