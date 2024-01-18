using LiveSplit.UI;
using System;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.RemotePlugin
{
    public partial class LSRemoteSettings : UserControl
    {
        private readonly LSRemote remote;

        public string ServerIP { get; set; }

        public ushort Port { get; set; }

        public string PortString
        {
            get => this.Port.ToString();
            set => this.Port = ushort.Parse(value);
        }

        public bool EnableAutosplit { get; set; }

        public LSRemoteSettings(LSRemote remote)
        {
            this.InitializeComponent();
            this.remote = remote;

            this.ServerIP = "127.0.0.1";
            this.Port = 16834;
            this.lblServerStatus.Text = "";

            this.txtIPAddress.DataBindings.Add("Text", this, "ServerIP", false, DataSourceUpdateMode.OnPropertyChanged);
            this.txtPort.DataBindings.Add("Text", this, "PortString", false, DataSourceUpdateMode.OnPropertyChanged);
        }

        private int CreateSettingsNode(XmlDocument document, XmlElement parent)
        {
            return SettingsHelper.CreateSetting(document, parent, "ServerIP", this.ServerIP) ^
                SettingsHelper.CreateSetting(document, parent, "Port", this.PortString) ^
                SettingsHelper.CreateSetting(document, parent, "EnableAutosplit", this.EnableAutosplit);
        }

        public XmlNode GetSettings(XmlDocument document)
        {
            XmlElement parent = document.CreateElement("Settings");
            this.CreateSettingsNode(document, parent);
            return parent;
        }

        public int GetSettingsHashCode() => this.CreateSettingsNode(null, null);

        public void SetSettings(XmlNode settings)
        {
            this.ServerIP = SettingsHelper.ParseString(settings["ServerIP"]);

            this.PortString = SettingsHelper.ParseString(settings["Port"]);

            this.EnableAutosplit = SettingsHelper.ParseBool(settings["EnableAutosplit"]);

            // Set checkbox checked based on the setting
            this.cbAutosplit.Checked = this.EnableAutosplit;
        }

        public void SetStatusText(string statusText) => this.lblServerStatus.Text = statusText;

        private async void btnConnect_Click(object sender, EventArgs e) => await this.remote.instance.ConnectToServer();

        private void cbAutosplit_CheckedChanged(object sender, EventArgs e) => this.EnableAutosplit = this.cbAutosplit.Checked;

        private void LSRemoteSettings_Load(object sender, EventArgs e) => this.lblServerStatus.Text = this.remote.instance.ServerStatus;
    }
}
