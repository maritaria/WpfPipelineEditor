using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using NetworkViewModel;
namespace EditorApplication
{
	public class ApplicationViewModel : ViewModelBase
	{
		#region Properties
		private Network m_Network = new Network();
		public Network Network
		{
			get
			{
				return m_Network;
			}
			set
			{
				OnPropertyChanging("Network");
				m_Network = value;
				OnPropertyChanged("Network");
			}
		}

		#endregion Properties

		#region Constructor
		public ApplicationViewModel()
		{
			var node1 = new Node(Network);
			new InputConnector(node1);
			new InputConnector(node1);
			new OutputConnector(node1);
			new OutputConnector(node1);
			var node2 = new Node(Network) { Y = 100 };
			new InputConnector(node2);
			new InputConnector(node2);
			new OutputConnector(node2);
			new OutputConnector(node2);
		}
		#endregion Constructor
	}
}