using System;

namespace ConstrainedBoundProperties
{
    class Program
    {
        static void Main()
        {
            //UsingNameBean();
            AccountBean account = new AccountBean();
            account.AddBusinessRule(rule => (int)rule.NewValue >= 0);
            account.Balance = 1;
            Console.WriteLine(account.Balance);
            account.Balance = 0;
            // account.Balance = -1;
            Predicate<PropertyChangeEvent> withDrawLimit = rule =>
                (int)rule.OldValue - (int)rule.NewValue <= 100;
            account.AddBusinessRule(withDrawLimit);
            account.Balance = 1000;
            account.Balance = 900;
            account.Balance = 799;
        }

        private static void UsingNameBean()
        {
            NameBean bean = new NameBean();
            bean.Name = "Anders";
            bean.Name = "Y";
            Console.WriteLine(bean.Name);
            bean.Name = "Yi";
            //IVetoableChangeListener vcl = new NameRule();
            //bean.AddVetoableChangeListener(vcl);
            bean.AddVetoableChangeListener(
                ev => (string)ev.NewValue != null && ((string)ev.NewValue).Length >= 2);
            bean.Name = "Bo";
            try
            {
                bean.Name = "A";
            }
            catch (PropertyVetoException ex)
            {
                Console.WriteLine("An exception was thrown " + ex.Message);
            }
        }
    }

    public class NameBean
    {
        private readonly VetoableChangeSupport _vcs;
        private string _name;

        public string Name
        {
            get => _name;
            set
            {
                _vcs.FireVetoableChange2("name", _name, newValue: value);
                _name = value;
            }
        }

        public NameBean()
        {
            _vcs = new VetoableChangeSupport(this);
        }

        public void AddVetoableChangeListener(IVetoableChangeListener vcl)
        {
            _vcs.AddVetoableChangeListener(vcl);
        }

        public void AddVetoableChangeListener(Predicate<PropertyChangeEvent> pred)
        {
            _vcs.AddVetoableChangeListener(pred);
        }
    }

    class NameRule : IVetoableChangeListener
    {
        public void VetoableChange(PropertyChangeEvent ev)
        {
            string newName = (string)ev.NewValue;
            if (newName == null || newName.Length < 2)
            {
                throw new PropertyVetoException();
            }
        }
    }
}