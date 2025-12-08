using Unity.Mathematics;
using UnityEngine;

namespace MapEditor
{
    public class Spawner : MonoBehaviour
    {
        public Transform trans;
        
        public void Create(GameObject target)
        {
            float x = Mathf.Floor(trans.position.x);
            float y = Mathf.Floor(trans.position.y);
            
            Instantiate(target,  new Vector3(x, y), quaternion.identity);
        }
    }
}