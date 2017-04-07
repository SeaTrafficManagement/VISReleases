using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace STM.StmModule.Simulator.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged<TProperty>(Expression<Func<TProperty>> projection)
        {
            var memberExpression = (MemberExpression)projection.Body;
            OnPropertyChangedExplicit(memberExpression.Member.Name);
        }

        void OnPropertyChangedExplicit(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        private bool _busy = false;
        public bool Busy
        {
            get
            {
                return _busy;
            }
            set
            {
                if (_busy == value)
                    return;

                _busy = value;

                OnPropertyChanged(() => Busy);
            }
        }

        private string _busyContent = "";
        public string BusyContent
        {
            get
            {
                return _busyContent;
            }
            set
            {
                if (_busyContent == value)
                    return;

                _busyContent = value;

                OnPropertyChanged(() => BusyContent);
            }
        }

    }
}
