using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STM.SSC.Internal
{
    public class Token
    {

        [JsonProperty("access_token")]
        private String accessToken;

        [JsonProperty("expires_in")]
        private long expiresIn;

        [JsonProperty("refresh_expires_in")]
        private long refreshExpiresIn;

        [JsonProperty("refresh_token")]
        private String refreshToken;

        [JsonProperty("token_type")]
        private String tokenType;

        [JsonProperty("id_token")]
        private String idToken;

        [JsonProperty("not-before-policy")]
        private long notBeforePolicy;

        [JsonProperty("session_state")]
        private String sessionState;

        public String AccessToken
        {
            get
            {
                return this.accessToken;
            }
            set
            {
                this.accessToken = value;
            }
        }

        public long ExpiresIn
        {
            get
            {
                return this.expiresIn;
            }
            set
            {
                this.expiresIn = value;
            }
        }

        public long RefreshExpiresIn
        {
            get
            {
                return this.refreshExpiresIn;
            }
            set
            {
                this.refreshExpiresIn = value;
            }
        }

        public string RefreshToken
        {
            get
            {
                return this.refreshToken;
            }
            set
            {
                this.refreshToken = value;
            }
        }
        public string TokenType
        {
            get
            {
                return this.tokenType;
            }
            set
            {
                this.tokenType = value;
            }
        }
        public string IdToken
        {
            get
            {
                return this.idToken;
            }
            set
            {
                this.idToken = value;
            }
        }
        public long NotBeforePolicy
        {
            get
            {
                return this.notBeforePolicy;
            }
            set
            {
                this.notBeforePolicy = value;
            }
        }
        public string SessionState
        {
            get
            {
                return this.sessionState;
            }
            set
            {
                this.sessionState = value;
            }
        }
    }
}
