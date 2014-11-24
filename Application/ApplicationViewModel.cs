using NetworkVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

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
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j<2;j++)
				{
					var num = j * 3 + i;
					var node1 = new Node(Network) { X = j*200, Y = i * 100 };
					node1.Name = "Node " + num;
					new InputConnector(node1) { Name = "Input 1" };
					new InputConnector(node1) { Name = "Input 2" };
					new OutputConnector(node1) { Name = "Output 1" };
					new OutputConnector(node1) { Name = "Output 2" };
				}
			}
		}

		#endregion Constructor
	}
}