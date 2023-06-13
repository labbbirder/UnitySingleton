using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using com.bbbirder.unity;

class SingeltonManager{
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
    [InitializeOnLoadMethod]
    static void CleanOnRecompileIfNeeded(){
        RemoveAllSingletons(inst=>inst.DestroyOnReloadDomain);
    }

    [InitializeOnLoadMethod]
    static void CleanOnPlayModeChangeIfNeeded(){
        EditorApplication.playModeStateChanged+=e=>{
            if(e==PlayModeStateChange.ExitingPlayMode){
                RemoveAllSingletons(inst=>inst.DestroyOnExitPlayMode);
            }else if(e==PlayModeStateChange.ExitingEditMode){
                RemoveAllSingletons(inst=>inst.DestroyOnExitEditMode);
            }else if(e==PlayModeStateChange.EnteredPlayMode){
                var instances = Resources
                    .FindObjectsOfTypeAll<SingletonBase>()
                    .ToArray();
                foreach(var inst in instances){
                    inst.DestroyOnSceneUnload ^= false;
                }
            }
        };
    }
}