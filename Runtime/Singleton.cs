using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace com.bbbirder.unity{
    [ExecuteInEditMode]
    public class Singleton<T> : SingletonBase where T:Singleton<T>
    {
        static T m_Instance;
        public static T Instance{
            get{
                //Note: Do not use the `??=` syntax for UnityEngine.Object as it does not work well with it!
                if(!m_Instance){
                    #if UNITY_EDITOR
                    m_Instance = Resources.FindObjectsOfTypeAll<T>().FirstOrDefault(inst=>inst);
                    #else
                    m_Instance = FindObjectOfType<T>();
                    #endif
                }
                if(!m_Instance){
                    m_Instance = new GameObject(typeof(T).Name){
                        hideFlags=HideFlags.HideAndDontSave,
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
        /// whether to destroy on script recompilation.
        /// </summary>
        public virtual bool DestroyOnReloadDomain => true;
        /// <summary>
        /// whether to destroy on Application mode switchs from play mode to editor mode.
        /// </summary>
        public virtual bool DestroyOnExitPlayMode => false;
        /// <summary>
        /// whether to destroy on Application mode switchs from editor mode to play mode.
        /// </summary>
        public virtual bool DestroyOnExitEditMode => false;
        /// <summary>
        /// whether to destroy on new scene load.
        /// </summary>
        /// <value></value>
        public bool DestroyOnLoad {
            get=>m_DestroyOnLoad;
            set{
                if(value){
                    SceneManager.MoveGameObjectToScene(gameObject,SceneManager.GetActiveScene());
                }else{
                    DontDestroyOnLoad(gameObject);
                }
                m_DestroyOnLoad = value;
            }
        }
        private bool m_DestroyOnLoad = false;
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
        static void RemoveAllSingletons(Func<SingletonBase,bool> predicate){
            Resources
            .FindObjectsOfTypeAll<SingletonBase>()
            .Where(predicate)
            .ForEach(inst=>inst.DestroySelf());
        }
        
        #if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        static void CleanOnRecompileIfNeeded(){
            RemoveAllSingletons(inst=>inst.DestroyOnReloadDomain);
        }

        [UnityEditor.InitializeOnLoadMethod]
        static void CleanOnPlayModeChangeIfNeeded(){
            UnityEditor.EditorApplication.playModeStateChanged+=e=>{
                if(e==PlayModeStateChange.ExitingPlayMode){
                    RemoveAllSingletons(inst=>inst.DestroyOnExitPlayMode);
                }else if(e==PlayModeStateChange.ExitingEditMode){
                    RemoveAllSingletons(inst=>inst.DestroyOnExitEditMode);
                }else if(e==PlayModeStateChange.EnteredPlayMode){
                    Resources
                    .FindObjectsOfTypeAll<SingletonBase>()
                    .ForEach(inst=>inst.DestroyOnLoad ^= false);
                }
            };
        }
        #endif
    }
}
