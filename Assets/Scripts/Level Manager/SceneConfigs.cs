using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Utilities
{
    public class SceneConfigs : MonoBehaviour
    {
        public static SceneConfigs Singleton;
        public List<SceneReference> scenes = new();
        private void Awake()
        {
            Singleton = this;
        }
    }
    public static class SceneConfigExtensions
    {
        public static SceneAsset GetReference(this SceneEnum sceneID)
        {
            return SceneConfigs.Singleton.scenes.Find(x => x.sceneId == sceneID).target;
        }

        public static SceneAsset GetSceneAsset(this string givenName)
        {
            return SceneConfigs.Singleton.scenes.Find(x => x.target.name == givenName).target;
        }
    }

    [Serializable]
    public class SceneReference
    {
        public SceneEnum sceneId;
        public SceneAsset target;
    }
    public enum SceneEnum
    {
        Scene1_Interior,
        Scene1_Outside,
        Scene2,
        Scene3_Village,
        Scene4,
        Scene5,
        Scene5_Chest,
        Scene6,
        Scene7,
        Scene8,
        Scene8_Chest,
        Scene9,
        Scene10,
        Scene11,
        Scene12,
        Scene13,
        Scene14,
        Scene15,
        Scene16,
        Scene17,
    }
}