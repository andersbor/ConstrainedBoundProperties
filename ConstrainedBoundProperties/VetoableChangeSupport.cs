using System;
using System.Collections.Generic;

namespace ConstrainedBoundProperties
{
    public class VetoableChangeSupport
    {
        private readonly object _source;
        private readonly IList<IVetoableChangeListener> _listeners = new List<IVetoableChangeListener>();
        private readonly IList<Predicate<PropertyChangeEvent>> _listeners2 = new List<Predicate<PropertyChangeEvent>>();

        public VetoableChangeSupport(object sourceBean)
        {
            if (sourceBean == null)
            {
                throw new ArgumentNullException();
            }
            _source = sourceBean;
        }

        public void AddVetoableChangeListener(Predicate<PropertyChangeEvent> listener)
        {
            if (listener == null) throw new ArgumentNullException();
            _listeners2.Add(listener);
        }

        public void AddVetoableChangeListener(IVetoableChangeListener listener)
        {
            if (listener == null)
            {
                throw new ArgumentNullException();
            }
            _listeners.Add(listener);
        }

        public bool RemoveVetoableChangeListener(IVetoableChangeListener listener)
        {
            if (listener == null)
            {
                throw new ArgumentNullException();
            }
            return _listeners.Remove(listener);
        }

        public IEnumerable<IVetoableChangeListener> Listeners()
        {
            return _listeners;
        }

        public void FireVetoableChange(string propertyName, object oldValue, object newValue)
        {
            if (oldValue == newValue)
            {
                return;
            }
            PropertyChangeEvent ev = new PropertyChangeEvent(_source, propertyName, newValue, oldValue);
            foreach (IVetoableChangeListener vcl in _listeners)
            {
                vcl.VetoableChange(ev);
            }
        }

        public void FireVetoableChange2(string propertyName, object oldValue, object newValue)
        {
            if (oldValue == newValue)
            {
                return;
            }
            PropertyChangeEvent ev = new PropertyChangeEvent(_source, propertyName, newValue, oldValue);
            foreach (Predicate<PropertyChangeEvent> vcl in _listeners2)
            {
                if (!vcl.Invoke(ev)) { throw new PropertyVetoException(); }
            }
        }
    }

    public interface IVetoableChangeListener
    {
        void VetoableChange(PropertyChangeEvent ev);
    }

    public class PropertyChangeEvent
    {
        public PropertyChangeEvent(object source, string propertyName, object newValue, object oldValue)
        {
            NewValue = newValue;
            OldValue = oldValue;
        }

        public object NewValue { get; }

        public object OldValue { get; }
    }

    public class PropertyVetoException : Exception
    {

    }
}