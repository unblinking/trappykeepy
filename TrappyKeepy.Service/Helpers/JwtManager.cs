using System.Text.Json;
using EasyEncrypt2;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Service
{

    public enum JwtType
    {
        NONE = 0,
        ACTIVATION = 1,
        ACCESS = 2
    }

    public class JwtPayload
    {
        public Guid userId { get; set; }
        public UserRole role { get; set; }
        public JwtType type { get; set; }
        public DateTime issued { get; set; }
    }

    public class JwtManager
    {
        private string secret;
        private string key;

        public JwtManager()
        {
            this.secret = $"{Environment.GetEnvironmentVariable("TK_JWT_SECRET")}";
            this.key = $"{Environment.GetEnvironmentVariable("TK_CRYPTO_KEY")}";
        }

        public string EncodeJwt(Guid userId, UserRole role, JwtType type)
        {
            // Verify that the required secret, key, and user id are set.
            if (string.IsNullOrWhiteSpace(this.secret))
            {
                throw new Exception("JWT secret is not defined, so a JWT cannot be created.");
            }
            if (string.IsNullOrWhiteSpace(this.key))
            {
                throw new Exception("Cryptography key is not defined, so a JWT cannot be created.");
            }
            if (userId == Guid.Empty)
            {
                throw new Exception("User id is not defined, so a JWT cannot be created.");
            }

            // Setup the encrypter
            var easy = new EasyEncrypt(key: System.Text.Encoding.Default.GetBytes(this.key));

            // Create the payload.
            var payload = new JwtPayload()
            {
                userId = userId,
                role = role,
                type = type,
                issued = DateTime.UtcNow
            };

            // Serialize the payload.
            var payloadSerialized = JsonSerializer.Serialize(payload);

            // Encrypt the payload.
            var payloadEncrypted = easy.Encrypt(payloadSerialized);

            var payloadWrapper = new Dictionary<string, object>
            {
                { "payload", payloadEncrypted }
            };

            // Setup the encoder.
            var algorithm = new HMACSHA256Algorithm(); // symmetric
            var serializer = new JsonNetSerializer();
            var urlEncoder = new JwtBase64UrlEncoder();
            var encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            // Encode the JWT.
            var jwt = encoder.Encode(payloadWrapper, this.secret);

            return jwt;
        }

        public JwtPayload DecodeJwt(string jwt)
        {
            // Verify that the required secret, key, and jwt are defined.
            if (string.IsNullOrWhiteSpace(this.secret))
            {
                throw new Exception("JWT secret is not defined, so the JWT cannot be read.");
            }
            if (string.IsNullOrWhiteSpace(this.key))
            {
                throw new Exception("Cryptography key is not defined, so the JWT cannot be read.");
            }
            if (string.IsNullOrWhiteSpace(jwt))
            {
                throw new Exception("JWT is not defined, so it cannot be read.");
            }

            // Setup the decoder.
            var serializer = new JsonNetSerializer();
            var provider = new UtcDateTimeProvider();
            var validator = new JwtValidator(serializer, provider);
            var urlEncoder = new JwtBase64UrlEncoder();
            var algorithm = new HMACSHA256Algorithm(); // symmetric
            var decoder = new JwtDecoder(serializer, validator, urlEncoder, algorithm);

            // Decode the JWT.
            var payloadWrapper = decoder.Decode(jwt, this.secret, verify: true);

            // Deserialize the payload.
            var payloadWrapperDeserialized = JsonSerializer.Deserialize<Dictionary<string, object>>(payloadWrapper);
            if (payloadWrapperDeserialized is null)
            {
                throw new Exception("Error deserializing the JWT payload wrapper");
            }

            // Setup the decrypter.
            var easy = new EasyEncrypt(key: System.Text.Encoding.Default.GetBytes(this.key));

            // Decrypt the payload.
            var payloadDecrypted = easy.Decrypt(payloadWrapperDeserialized["payload"].ToString());

            // Deserialize the decrypted payload.
            var payloadDeserialized = JsonSerializer.Deserialize<JwtPayload>(payloadDecrypted);

            if (payloadDeserialized is null)
            {
                throw new Exception("Failure reading JWT.");
            }

            return payloadDeserialized;
        }
    }
}
