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
			for (int i = 0;i<3;i++)
			{
				var node1 = new Node(Network) { X = 0, Y = i * 100 };
				new InputConnector(node1);
				new InputConnector(node1);
				new OutputConnector(node1);
				new OutputConnector(node1);
			}
		}
		#endregion Constructor
	}
}