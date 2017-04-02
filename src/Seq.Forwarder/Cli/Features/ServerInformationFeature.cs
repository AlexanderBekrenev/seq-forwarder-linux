﻿// Copyright 2016-2017 Datalust Pty Ltd
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

namespace Seq.Forwarder.Cli.Features
{
    class ServerInformationFeature : CommandFeature
    {
        public ServerInformationFeature()
        {
            Url = "";
            ApiKey = "";
        }

        public bool IsUrlSpecified => !string.IsNullOrEmpty(Url);
        public bool IsApiKeySpecified => !string.IsNullOrEmpty(ApiKey);

        public string Url { get; set; }
        public string ApiKey { get; set; }

        public override void Enable(OptionSet options)
        {
            options.Add("u=|url=",
                "The URL of the Seq server to import logs into; by default the URL in SeqForwarder.json will  be used",
                v => Url = v.Trim());

            options.Add("a=|apikey=",
                "The API key to use when connecting to the server; by default the API key in SeqForwarder.json will be used",
                v => ApiKey = v.Trim());
        }
    }
}