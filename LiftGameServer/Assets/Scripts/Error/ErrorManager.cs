using UnityEngine;
using UnityEngine.Events;

namespace Lift
{
    public class ErrorManager : MonoBehaviour
    {
        public class ErrorEventParams
        {
            public string message;

            public ErrorEventParams(string message)
            {
                this.message = message;
            }
        }

        public static ErrorManager Singleton { get; private set; }

        [SerializeField]
        bool throwOnError;
        [Space]
        public UnityEvent<ErrorEventParams> OnError = new();

        void Awake()
        {
            if (Singleton == null)
            {
                Singleton = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void Error(string message)
        {
            OnError.Invoke(new ErrorEventParams(message));
            
            if (throwOnError)
            {
                throw new System.Exception(message);
            }
            Debug.LogError(message);
        }

        public void Exception(System.Exception e)
        {
            OnError.Invoke(new ErrorEventParams(e.Message));
            
            if (throwOnError)
            {
                throw e;
            }
            Debug.LogException(e);
        }
    }
}
