// Copyright 2016-2017 Datalust Pty Ltd
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Newtonsoft.Json;
using Seq.Forwarder.Util;

// ReSharper disable UnusedMember.Global, AutoPropertyCanBeMadeGetOnly.Global

namespace Seq.Forwarder.Config
{
    public class SeqForwarderOutputConfig
    {
        public string ServerUrl { get; set; } = "http://localhost:5341";
        public ulong EventBodyLimitBytes { get; set; } = 256 * 1024;
        public ulong RawPayloadLimitBytes { get; set; } = 10 * 1024 * 1024;

#if WINDOWS
        const string ProtectedDataPrefix = "pd.";

        [JsonProperty("apiKey")]
        public string? EncodedApiKey { get; set; }

        [JsonIgnore]
        public string? ApiKey
        {
            get
            {
                if (string.IsNullOrWhiteSpace(EncodedApiKey))
                    return null;

                if (!EncodedApiKey.StartsWith(ProtectedDataPrefix))
                    return EncodedApiKey;

                return MachineScopeDataProtection.Unprotect(EncodedApiKey.Substring(ProtectedDataPrefix.Length));
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    EncodedApiKey = null;
                    return;
                }

                EncodedApiKey = $"{ProtectedDataPrefix}{MachineScopeDataProtection.Protect(value)}";
            }
        }
    }
#endif
}
