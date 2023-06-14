namespace com.bbbirder.unity{
    public enum SingletonDestroyCondition{
        
        /// <summary>
        /// Dont destroy automatically.
        /// </summary>
        Never = 0,
        /// <summary>
        /// Destroy on scene unload.
        /// </summary>
        SceneUnload = 1<<0,
        /// <summary>
        /// Destroy on script recompilation. [Default]
        /// </summary>
        ReloadDomain = 1<<1,
        /// <summary>
        /// Destroy on application mode switchs from play mode to editor mode.
        /// </summary>
        ExitPlay = 1<<2,
        /// <summary>
        /// Destroy on application mode switchs from editor mode to play mode.
        /// </summary>
        ExitEdit = 1<<3,
    }
    public enum SingletonCreateCondition{
        /// <summary>
        /// Dont instantiate automatically.
        /// </summary>
        LazyLoad = 0,
        /// <summary>
        /// Instantiate on domain reloads. More exactly, editor mode means script compiled mostly, while runtime mode means application launched.
        /// </summary>
        ReloadDomain = 1<<0,
        /// <summary>
        /// Instantiate on enter play mode. Editor only.
        /// </summary>
        EnterPlay = 1<<1,
    }
    public static class Extensions{
        public static bool Contains(this SingletonCreateCondition con,SingletonCreateCondition con2)
            => (con & con2)!=0;
        public static bool Contains(this SingletonDestroyCondition con,SingletonDestroyCondition con2)
            => (con & con2)!=0;
    }
}