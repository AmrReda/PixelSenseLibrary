using System;

namespace PixelSenseLibrary.Events
{
    public class PropertyChangedEventArgs<T> : EventArgs
    {
        public PropertyChangedEventArgs(T oldValue, T newValue)
        {
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }

        public T NewValue { get; set; }

        public T OldValue { get; set; }

    }
}