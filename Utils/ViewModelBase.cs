using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
	public abstract class ViewModelBase : INotifyPropertyChanged, INotifyPropertyChanging
	{
		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string name)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}

		#endregion INotifyPropertyChanged Members

		#region INotifyPropertyChanging Members

		public event PropertyChangingEventHandler PropertyChanging;

		protected virtual void OnPropertyChanging(string name)
		{
			if (PropertyChanging != null)
			{
				PropertyChanging(this, new PropertyChangingEventArgs(name));
			}
		}

		#endregion INotifyPropertyChanging Members
	}
}