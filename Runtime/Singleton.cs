using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Assertions;

namespace com.bbbirder.unity{
    [ExecuteInEditMode]
    public class Singleton<T> : SingletonBase where T:MonoBehaviour
    {
        static T m_Instance;
        public static T Instance{
            get{
                // m_Instance??=Resources.FindObjectsOfTypeAll<T>().FirstOrDefault();
                // m_Instance??=new GameObject(typeof(T).Name){
                //     hideFlags=HideFlags.HideAndDontSave
                // }.AddComponent<T>();
                //Note: Do not use the `??=` syntax for UnityEngine.Object as it does not work well with it!
                if(!m_Instance){
                    m_Instance = Resources.FindObjectsOfTypeAll<T>().FirstOrDefault();
                }
                if(!m_Instance){
                    m_Instance = new GameObject(typeof(T).Name){
                        hideFlags=HideFlags.HideAndDontSave
                    }.AddComponent<T>();
                }
                Assert.IsNotNull(m_Instance);
                return m_Instance;
            }
        }
        protected virtual void Awake(){
            if(m_Instance==null){
                m_Instance = this as T;
                if(Application.isPlaying){
                    DontDestroyOnLoad(gameObject);
                }
            }else{
                DestroySelf();
            }
        }
        
    }
    
    public abstract class SingletonBase:MonoBehaviour{
        /// <summary>
        /// whether to destroy instantiated singleton on script recompilation,
        /// which will be helpful when you modify the default value of fields frequently.
        /// </summary>
        public virtual bool destroyOnReloadDomain => true;
        public Dictionary<string,object> extraData {get;private set;}=new();
        internal void DestroySelf(){
            if(Application.isPlaying){
                Destroy(gameObject);
            }else{
                DestroyImmediate(gameObject);
            }
        }
    }
    class SingeltonManager{
        #if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        static void CleanOnRecompileIfNeeded(){
            Resources
                .FindObjectsOfTypeAll<SingletonBase>()
                .Where(inst=>inst.destroyOnReloadDomain)
                .ForEach(inst=>inst.DestroySelf());
        }
        #endif
    }
}
