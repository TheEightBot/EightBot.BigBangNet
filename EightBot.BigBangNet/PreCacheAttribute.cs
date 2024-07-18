using System;
namespace EightBot.BigBang
{
    [AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true)]
    public class PreCacheAttribute : Attribute
    {
        public PreCacheAttribute()
        {
        }
    }
}
