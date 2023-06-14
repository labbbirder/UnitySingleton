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
                    DestroyOnSceneUnload = DestroyOnSceneUnload;
                }
            }else{
                DestroySelf();
            }
        }
    }
    
    public abstract class SingletonBase:MonoBehaviour{
        SingletonDestroyCondition m_DestroyCondition = SingletonDestroyCondition.ReloadDomain;
        public virtual SingletonDestroyCondition DestroyCondition{
            get => m_DestroyCondition;
            private set {
                var flgSceneUnload = (value^m_DestroyCondition) & SingletonDestroyCondition.SceneUnload;
                if(flgSceneUnload!=0){
                    var hasFlag = 0 != (value&SingletonDestroyCondition.SceneUnload);
                    DestroyOnSceneUnload = hasFlag;
                }
                m_DestroyCondition = value;
            }
        }
        public bool DestroyOnSceneUnload{
            get => DestroyCondition.Contains(SingletonDestroyCondition.SceneUnload);
            set {
                if(value){
                    SceneManager.MoveGameObjectToScene(gameObject,SceneManager.GetActiveScene());
                    DestroyCondition |= SingletonDestroyCondition.SceneUnload;
                }else{
                    DontDestroyOnLoad(gameObject);
                    DestroyCondition &= ~SingletonDestroyCondition.SceneUnload;
                }
            }
        }
        internal void DestroySelf(){
            if(Application.isPlaying){
                Destroy(gameObject);
            }else{
                DestroyImmediate(gameObject);
            }
        }
        

    }
}
