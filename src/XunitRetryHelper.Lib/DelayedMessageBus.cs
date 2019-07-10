namespace XunitRetryHelper.Lib
{
    using System.Collections;
    using System.Collections.Generic;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class DelayedMessageBus : IMessageBus
    {
        private readonly IMessageBus innerBus;
        private readonly IList<IMessageSinkMessage> messages = new List<IMessageSinkMessage>();
        private IList<IMessageSinkMessage> failedMessages;

        public DelayedMessageBus(IMessageBus innerBus)
        {
            this.innerBus = innerBus;
        }

        public bool QueueMessage(IMessageSinkMessage message)
        {
            lock (this.messages)
            {
                this.messages.Add(message);
            }

            return true;
        }

        public void StartRun()
        {
            lock (this.messages)
            {
                if (this.messages.Count > 0)
                {
                    var target = this.failedMessages ?? (this.failedMessages = new List<IMessageSinkMessage>());
                    for (var i = 0; i < this.messages.Count; i++)
                    {
                        var msg = this.messages[i] as TestFailed;
                        if (msg != null)
                        {
                            target.Add(msg);
                        }
                    }
                    this.messages.Clear();
                }
            }
        }

        public void Complete()
        {
            if (this.failedMessages != null)
            {
                for (var i = 0; i < this.failedMessages.Count; i++)
                {
                    this.innerBus.QueueMessage(this.failedMessages[i]);
                }
            }

            for (var i = 0; i < this.messages.Count; i++)
            {
                this.innerBus.QueueMessage(this.messages[i]);
            }

            this.messages.Clear();
        }

        public void Dispose()
        {
            this.messages.Clear();
        }
    }
}
