using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Linq;
using System;
using UnityEngine.Assertions;

namespace com.bbbirder.unity{
    public class SingletonWindow : EditorWindow
    {
        const int SOURCE_UPDATE_INTERVAL = 100;
        const string uiRootGUID    = "72ffbc6d3ac74ec4991e3a339804d8d5";
        const string uiElementGUID = "5ff6281642adb5c48a87d0bac8b6c70c";
        bool inited = false;
        [MenuItem("Window/bbbirder/Singletons")]
        public static void ShowWindow()
        {
            var window = GetWindow<SingletonWindow>();
            window.titleContent = new GUIContent("Singletons");
            window.Show();
            window.CreateGUI();//avoid Unity bug: https://forum.unity.com/threads/creategui-not-being-called.1111852/
        }

        public void CreateGUI()
        {
            if(inited) return;
            var uiRootAsset    = GetVisualTreeAssetByGUID(uiRootGUID);
            var uiElementAsset = GetVisualTreeAssetByGUID(uiElementGUID);
            uiRootAsset.CloneTree(rootVisualElement);
            var lst = rootVisualElement.Q<ListView>("lst");
            // lst.makeItem = uiElementAsset.CloneTree;
            // lst.bindItem = (ve,i)=>{
            //     var go = lst.itemsSource[i] as GameObject;
            //     var txtName    = ve.Q<Label>("txtName");
            //     var btnInspect = ve.Q("btnInspect");
            //     var btnDestroy = ve.Q("btnDestroy");
            //     var mask       = ve.Q<MaskField>("mask");
            //     txtName.text = go.name;
            //     mask.choices = Enum.GetNames(typeof(HideFlags))[1..^2].ToList();
            //     mask.value = (int)go.hideFlags;
            //     btnDestroy.RegisterCallback<ClickEvent>(e=>{
            //         GameObject.DestroyImmediate(go);
            //     });
            //     btnInspect.RegisterCallback<ClickEvent>(e=>{
            //         Selection.activeGameObject = go;
            //         // EditorUtility.OpenPropertyEditor(go);
            //     });
            //     txtName.RegisterCallback<ClickEvent>(e=>{
            //         Selection.activeGameObject = go;
            //     });
            // };
            (lst.makeItem,lst.bindItem) = BindItem();
            rootVisualElement.schedule.Execute(UpdateSource).Every(SOURCE_UPDATE_INTERVAL);
            (Func<VisualElement>,Action<VisualElement,int>) BindItem(){
                GameObject    gameObject = null;
                Label         txtName    = null;
                VisualElement btnInspect = null;
                VisualElement btnDestroy = null;
                MaskField     mask       = null;
                return (OnCreate,OnBind);
                VisualElement OnCreate(){
                    var ele = uiElementAsset.CloneTree();
                    txtName    = ele.Q<Label>(nameof(txtName));
                    btnInspect = ele.Q(nameof(btnInspect));
                    btnDestroy = ele.Q(nameof(btnDestroy));
                    mask       = ele.Q<MaskField>(nameof(mask));
                    mask.choices = Enum.GetNames(typeof(HideFlags))[1..^2].ToList();
                    mask.RegisterValueChangedCallback(e=>{
                        gameObject.hideFlags = (HideFlags)e.newValue;
                    });
                    btnDestroy.RegisterCallback<ClickEvent>(e=>{
                        GameObject.DestroyImmediate(gameObject);
                    });
                    btnInspect.RegisterCallback<ClickEvent>(e=>{
                        Selection.activeGameObject = gameObject;
                        // EditorUtility.OpenPropertyEditor(go);
                    });
                    txtName.RegisterCallback<ClickEvent>(e=>{
                        Selection.activeGameObject = gameObject;
                    });
                    return ele;
                }
                void OnBind(VisualElement _,int idx){
                    gameObject = lst.itemsSource[idx] as GameObject;
                    txtName.text = gameObject.name;
                    mask.value = (int)gameObject.hideFlags;
                }
            }
            void UpdateSource(){
                lst.itemsSource = Resources.FindObjectsOfTypeAll<SingletonBase>()
                    .Select(comp=>comp.gameObject)
                    .Distinct()
                    .ToList();
            }
        }
        VisualTreeAsset GetVisualTreeAssetByGUID(string guid)=>
            AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetDatabase.GUIDToAssetPath(guid));
    }
}
