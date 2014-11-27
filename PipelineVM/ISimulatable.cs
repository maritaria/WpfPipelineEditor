using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelineVM
{
	/// <summary>
	/// Defines the required interface for an object to be able to participate in the simulation
	/// </summary>
	public interface ISimulatable
	{
		SimulationStatus Status { get; }

		void Start(bool forcePrepare);
		void Stop();
		void Pause();
		void Prepare();
		bool IsPrepared { get; }
		
		event EventHandler StatusChanged;
	}

	public enum SimulationStatus : int
	{
		Unknown,
		Preparing,
		Running,
		Pausing,
		Paused,
		Stopping,
		Stopped,
	}
}