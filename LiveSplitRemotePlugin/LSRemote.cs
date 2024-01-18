using LiveSplit.Model;
using LiveSplit.TimeFormatters;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using System;
using System.Windows.Threading;
using System.Xml;

namespace LiveSplit.RemotePlugin
{
    public class LSRemote : LogicComponent
    {
        public override string ComponentName => "LiveSplit Remote";
        private readonly LiveSplitState state;
        public readonly LSRemoteSettings settings;
        public readonly LSRemoteInstance instance;
        protected ITimerModel Model { get; set; }
        protected ITimeFormatter SplitTimeFormatter { get; set; }
        public string IGT { get; set; }
        public int SplitIndex { get; set; }

        private readonly DispatcherTimer t;

        public LSRemote(LiveSplitState state)
        {
            this.Model = new TimerModel() { CurrentState = state };
            this.instance = new LSRemoteInstance(this);
            this.settings = new LSRemoteSettings(this);
            this.SplitTimeFormatter = new RegularTimeFormatter(TimeAccuracy.Hundredths);

            this.state = state;
            this.state.IsGameTimePaused = true;
            state.OnStart += (sender, e) => this.instance.sendCommandString("starttimer", false);
            state.OnPause += (sender, e) => this.instance.sendCommandString("pause", false);
            state.OnResume += (sender, e) => this.instance.sendCommandString("resume", false);
            state.OnReset += (sender, e) => this.instance.sendCommandString("reset", false);

            state.OnSplit += (sender, e) => this.instance.sendCommandString("split", false);
            state.OnUndoSplit += (sender, e) => this.instance.sendCommandString("unsplit", false);
            state.OnSkipSplit += (sender, e) => this.instance.sendCommandString("skipsplit", false);

            this.t = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 10), DispatcherPriority.Background, this.t_Tick, Dispatcher.CurrentDispatcher)
            {
                IsEnabled = true
            };
        }

        private void t_Tick(object sender, EventArgs e)
        {
            TimingMethod timingMethod = this.state.CurrentTimingMethod;
            if (timingMethod == TimingMethod.GameTime && !this.state.IsGameTimeInitialized)
            {
                timingMethod = TimingMethod.RealTime;
            }

            if (this.state.CurrentTime[timingMethod].HasValue)
            {
                this.IGT = this.SplitTimeFormatter.Format(this.state.CurrentTime[timingMethod]);
            }
        }

        public override void Dispose()
        {
            this.t.IsEnabled = false;
            this.instance.Disconnect();
        }

        public override XmlNode GetSettings(XmlDocument document) => this.settings.GetSettings(document);

        public int GetSettingsHashCode => this.settings.GetHashCode();

        public override System.Windows.Forms.Control GetSettingsControl(LayoutMode mode) => this.settings;

        public override void SetSettings(XmlNode settings) => this.settings.SetSettings(settings);

        public override void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            this.instance.sendCommandString("setgametime " + this.IGT, false);
            if (this.settings.EnableAutosplit)
            {
                int RemoteSplitIndex = int.Parse(this.instance.sendCommandString("getsplitindex", true));
                int difference = state.CurrentSplitIndex - RemoteSplitIndex;
                while (difference > 1)
                {
                    this.instance.sendCommandString("skipsplit", false);
                    difference--;
                }
                while (difference < 0)
                {
                    this.instance.sendCommandString("unsplit", false);
                    difference++;
                }
                if (difference == 1)
                {
                    this.instance.sendCommandString("split", false);
                }
            }
        }
    }
}
