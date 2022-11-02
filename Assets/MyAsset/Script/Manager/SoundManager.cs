using UnityEngine;

public class SoundManager : SingletonPattern_IsA_Mono<SoundManager>
{
    AudioSource bg, se1, se2, se3;

    //tmp
    AudioSource tmp_As;

    private void Awake()
    {
        bg = transform.GetChild(0).GetComponent<AudioSource>();
        se1 = transform.GetChild(1).GetComponent<AudioSource>();
        se2 = transform.GetChild(2).GetComponent<AudioSource>();
        se3 = transform.GetChild(3).GetComponent<AudioSource>();
    }

    public void Play(AudioClip _source, int _id)
    {
        switch (_id)
        {
            case 0:
                tmp_As = bg;
                break;
            case 1:
                tmp_As = se1;
                break;
            case 2:
                tmp_As = se2;
                break;
            case 3:
                tmp_As = se3;
                break;
            default:
                return;
        }
        if (_source != null)
            tmp_As.clip = _source;
        tmp_As.Play();
    }

    public void Stop(int _id)
    {
        switch (_id)
        {
            case 0:
                tmp_As = bg;
                break;
            case 1:
                tmp_As = se1;
                break;
            case 2:
                tmp_As = se2;
                break;
            case 3:
                tmp_As = se3;
                break;
            default:
                return;
        }
        tmp_As.Stop();
    }
}
