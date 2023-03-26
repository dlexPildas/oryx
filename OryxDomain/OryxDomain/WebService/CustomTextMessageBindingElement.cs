using System;
using System.ServiceModel.Channels;
using System.Xml;

namespace OryxDomain.WebServices
{
    public class CustomTextMessageBindingElement : MessageEncodingBindingElement
    {
        private MessageVersion msgVersion;
        private string mediaType;
        private string encoding;
        private XmlDictionaryReaderQuotas readerQuotas;

        CustomTextMessageBindingElement(CustomTextMessageBindingElement binding)
            : this(binding.Encoding, binding.MediaType, binding.MessageVersion)
        {
            readerQuotas = new XmlDictionaryReaderQuotas();
            binding.ReaderQuotas.CopyTo(readerQuotas);
        }

        public CustomTextMessageBindingElement(string encoding, string mediaType,
            MessageVersion msgVersion)
        {
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            if (mediaType == null)
                throw new ArgumentNullException("mediaType");

            if (msgVersion == null)
                throw new ArgumentNullException("msgVersion");

            this.msgVersion = msgVersion;
            this.mediaType = mediaType;
            this.encoding = encoding;
            readerQuotas = new XmlDictionaryReaderQuotas();
        }

        public CustomTextMessageBindingElement(string encoding, string mediaType)
            : this(encoding, mediaType, MessageVersion.Soap12WSAddressing10)
        {
        }

        public CustomTextMessageBindingElement(string encoding)
            : this(encoding, "text/xml")
        {

        }

        public CustomTextMessageBindingElement()
            : this("UTF-8")
        {
        }

        public override MessageVersion MessageVersion
        {
            get
            {
                return msgVersion;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                msgVersion = value;
            }
        }


        public string MediaType
        {
            get
            {
                return mediaType;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                mediaType = value;
            }
        }

        public string Encoding
        {
            get
            {
                return encoding;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                encoding = value;
            }
        }

        // This encoder does not enforces any quotas for the unsecure messages. The  
        // quotas are enforced for the secure portions of messages when this encoder 
        // is used in a binding that is configured with security.  
        public XmlDictionaryReaderQuotas ReaderQuotas
        {
            get
            {
                return readerQuotas;
            }
        }

        #region IMessageEncodingBindingElement Members 

        public override MessageEncoderFactory CreateMessageEncoderFactory()
        {
            return new CustomTextMessageEncoderFactory(MediaType,
                Encoding, MessageVersion);
        }

        #endregion


        public override BindingElement Clone()
        {
            return new CustomTextMessageBindingElement(this);
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            context.BindingParameters.Add(this);
            return context.BuildInnerChannelFactory<TChannel>();
        }

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            return context.CanBuildInnerChannelFactory<TChannel>();
        }



        public override T GetProperty<T>(BindingContext context)
        {
            if (typeof(T) == typeof(XmlDictionaryReaderQuotas))
            {
                return (T)(object)readerQuotas;
            }
            else
            {
                return base.GetProperty<T>(context);
            }
        }
    }
}
