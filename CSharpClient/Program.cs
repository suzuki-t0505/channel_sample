using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Phoenix;
using PhoenixTests.WebSocketImpl;

using System.Threading;
using System.Threading.Tasks;

namespace CSharpClient {
    class Program
    {
        public static void Main(string[] args)
        {
            var socketOptions = new Socket.Options(new JsonMessageSerializer());
            var socketAddress = "ws://localhost:4000/socket";
            var socketFactory = new WebsocketSharpFactory();
            var socket = new Socket(socketAddress, null, socketFactory, socketOptions);

            var OnOpenCount = 0;
            void OnOpenCallback()
            {
              OnOpenCount++;
            }

            List<string> onCloseData = new();
            void OnCloseCallback(ushort code, string message)
            {
              onCloseData.Add(message);
            }

            socket.OnOpen += OnOpenCallback;
            socket.OnClose += OnCloseCallback;

            Dictionary<string, object> channelParams = new Dictionary<string, object>(){{"body", "Hello World(C#)"}};
            Dictionary<string, object> pushParams = new Dictionary<string, object>(){{"body", "New Message C#!"}};

            Message? message = null;
            Message? errorMessage = null;

            Reply? joinOkReply = null;
            Reply? joinErrorReply = null;
            Reply? pushOkReply = null;


            socket.Connect();

            var roomChannel = socket.Channel("room:lobby", channelParams);

            // errorイベントの受信
            roomChannel.On(Message.InBoundEvent.Error, m => errorMessage = m);

            // イベントの受信
            roomChannel.On("new_msg", m => {
                message = m;
                var payload = message?.Payload.Unbox<JObject>();
                Console.WriteLine($"[{message?.Topic}] [{message?.Event}] {payload["body"].ToObject<string>()}");
            });

            // チャネルに参加
            roomChannel.Join()
                .Receive(ReplyStatus.Ok, r => joinErrorReply = r)
                .Receive(ReplyStatus.Error, r => joinErrorReply = r);

            roomChannel.Push("new_msg", pushParams).Receive(ReplyStatus.Ok, r => pushOkReply = r);

            Task<string> disconnect = Disconnect(socket, roomChannel);

            Console.WriteLine(disconnect.Result);
        }

        static async Task<string> Disconnect(Socket socket, Channel channel)
        {
          Console.WriteLine("ソケットとの接続を切断します。");
          Dictionary<string, object> _params = new Dictionary<string, object>(){{"body", "ルームから抜ける"}};
          channel.Push("new_msg", _params).Receive(ReplyStatus.Ok, r => Console.WriteLine("メッセージを送信しました。"));
          await Task.Delay(5000);
          socket.Disconnect();
          return "ソケットとの接続を切断しました。";
        }
    }
}