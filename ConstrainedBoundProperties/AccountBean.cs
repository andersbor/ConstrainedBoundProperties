using System;

namespace ConstrainedBoundProperties
{
    public class AccountBean
    {
        private readonly VetoableChangeSupport _vcs;
        private int _balance;
        private string _name;

        public int Balance
        {
            get => _balance;
            set
            {
                _vcs.FireVetoableChange2("balance", _balance, value);
                _balance = value;
            }
        }

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public AccountBean() : this(0) { }

        public AccountBean(int initialBalance)
        {
            _vcs = new VetoableChangeSupport(this);
            Balance = initialBalance;
        }

        public void Deposit(int amount)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException();
            Balance = Balance + amount;
        }

        public void Withdraw(int amount)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException();
            Balance = Balance - amount;
        }

        public void AddBusinessRule(Predicate<PropertyChangeEvent> rule)
        {
            _vcs.AddVetoableChangeListener(rule);
        }

    }
}
