using UnityEngine;
using UnityEngine.InputSystem;

namespace MapEditor
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public PlayerInput input;
        public InputAction Touch { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            Touch = input.actions["Touch"];
        }
    }
}
