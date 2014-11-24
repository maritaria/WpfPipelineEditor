using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Utils
{
	public class UniqueObservableList<T> : INotifyCollectionChanged, IList<T>
	{
		#region Properties

		private List<T> m_List;

		#endregion Properties

		#region Constructor
		public UniqueObservableList()
		{
			m_List = new List<T>();
		}

		public UniqueObservableList(int capacity)
		{
			m_List = new List<T>(capacity);
		}

		public UniqueObservableList(IEnumerable<T> collection)
		{
			m_List = new List<T>();
			foreach (object o in collection)
			{
			}
		}

		#endregion Constructor

		#region INotifyCollectionChanged Members

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if (CollectionChanged != null)
			{
				CollectionChanged(this, e);
			}
		}

		#endregion INotifyCollectionChanged Members

		#region IList<T> Members

		public T this[int index]
		{
			get
			{
				return m_List[index];
			}
			set
			{
				RemoveAt(index);
				Insert(index, value);
			}
		}

		public int IndexOf(T item)
		{
			return m_List.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			if (!m_List.Contains(item))
			{
				m_List.Insert(index, item);
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
			}
		}

		public void RemoveAt(int index)
		{
			T value = m_List[index];
			m_List.RemoveAt(index);
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, index));
		}

		#endregion IList<T> Members

		#region ICollection<T> Members

		public int Count
		{
			get { return m_List.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public void Add(T item)
		{
			if (!m_List.Contains(item))
			{
				m_List.Add(item);
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
			}
		}

		public void Clear()
		{
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			m_List.Clear();
		}

		public bool Contains(T item)
		{
			return m_List.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			m_List.CopyTo(array, arrayIndex);
		}

		public bool Remove(T item)
		{
			bool result = m_List.Remove(item);
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
			return result;
		}

		#endregion ICollection<T> Members

		#region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator()
		{
			return m_List.GetEnumerator();
		}

		#endregion IEnumerable<T> Members

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion IEnumerable Members
	}
}