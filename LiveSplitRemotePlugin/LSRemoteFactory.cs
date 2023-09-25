using LiveSplit.Model;
using LiveSplit.UI.Components;
using LiveSplit.RemotePlugin;
using System;
using System.Reflection;

[assembly: ComponentFactory(typeof(LSRemoteFactory))]
namespace LiveSplit.RemotePlugin
{
	internal class LSRemoteFactory : IComponentFactory
	{
		public string ComponentName => "LiveSplit Remote";
		public string Description => "Connect to a Livesplit server and sync telemetry and events";
		public string UpdateName => ComponentName;

		public ComponentCategory Category => ComponentCategory.Control;
		public string UpdateURL => "";
		public string XMLURL => UpdateURL + "";

		public Version Version => Assembly.GetExecutingAssembly().GetName().Version;

		public IComponent Create(LiveSplitState state)
		{
			return new LSRemote(state);
		}
	}
}
