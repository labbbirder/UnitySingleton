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

        [MenuItem("Window/bbbirder/Singletons")]
        public static void ShowWindow()
        {
            var window = GetWindow<SingletonWindow>();
            window.titleContent = new GUIContent("Singletons");
            window.Show();
        }
        private void CreateGUI()
        {
            var uiRootAsset    = GetVisualTreeAssetByGUID(uiRootGUID);
            var uiElementAsset = GetVisualTreeAssetByGUID(uiElementGUID);
            uiRootAsset.CloneTree(rootVisualElement);
            var lst = rootVisualElement.Q<ListView>("lst");
            (lst.makeItem,lst.bindItem) = BindItem();
            rootVisualElement.schedule.Execute(UpdateSource).Every(SOURCE_UPDATE_INTERVAL);
            (Func<VisualElement>,Action<VisualElement,int>) BindItem(){
                // GameObject    gameObject = null;
                // Label         txtName    = null;
                // VisualElement btnInspect = null;
                // VisualElement btnDestroy = null;
                // MaskField     mask       = null;
                return (OnCreate,OnBind);
                VisualElement OnCreate(){
                    var ele = uiElementAsset.CloneTree();
                    var txtName    = ele.Q<Label>("txtName");
                    var btnInspect = ele.Q("btnInspect");
                    var btnDestroy = ele.Q("btnDestroy");
                    var mask       = ele.Q<MaskField>("mask");
                    mask.choices = Enum.GetNames(typeof(HideFlags))[1..^2].ToList();
                    mask.RegisterValueChangedCallback(e=>{
                        GetGameObject().hideFlags = (HideFlags)e.newValue;
                    });
                    btnDestroy.RegisterCallback<ClickEvent>(e=>{
                        GameObject.DestroyImmediate(GetGameObject());
                    });
                    btnInspect.RegisterCallback<ClickEvent>(e=>{
                        Selection.activeGameObject = GetGameObject();
                        // EditorUtility.OpenPropertyEditor(go);
                    });
                    txtName.RegisterCallback<ClickEvent>(e=>{
                        Selection.activeGameObject = GetGameObject();
                    });
                    return ele;
                    GameObject GetGameObject()=>ele.userData as GameObject;
                }
                void OnBind(VisualElement ele,int idx){
                    var txtName    = ele.Q<Label>("txtName");
                    var mask       = ele.Q<MaskField>("mask");
                    var gameObject = lst.itemsSource[idx] as GameObject;
                    txtName.text = gameObject.name;
                    mask.value = (int)gameObject.hideFlags;
                    ele.userData = gameObject;
                    // Debug.Log(gameObject.name+gameObject.hideFlags);
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
