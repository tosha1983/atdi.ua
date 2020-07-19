using Atdi.Common.Extensions;
using Atdi.Contracts.CoreServices.Identity;
using Atdi.DataModels.Identity;
using Atdi.Platform.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Atdi.CoreServices.Identity
{
    
    public sealed class  UserTokenProvider : LoggedObject,  IUserTokenProvider
    {
	    private const string SecretKey = "A699967B-B579-4C11-9121-C0AB8BC50290";

	    public UserTokenProvider(ILogger logger) : base(logger)
        {
        }


        public UserToken CreatUserToken(UserTokenData tokenData)
        {
            if (tokenData == null)
            {
                throw new ArgumentNullException(nameof(tokenData));
            }

            try
            {
	            var rawData = tokenData.Serialize();
	            var encryptedData = Encryptor.EncryptStringAES(Convert.ToBase64String(rawData), SecretKey);

				var token = new UserToken
                {
                    Data = Encoding.Unicode.GetBytes(encryptedData)
                };

                return token;
            }
            catch (Exception e)
            {
                this.Logger.Exception(Contexts.IdentityCoreServices, Categories.Handling, e, this);
                throw new InvalidOperationException("Failed to pack the user token", e);
            }
        }

        public string GetHashPassword(string password)
        {
            var passwordAsBytes = UTF8Encoding.UTF8.GetBytes(password);
            var shaProvider = new SHA256CryptoServiceProvider();
            var hash = shaProvider.ComputeHash(passwordAsBytes);

            var sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }

        public UserTokenData UnpackUserToken(UserToken userToken)
        {
            if (userToken == null)
            {
                throw new ArgumentNullException(nameof(userToken));
            }

            if (userToken.Data == null)
            {
                throw new ArgumentNullException(nameof(userToken.Data));
            }

            try
            {
	            var encodeData = Encoding.Unicode.GetString(userToken.Data);
	            var plainTextData = Encryptor.DecryptStringAES(encodeData, SecretKey);
	            var rawData = Convert.FromBase64String(plainTextData);
	            return rawData.Deserialize<UserTokenData>();
            }
            catch(Exception e)
            {
                this.Logger.Exception(Contexts.IdentityCoreServices, Categories.Handling, e, this);
                throw new InvalidOperationException("Failed to unpack the user token", e);
            }
            
        }
    }
}
