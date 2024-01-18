using LiveSplit.Model;
using LiveSplit.RemotePlugin;
using LiveSplit.UI.Components;
using System;

[assembly: ComponentFactory(typeof(LSRemoteFactory))]
namespace LiveSplit.RemotePlugin
{
    internal class LSRemoteFactory : IComponentFactory
    {
        public string ComponentName => "LiveSplit Remote";
        public string Description => "Connect to a Livesplit server and sync telemetry and events";
        public string UpdateName => this.ComponentName;

        public ComponentCategory Category => ComponentCategory.Control;
        public string UpdateURL => "";
        public string XMLURL => this.UpdateURL + "";

        public Version Version => Version.Parse("1.0.2");

        public IComponent Create(LiveSplitState state) => new LSRemote(state);
    }
}
