using UnityEngine;

namespace UniRx.Examples
{
    /// <summary>
    /// SCENE:
    /// GAME_OBJECT:
    /// </summary>
    public class Sample05_ConvertFromCoroutineMonoBehaviour : MonoBehaviour
    {
        #region Monobehaviour Events

        // Start is called before the first frame update
        void Start()
        {
            Sample05_ConvertFromCoroutine.GetWWW("http://google.com/")
                .Subscribe(s => { Debug.LogFormat($"Rezultat pretvaranja korutine u observable: {s}"); })
                .AddTo(this);
        }

        #endregion
    }
}