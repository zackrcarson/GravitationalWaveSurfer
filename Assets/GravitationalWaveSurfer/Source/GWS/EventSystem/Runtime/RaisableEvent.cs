using System;

namespace GWS.EventSystem.Runtime
{
    /// <summary>
    /// An event that can be externally invoked.
    /// </summary>
    public struct RaisableEvent
    {
        /// <summary>
        /// The event. 
        /// </summary>
        public event Action OnEvent;

        /// <summary>
        /// Raises the <see cref="OnEvent"/>.
        /// </summary>
        public void RaiseEvent() => OnEvent?.Invoke();
    }
    
    /// <inheritdoc cref="RaisableEvent"/>
    public struct RaisableEvent<T>
    {
        /// <inheritdoc cref="RaisableEvent.OnEvent"/>
        public event Action<T> OnEvent;
        /// <inheritdoc cref="RaisableEvent.RaiseEvent"/>
        public void RaiseEvent(T obj) => OnEvent?.Invoke(obj);
    }

    /// <inheritdoc cref="RaisableEvent"/>
    public struct RaisableEvent<T1, T2>
    {
        /// <inheritdoc cref="RaisableEvent.OnEvent"/>
        public event Action<T1, T2> OnEvent;
        /// <inheritdoc cref="RaisableEvent.RaiseEvent"/>
        public void RaiseEvent(T1 arg1, T2 arg2) => OnEvent?.Invoke(arg1, arg2);
    }

    /// <inheritdoc cref="RaisableEvent"/>
    public struct RaisableEvent<T1, T2, T3>
    {
        /// <inheritdoc cref="RaisableEvent.OnEvent"/>
        public event Action<T1, T2, T3> OnEvent;
        /// <inheritdoc cref="RaisableEvent.RaiseEvent"/>
        public void RaiseEvent(T1 arg1, T2 arg2, T3 arg3) => OnEvent?.Invoke(arg1, arg2, arg3);
    }
    
    /// <inheritdoc cref="RaisableEvent"/>
    public struct RaisableEvent<T1, T2, T3, T4>
    {
        /// <inheritdoc cref="RaisableEvent.OnEvent"/>
        public event Action<T1, T2, T3, T4> OnEvent;
        /// <inheritdoc cref="RaisableEvent.RaiseEvent"/>
        public void RaiseEvent(T1 arg1, T2 arg2, T3 arg3, T4 arg4) => OnEvent?.Invoke(arg1, arg2, arg3, arg4);
    }
    
    /// <inheritdoc cref="RaisableEvent"/>
    public struct RaisableEvent<T1, T2, T3, T4, T5>
    {
        /// <inheritdoc cref="RaisableEvent.OnEvent"/>
        public event Action<T1, T2, T3, T4, T5> OnEvent;
        /// <inheritdoc cref="RaisableEvent.RaiseEvent"/>
        public void RaiseEvent(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => OnEvent?.Invoke(arg1, arg2, arg3, arg4, arg5);
    }
    
    /// <inheritdoc cref="RaisableEvent"/>
    public struct RaisableEvent<T1, T2, T3, T4, T5, T6>
    {
        /// <inheritdoc cref="RaisableEvent.OnEvent"/>
        public event Action<T1, T2, T3, T4, T5, T6> OnEvent;
        /// <inheritdoc cref="RaisableEvent.RaiseEvent"/>
        public void RaiseEvent(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) => OnEvent?.Invoke(arg1, arg2, arg3, arg4, arg5, arg6);
    }
    
    /// <inheritdoc cref="RaisableEvent"/>
    public struct RaisableEvent<T1, T2, T3, T4, T5, T6, T7>
    {
        /// <inheritdoc cref="RaisableEvent.OnEvent"/>
        public event Action<T1, T2, T3, T4, T5, T6, T7> OnEvent;
        /// <inheritdoc cref="RaisableEvent.RaiseEvent"/>
        public void RaiseEvent(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) => OnEvent?.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
    }

    /// <inheritdoc cref="RaisableEvent"/>
    public struct RaisableEvent<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        /// <inheritdoc cref="RaisableEvent.OnEvent"/>
        public event Action<T1, T2, T3, T4, T5, T6, T7, T8> OnEvent;
        /// <inheritdoc cref="RaisableEvent.RaiseEvent"/>
        public void RaiseEvent(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8) => OnEvent?.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
    }

    /// <inheritdoc cref="RaisableEvent"/>
    public struct RaisableEvent<T1, T2, T3, T4, T5, T6, T7, T8, T9>
    {
        /// <inheritdoc cref="RaisableEvent.OnEvent"/>
        public event Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> OnEvent;
        /// <inheritdoc cref="RaisableEvent.RaiseEvent"/>
        public void RaiseEvent(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9) => OnEvent?.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
    }

    /// <inheritdoc cref="RaisableEvent"/>
    public struct RaisableEvent<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
    {
        /// <inheritdoc cref="RaisableEvent.OnEvent"/>
        public event Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> OnEvent;
        /// <inheritdoc cref="RaisableEvent.RaiseEvent"/>
        public void RaiseEvent(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10) => OnEvent?.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
    }

    /// <inheritdoc cref="RaisableEvent"/>
    public struct RaisableEvent<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
    {
        /// <inheritdoc cref="RaisableEvent.OnEvent"/>
        public event Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> OnEvent;
        /// <inheritdoc cref="RaisableEvent.RaiseEvent"/>
        public void RaiseEvent(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11) => OnEvent?.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
    }

    /// <inheritdoc cref="RaisableEvent"/>
    public struct RaisableEvent<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>
    {
        /// <inheritdoc cref="RaisableEvent.OnEvent"/>
        public event Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> OnEvent;
        /// <inheritdoc cref="RaisableEvent.RaiseEvent"/>
        public void RaiseEvent(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12) => OnEvent?.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
    }

    /// <inheritdoc cref="RaisableEvent"/>
    public struct RaisableEvent<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>
    {
        /// <inheritdoc cref="RaisableEvent.OnEvent"/>
        public event Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> OnEvent;
        /// <inheritdoc cref="RaisableEvent.RaiseEvent"/>
        public void RaiseEvent(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13) => OnEvent?.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
    }

    /// <inheritdoc cref="RaisableEvent"/>
    public struct RaisableEvent<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>
    {
        /// <inheritdoc cref="RaisableEvent.OnEvent"/>
        public event Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> OnEvent;
        /// <inheritdoc cref="RaisableEvent.RaiseEvent"/>
        public void RaiseEvent(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14) => OnEvent?.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
    }

    /// <inheritdoc cref="RaisableEvent"/>
    public struct RaisableEvent<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>
    {
        /// <inheritdoc cref="RaisableEvent.OnEvent"/>
        public event Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> OnEvent;
        /// <inheritdoc cref="RaisableEvent.RaiseEvent"/>
        public void RaiseEvent(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15) => OnEvent?.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
    }

    /// <inheritdoc cref="RaisableEvent"/>
    public struct RaisableEvent<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>
    {
        /// <inheritdoc cref="RaisableEvent.OnEvent"/>
        public event Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> OnEvent;
        /// <inheritdoc cref="RaisableEvent.RaiseEvent"/>
        public void RaiseEvent(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16) => OnEvent?.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);
    }
}