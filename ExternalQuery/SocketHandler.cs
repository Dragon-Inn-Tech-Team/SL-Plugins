using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PluginAPI.Core;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ExternalQuery
{
	public class SocketHandler
	{
		public TcpListener TcpListener;

		public SocketHandler()
		{
			TcpListener = new TcpListener(IPAddress.Any, Plugin.Config.Port);
			TcpListener.Start();

			new Thread(() =>
			{
				listen();
			}).Start();
		}

		public void listen()
		{
			while (true)
			{
				if (TcpListener.Pending())
				{
					var client = TcpListener.AcceptTcpClient();

					Log.Info($"Connection from {client.Client.RemoteEndPoint} opened");

					NetworkStream stream = client.GetStream();

					try
					{
						string data = string.Empty;

						byte[] bytes = new byte[1024];

						stream.Read(bytes, 0, bytes.Length);

						data = UTF8Encoding.UTF8.GetString(bytes, 0, bytes.Length);

						if (string.IsNullOrEmpty(data))
						{
							Log.Info($"Remote query from {client.Client.RemoteEndPoint} rejected due to invalid request format");
							send(stream, JsonConvert.SerializeObject(new JObject(new JProperty("response", "Invalid format"))));
						}
						else
						{
							try
							{
								JObject obj = JObject.Parse(data);

								if (!obj.ContainsKey("password") || obj["password"].ToString() != Plugin.Config.Password)
								{
									Log.Info($"Remote query from {client.Client.RemoteEndPoint} rejected due to invalid password");
									send(stream, JsonConvert.SerializeObject(new JObject(new JProperty("response", "Invalid Query Password"))));
								}
								else if (!obj.ContainsKey("command"))
								{
									Log.Info($"Remote query from {client.Client.RemoteEndPoint} rejected due to invalid request format");
									send(stream, JsonConvert.SerializeObject(new JObject(new JProperty("response", "Invalid format"))));
								}
								else
								{
									Log.Info($"Remote query from {client.Client.RemoteEndPoint} executed command {obj["command"]}");

									var cmdStr = obj["command"].ToString().ToLower();

									var cmd = cmdStr.Split(' ')[0];
									cmd = cmd.Replace("/", string.Empty);

									if (!cmdStr.StartsWith("/"))
										cmdStr = $"/{cmdStr}";

									switch (cmd)
									{
										case "ban":
										case "rban":
										case "remoteban":
											{
												var response = CustomCommandHandler.BanCommand(obj["command"].ToString());
												send(stream, JsonConvert.SerializeObject(new JObject(new JProperty("response", response))));
												break;
											}
										case "kick":
										case "rkick":
										case "remotekick":
											{
												var response = CustomCommandHandler.KickCommand(obj["command"].ToString());
												send(stream, JsonConvert.SerializeObject(new JObject(new JProperty("response", response))));
												break;
											}
										case "unban":
										case "runban":
										case "remoteunban":
											{
												var response = CustomCommandHandler.UnbanCommand(obj["command"].ToString());
												send(stream, JsonConvert.SerializeObject(new JObject(new JProperty("response", response))));
												break;
											}
										case "forceclass":
											{
												var response = $"Command '{cmdStr}' cannot be executed remotely";
												send(stream, JsonConvert.SerializeObject(new JObject(new JProperty("response", response))));
												break;
											}
										default:
											{
												var response = Server.RunCommand(cmdStr, new ServerConsoleSender());
												send(stream, JsonConvert.SerializeObject(new JObject(new JProperty("response", response))));
												break;
											}

									}
								}
							}
							catch (Exception e)
							{
								Log.Info($"Error executing remote command: {e}\nData: {data}");
							}
						}

						Log.Info($"Connection from {client.Client.RemoteEndPoint} closed");

						stream.Close();
						client.Close();
						client.Dispose();
					}
					catch (Exception e)
					{
						Log.Info($"Error executing remote command: {e}");
						send(stream, JsonConvert.SerializeObject(new JObject(new JProperty("response", e))));

						stream.Close();
						client.Close();
						client.Dispose();
					}
				}
				Thread.Sleep(50);
			}
		}

		public void send(NetworkStream stream, string content)
		{
			byte[] bytes = UTF8Encoding.UTF8.GetBytes(content);
			stream.Write(bytes, 0, bytes.Length);
			Log.Info($"Command output sent to remote query source");
		}
	}
}
