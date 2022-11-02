using UnityEngine;
//출처: https://mrhook.co.kr/266 [리프(리뷰하는 프로그래머) TV]
public class SingletonPattern_IsA_Mono<T> : MonoBehaviour where T : Component
{
    private static T m_instance = null;
    public static T Instance
    {
        get
        {
            m_instance = FindObjectOfType(typeof(T)) as T;
            if (m_instance == null)
            {
                m_instance = new GameObject(typeof(T).ToString(), typeof(T)).AddComponent<T>();
                DontDestroyOnLoad(m_instance);
            }
            return m_instance;
        }
    }

    public static bool DontDestroyInst(T _this)
    {
        if (_this.gameObject == Instance.gameObject)
        {
            DontDestroyOnLoad(Instance);
            return true;
        }
        Destroy(_this.gameObject); //변수 체킹 용 디버그 줄
        return false;
    }
}
