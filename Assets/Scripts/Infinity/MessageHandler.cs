using System;
using System.Collections.Generic;

namespace Infinity
{
    public delegate bool SubscribeCallback(Message m);

    public abstract class Message : IDisposable
    {
        public abstract void Dispose();
    }

    public struct MessageInfo
    {
        public SubscribeCallback Callback;

        public bool ReceiveOnlyOnce;

        public override bool Equals(object obj)
        {
            return obj is SubscribeCallback c && Equals(c);
        }

        public override int GetHashCode()
        {
            return Callback.GetHashCode();
        }
    }

    /// <summary>
    /// Super simple message handler
    /// </summary>
    public class MessageHandler
    {
        private readonly Dictionary<Type, List<MessageInfo>> messageInfoDict = new Dictionary<Type, List<MessageInfo>>();

        public void Subscribe<T>(SubscribeCallback c, bool once = false) where T : Message
        {
            var type = typeof(T);
            var messageInfo = new MessageInfo { Callback = c, ReceiveOnlyOnce = once };

            // Make new list and add it when there are no initial list for the given type
            if (!messageInfoDict.ContainsKey(type))
            {
                var list = new List<MessageInfo>();
                messageInfoDict.Add(type, list);
            }

            messageInfoDict[type].Add(messageInfo);
        }

        public void UnSubscribe<T>(SubscribeCallback c) where T : Message
        {
            var type = typeof(T);

            if (messageInfoDict.TryGetValue(type, out var infos))
            {
                var idx = infos.FindIndex(i => i.Callback.Equals(c));
                if (idx == -1) return;

                infos.RemoveAt(idx);
                if (infos.Count == 0)
                    messageInfoDict.Remove(type);
            }
        }

        public void PublishMessage<T>(T m) where T : Message
        {
            var type = typeof(T);
            var removeList = new List<MessageInfo>();

            if (!messageInfoDict.TryGetValue(type, out var infos)) return;

            foreach (var i in infos)
            {
                i.Callback.Invoke(m);

                if (i.ReceiveOnlyOnce)
                    removeList.Add(i);
            }

            foreach (var i in removeList)
                infos.Remove(i);

            if (infos.Count == 0)
                messageInfoDict.Remove(type);

            m.Dispose();
        }
    }
}