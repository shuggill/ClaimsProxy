using System;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;

namespace TestApp
{
    public class CookieBehavior : IEndpointBehavior
    {
        private string cookie;

        public CookieBehavior(string cookie)
        {
            this.cookie = cookie;
        }

        public void AddBindingParameters(ServiceEndpoint serviceEndpoint,
            System.ServiceModel.Channels
            .BindingParameterCollection bindingParameters) { }

        public void ApplyClientBehavior(ServiceEndpoint serviceEndpoint,
            System.ServiceModel.Dispatcher.ClientRuntime behavior)
        {
            behavior.MessageInspectors.Add(new CookieMessageInspector(cookie));
        }

        public void ApplyDispatchBehavior(ServiceEndpoint serviceEndpoint,
            System.ServiceModel.Dispatcher
            .EndpointDispatcher endpointDispatcher) { }

        public void Validate(ServiceEndpoint serviceEndpoint) { }
    }

    public class CookieMessageInspector : IClientMessageInspector
    {
        private string cookie;

        public CookieMessageInspector(string cookie)
        {
            this.cookie = cookie;
        }

        public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply,
            object correlationState) { }

        public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request,
            System.ServiceModel.IClientChannel channel)
        {
            HttpRequestMessageProperty httpRequestMessage;
            object httpRequestMessageObject;
            if (request.Properties.TryGetValue(HttpRequestMessageProperty.Name
                , out httpRequestMessageObject))
            {
                httpRequestMessage = httpRequestMessageObject
                    as HttpRequestMessageProperty;
                if (string.IsNullOrEmpty(httpRequestMessage.Headers["Cookie"]))
                {
                    httpRequestMessage.Headers["Cookie"] = cookie;
                }
            }
            else
            {
                httpRequestMessage = new HttpRequestMessageProperty();
                httpRequestMessage.Headers.Add("Cookie", cookie);
                request.Properties.Add(HttpRequestMessageProperty.Name
                    , httpRequestMessage);
            }

            return null;
        }
    }
}