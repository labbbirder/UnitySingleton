using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace com.bbbirder.unity{
    [ExecuteInEditMode]
    public class Singleton<T> : SingletonBase where T:Singleton<T>
    {
        const BindingFlags bindingFlags = BindingFlags.Default
            | BindingFlags.FlattenHierarchy
            | BindingFlags.Static
            | BindingFlags.Public
            | BindingFlags.NonPublic
            ;
        static T m_Instance;
        public static T Instance{
            get{
                if(!m_Instance){
                    #if UNITY_EDITOR
                    m_Instance = Resources.FindObjectsOfTypeAll<T>().FirstOrDefault(inst=>inst);
                    #else
                    m_Instance = FindObjectOfType<T>();
                    #endif
                }
                if(!m_Instance){
                    m_Instance = (T)typeof(T).GetMethod("CreateInstance",bindingFlags).Invoke(null,null);
                }
                Assert.IsNotNull(m_Instance);
                return m_Instance;
            }
        }
        protected static T CreateInstance(){
            return new GameObject(typeof(T).Name){ 
                hideFlags = HideFlags.HideAndDontSave ^ HideFlags.NotEditable,
            }.AddComponent<T>();
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
        /// whether to destroy on scene unload.
        /// </summary>
        /// <value></value>
        public bool DestroyOnSceneUnload {
            get=>m_DestroyOnSceneUnload;
            set{
                if(value){
                    SceneManager.MoveGameObjectToScene(gameObject,SceneManager.GetActiveScene());
                }else{
                    DontDestroyOnLoad(gameObject);
                }
                m_DestroyOnSceneUnload = value;
            }
        }
        private bool m_DestroyOnSceneUnload = false;

        internal void DestroySelf(){
            if(Application.isPlaying){
                Destroy(gameObject);
            }else{
                DestroyImmediate(gameObject);
            }
        }
    }
}
