using LiveSplit.Model;
using LiveSplit.TimeFormatters;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Xml;

namespace LiveSplit.RemotePlugin
{
	public class LSRemote : LogicComponent
	{
		public override string ComponentName => "LiveSplit Remote";
		private LiveSplitState state;
		private LSRemoteSettings settings;
		private LSRemoteInstance instance;
		protected ITimerModel Model { get; set; }
		protected ITimeFormatter SplitTimeFormatter { get; set; }
		public string IGT { get; set; }
		public int SplitIndex { get; set; }
		DispatcherTimer t;

		public LSRemote(LiveSplitState state)
		{
			Model = new TimerModel() { CurrentState = state };
			instance = new LSRemoteInstance();
			settings = new LSRemoteSettings(instance);
			SplitTimeFormatter = new RegularTimeFormatter(TimeAccuracy.Hundredths);

			this.state = state;
			this.state.IsGameTimePaused = true;
			state.OnStart += (sender, e) => { instance.sendCommandString("starttimer", false); };
			state.OnPause += (sender, e) => { instance.sendCommandString("pause", false); };
			state.OnResume += (sender, e) => { instance.sendCommandString("resume", false); };
			state.OnReset += (sender, e) => { instance.sendCommandString("reset", false); };

			if (! instance.Autosplit)
			{
				state.OnSplit += (sender, e) => { instance.sendCommandString("split", false); };
				state.OnUndoSplit += (sender, e) => { instance.sendCommandString("unsplit", false); };
				state.OnSkipSplit += (sender, e) => { instance.sendCommandString("skipsplit", false); };
			}

			t = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 10), DispatcherPriority.Background, t_Tick, Dispatcher.CurrentDispatcher);
			t.IsEnabled = true;
		}

		private void t_Tick(object sender, EventArgs e)
		{
			var timingMethod = state.CurrentTimingMethod;
		    if (timingMethod == TimingMethod.GameTime && !state.IsGameTimeInitialized) {
				timingMethod = TimingMethod.RealTime;
			}
            
			if (state.CurrentTime[timingMethod].HasValue)
			{
				IGT = SplitTimeFormatter.Format(state.CurrentTime[timingMethod]);
			}
		}

		public override void Dispose()
		{
			t.IsEnabled = false;
			instance.Disconnect();
		}

		public override XmlNode GetSettings(XmlDocument document)
		{
			return settings.GetSettings(document);
		}

		public override System.Windows.Forms.Control GetSettingsControl(LayoutMode mode)
		{
			return settings;
		}

		public override void SetSettings(XmlNode settings)
		{
			this.settings.SetSettings(settings);
		}

		public override void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
		{
			instance.sendCommandString("setgametime " + IGT, false);
			if (instance.Autosplit)
			{
				int RemoteSplitIndex = int.Parse(instance.sendCommandString("getsplitindex",true));
				int difference = state.CurrentSplitIndex - RemoteSplitIndex;
				while(difference > 1)
				{
					instance.sendCommandString("skipsplit",false);
					difference--;
				}
				while(difference < 0)
				{
					instance.sendCommandString("unsplit",false);
					difference++;
				}
				if(difference == 1)
				{
					instance.sendCommandString("split",false);
				}
			}	
		}
	}
}
