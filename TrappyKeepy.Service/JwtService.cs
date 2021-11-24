using EasyEncrypt2;
using SimpleJWT;

namespace TrappyKeepy.Service
{

    public enum JwtTokenType
    {
        NONE = 0,
        ACTIVATION = 1,
        ACCESS = 2
    }

    public class JwtService
    {
        private string secret;
        private string key;

        public JwtService()
        {
            this.secret = $"{Environment.GetEnvironmentVariable("TK_JWT_SECRET")}";
            this.key = $"{Environment.GetEnvironmentVariable("TK_CRYPTO_KEY")}";
        }

        public string EncodeJwt(Guid id, JwtTokenType type)
        {
            if (string.IsNullOrWhiteSpace(this.secret))
            {
                throw new Exception("JWT secret is not defined.");
            }
            if (string.IsNullOrWhiteSpace(this.key))
            {
                throw new Exception("Cryptography key is not defined.");
            }

            var encrypter = new EasyEncrypt(key: System.Text.Encoding.Default.GetBytes(this.key));
            var payload = new Dictionary<string, object>
            {
                { "id", encrypter.Encrypt(id.ToString()) },
                { "type", encrypter.Encrypt(type.ToString()) },
                { "issued", encrypter.Encrypt(DateTime.Now.ToString()) },
                { "expires", encrypter.Encrypt(DateTime.Now.AddHours(24).ToString()) }
            };

            var jwtEncoder = new JwtEncoder();
            var jwt = jwtEncoder.Encode(payload, this.secret);

            return jwt;
        }

        public Dictionary<string, object> DecodeJwt(string jwt)
        {
            if (string.IsNullOrWhiteSpace(this.secret))
            {
                throw new Exception("JWT secret is not defined.");
            }

            var jwtDecoder = new JwtDecoder();
            var decodedPayload = jwtDecoder.Decode(jwt, this.secret);

            var encrypter = new EasyEncrypt(key: System.Text.Encoding.Default.GetBytes(this.key));
            var decryptedPayload = new Dictionary<string, object>
            {
                { "id", encrypter.Decrypt(decodedPayload["id"].ToString()) },
                { "type", encrypter.Decrypt(decodedPayload["type"].ToString()) },
                { "issued", encrypter.Decrypt(decodedPayload["issued"].ToString()) },
                { "expires", encrypter.Encrypt(decodedPayload["expires"].ToString()) }
            };

            return decryptedPayload;
        }
    }
}