using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Debug_Server
{
    class AsyncServer
    {
        private byte[] _buffer = new byte[1024];
        private List<Socket> _clientSockets = new List<Socket>();
        private Socket _serverSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
        private static readonly string server_username = "ohm";
        private static readonly string server_password = "741895623ohm";
        private static readonly string server_host = "35.231.112.9";
        private static readonly string server_port = "27019";
        private static readonly string database_name = "cool_db";
        private static IMongoClient _mongoClient = new MongoClient("mongodb://" + server_username + ":" + server_password + "@" + server_host + ":" + server_port + "/" + database_name);
        private static IMongoDatabase _database = _mongoClient.GetDatabase("cool_db");
        private static IMongoCollection<BsonDocument> _debugs = _database.GetCollection<BsonDocument>(typeof(Debug).Name);
        private static Debug _current;
        public void SetupServer()
        {
            Console.WriteLine("Starting Server...");
            _serverSocket.Bind(new IPEndPoint(IPAddress.Any, 4242));
            _serverSocket.Listen(5);
            _serverSocket.BeginAccept(AcceptCallback, null);
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            Socket socket = _serverSocket.EndAccept(ar);
            _clientSockets.Add(socket);
            Console.WriteLine("Client Connected");
            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceiveCallback, socket);
            _serverSocket.BeginAccept(AcceptCallback, null);
        }

        private async void ReceiveCallback(IAsyncResult ar)
        {
            Socket socket = (Socket) ar.AsyncState;
            int received = socket.EndReceive(ar);
            byte[] dataBuf = new byte[received];
            Array.Copy(_buffer, dataBuf, received);

            string id = Encoding.ASCII.GetString(dataBuf);
            Console.WriteLine("Text received: "+id);
            _current = await GetDebug(id);
            _current = Debugger.StartProcess(_current);
            await Update();
            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceiveCallback, socket);
        }

        private void SendCallback(IAsyncResult ar)
        {
            Socket socket = (Socket) ar.AsyncState;
            socket.EndSend(ar);
        }

        private void SendText(Socket socket,string text)
        {
            byte[] data = Encoding.ASCII.GetBytes(text);
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, SendCallback, socket);
        }
        private static async Task<Debug> GetDebug(string id)
        {
            var filter = new BsonDocument { { "_id", id } };
            using (var cursor = await _debugs.FindAsync(filter))
            {
                return JsonConvert.DeserializeObject<Debug>(cursor.First().ToString());
            }

        }
        private static async Task<bool> Update()
        {
            try
            {
                var filter = new BsonDocument { { "_id", _current._id } };
                var json = JsonConvert.SerializeObject(_current);
                await _debugs.ReplaceOneAsync(filter, BsonDocument.Parse(json));

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
