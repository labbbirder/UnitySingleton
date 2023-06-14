using System;
using System.Runtime.CompilerServices;

namespace com.bbbirder.unity{
    [AttributeUsage(AttributeTargets.Assembly|AttributeTargets.Class)]
    public class SingletonAutoLoadAttribute:Attribute{
        public Type type {get;private set;}
        public SingletonCreateCondition createCondition {get;private set;}
        public SingletonAutoLoadAttribute(Type type,SingletonCreateCondition createCondition=SingletonCreateCondition.ReloadDomain){
            this.type = type;
            this.createCondition = createCondition;
        }
    }
}