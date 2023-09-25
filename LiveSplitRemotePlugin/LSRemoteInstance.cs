using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Linq;

namespace LiveSplit.RemotePlugin
{
	public class LSRemoteInstance
	{
		DateTime start;
		TcpClient client;

		public static string AppDataRoamingPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\LiveSplitRemote";
		public static string ConfigurationFile = AppDataRoamingPath + @"\Config.xml";

		public string ServerStatus { get; set; }
		public string IPAddress { get; set; }
		public int Port { get; set; }
		public bool Autosplit { get; set; }
		
		public LSRemoteInstance()
		{
			start = DateTime.Now;
			CreateConfiguration();
			VerifyConfig();
			ReadConfiguration();
			ConnectToServer();
		}
		
		public string sendCommandString(string commandString, bool hasReturn)
		{
			if(client.Connected) {
				StreamWriter sw = new StreamWriter(client.GetStream());
				StreamReader sr = new StreamReader(client.GetStream());
				sw.WriteLine(commandString);
				sw.Flush();
				if(hasReturn) {
					return sr.ReadLine();
				}
			}
			
			return "";
		}

		public void CreateConfiguration()
		{
			if (!File.Exists(ConfigurationFile))
			{
				Directory.CreateDirectory(AppDataRoamingPath);
				XmlTextWriter Writer = new XmlTextWriter(ConfigurationFile, Encoding.UTF8);
				Writer.Formatting = Formatting.Indented;
				Writer.WriteStartElement("Configs");

				Writer.WriteStartElement("IPAddress");
				Writer.WriteString("0.0.0.0");
				Writer.WriteEndElement();

				Writer.WriteStartElement("Port");
				Writer.WriteString("16834");
				Writer.WriteEndElement();

				Writer.WriteStartElement("Autosplit");
				Writer.WriteString("True");
				Writer.WriteEndElement();

				Writer.WriteEndElement();
				Writer.Close();
			}
		}

		public void ReadConfiguration()
		{
			if (File.Exists(ConfigurationFile))
			{
				XDocument xml = XDocument.Load(ConfigurationFile);
				XElement ipElement = xml.Element("Configs").Element("IPAddress");
				IPAddress = ipElement.Value;
				XElement portElement = xml.Element("Configs").Element("Port");
				Port = int.Parse(portElement.Value);
				XElement autoSplitElement = xml.Element("Configs").Element("Autosplit");
				Autosplit = bool.Parse(autoSplitElement.Value);
			}
		}


		private void VerifyConfig()
		{
			XDocument xml = XDocument.Load(ConfigurationFile);
			XElement ipElement = xml.Element("Configs").Element("IPAddress");
			XElement portElement = xml.Element("Configs").Element("Port");
			XElement autoSplitElement = xml.Element("Configs").Element("Autosplit");
			if (ipElement == null)
			{
				xml.Descendants("Configs").FirstOrDefault().Add(new XElement("IPAddress","0.0.0.0"));
			}
			if (portElement == null)
			{
				xml.Descendants("Configs").FirstOrDefault().Add(new XElement("Port", "16834"));
			}
			if (autoSplitElement == null)
			{
				xml.Descendants("Configs").FirstOrDefault().Add(new XElement("Autosplit", "True"));
			}
			xml.Save(ConfigurationFile);
		}

		public void SaveConfiguration(string ip, string port, bool autoSplit)
		{
			XDocument xml = XDocument.Load(ConfigurationFile);
			xml.Element("Configs").Element("IPAddress").Value = ip;
			xml.Element("Configs").Element("Port").Value = port.ToString();
			xml.Element("Configs").Element("Autosplit").Value = autoSplit.ToString();
			xml.Save(ConfigurationFile);
			ReadConfiguration();
		}

		public async Task ConnectToServer()
		{
			try
			{
				if(client != null)
				{
					client.Close();
				}
				client = new TcpClient();
				await client.ConnectAsync(IPAddress, Port);
				ServerStatus = "Connected";
			}
			catch
			{
				ServerStatus = "Connection Error\nPlease check settings and restart LiveSplit server";
			}
		}

		public void Disconnect()
		{

		}
	}
}
