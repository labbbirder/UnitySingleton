using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using com.bbbirder.unity;
using static com.bbbirder.unity.SingletonBase;
using System.Collections.Generic;
using System.Reflection;

namespace com.bbbirder.unityeditor{
    static class SingeltonManager{
        static Dictionary<Type,SingletonCreateCondition> lut_singletonCreation = null;
        static void DestroyGameObject(GameObject go){
            if(Application.isPlaying){
                GameObject.Destroy(go);
            }else{
                GameObject.DestroyImmediate(go);
            }
        }
        static void RemoveAllSingletons(Func<SingletonBase,bool> predicate){
            if(Selection.activeGameObject?.GetComponent<SingletonBase>()){
                Selection.activeGameObject = null;
            }
            var instances = Resources
                .FindObjectsOfTypeAll<SingletonBase>()
                .Where(predicate)
                .ToArray();
            foreach(var inst in instances){
                DestroyGameObject(inst.gameObject);
            }
        }
        static Dictionary<Type,SingletonCreateCondition> GetSingletonTypes(){
            if(lut_singletonCreation!=null) return lut_singletonCreation;
            var baseType = typeof(SingletonAutoLoadAttribute);
            var thisAssemblyName = baseType.Assembly.GetName().ToString();
            return lut_singletonCreation = AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly 
                    => assembly.GetReferencedAssemblies().Any(r=>r.ToString()==thisAssemblyName)
                    && assembly.GetName().ToString() != thisAssemblyName)
                .SelectMany(assembly
                    => assembly.GetCustomAttributes(false).OfType<SingletonAutoLoadAttribute>())
                .Select(att=>(att.type,att.createCondition))
                .ToDictionary(d=>d.type,d=>d.createCondition);
        }
        [InitializeOnLoadMethod]
        #if !UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod]
        #endif
        static void AutoCreateOnDomainReload(){
            foreach(var (t,con) in GetSingletonTypes()){
                if(con.Contains(SingletonCreateCondition.ReloadDomain)){
                    t.BaseType.GetProperty("Instance",bindingFlags).GetValue(null);
                }
            }
        }    
        [InitializeOnLoadMethod]
        static void AutoCreateOnEnterPlayMode(){
            EditorApplication.playModeStateChanged+=e=>{
                if(e==PlayModeStateChange.EnteredPlayMode){
                    foreach(var (t,con) in GetSingletonTypes()){
                        if(con.Contains(SingletonCreateCondition.EnterPlay)){
                            t.BaseType.GetProperty("Instance",bindingFlags).GetValue(null);
                        }
                    }
                }
            };
        }

        [InitializeOnLoadMethod]
        static void CleanOnRecompileIfNeeded(){
            RemoveAllSingletons(inst=>inst.shouldDestroyOnReloadDomain());
        }

        [InitializeOnLoadMethod]
        static void CleanOnPlayModeChangeIfNeeded(){
            EditorApplication.playModeStateChanged+=e=>{
                if(e==PlayModeStateChange.ExitingPlayMode){
                    RemoveAllSingletons(inst=>inst.shouldDestroyOnExitPlayMode());
                }else if(e==PlayModeStateChange.ExitingEditMode){
                    RemoveAllSingletons(inst=>inst.shouldDestroyOnExitEditMode());
                }else if(e==PlayModeStateChange.EnteredPlayMode){
                    var instances = Resources
                        .FindObjectsOfTypeAll<SingletonBase>()
                        .ToArray();
                    foreach(var inst in instances){
                        inst.DestroyOnSceneUnload = inst.DestroyOnSceneUnload;
                    }
                }
            };
        }

        
        static bool shouldDestroyOnReloadDomain(this SingletonBase singleton){
            var t = singleton.GetType();
            var isAutoCreate = lut_singletonCreation.ContainsKey(t)
                && lut_singletonCreation[t].Contains(SingletonCreateCondition.ReloadDomain);
            return singleton.DestroyCondition.Contains(SingletonDestroyCondition.ReloadDomain)
                && !isAutoCreate;
        }
        static bool shouldDestroyOnExitPlayMode(this SingletonBase singleton)
            => singleton.DestroyCondition.Contains(SingletonDestroyCondition.ExitPlay);
        static bool shouldDestroyOnExitEditMode(this SingletonBase singleton)
            => singleton.DestroyCondition.Contains(SingletonDestroyCondition.ExitEdit);
            
        static BindingFlags bindingFlags = 0
            | BindingFlags.Static
            // | BindingFlags.Instance
            | BindingFlags.Public
            // | BindingFlags.NonPublic
            ;
    }
}