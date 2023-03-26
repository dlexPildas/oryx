using System.ServiceModel.Channels;

namespace OryxDomain.WebServices
{
    public class CustomTextMessageEncoderFactory : MessageEncoderFactory
    {
        private MessageEncoder encoder;
        private MessageVersion version;
        private string mediaType;
        private string charSet;

        internal CustomTextMessageEncoderFactory(string mediaType, string charSet,
            MessageVersion version)
        {
            this.version = version;
            this.mediaType = mediaType;
            this.charSet = charSet;
            encoder = new CustomTextMessageEncoder(this);
        }

        public override MessageEncoder Encoder
        {
            get
            {
                return encoder;
            }
        }

        public override MessageVersion MessageVersion
        {
            get
            {
                return version;
            }
        }

        internal string MediaType
        {
            get
            {
                return mediaType;
            }
        }

        internal string CharSet
        {
            get
            {
                return charSet;
            }
        }
    }

}
