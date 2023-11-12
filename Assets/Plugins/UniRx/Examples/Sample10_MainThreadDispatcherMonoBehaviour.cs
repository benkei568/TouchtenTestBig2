using UnityEngine;

namespace UniRx.Examples
{
    /// <summary>
    /// SCENE:
    /// GAME_OBJECT:
    /// </summary>
    public class Sample10_MainThreadDispatcherMonoBehaviour : MonoBehaviour
    {
        #region Monobehaviour Events

        // Start is called before the first frame update
        void Start()
        {
            Sample10_MainThreadDispatcher mainThreadDispatcher = new Sample10_MainThreadDispatcher();
            mainThreadDispatcher.Run();
        }
        
        #endregion
    }
}