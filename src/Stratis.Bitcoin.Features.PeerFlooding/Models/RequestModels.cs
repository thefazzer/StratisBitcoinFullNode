using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Stratis.Bitcoin.Features.Wallet.Controllers;
using Stratis.Bitcoin.Features.Wallet.Validations;
using Stratis.Bitcoin.Utilities.ValidationAttributes;

namespace Stratis.Bitcoin.Features.PeerFlooding.Models
{
    public class RequestModels
    {
        public class RequestModel
        {
            public override string ToString()
            {
                return JsonConvert.SerializeObject(this, Formatting.Indented);
            }
        }

        public class PeerFloodingRequest : RequestModel
        {
            //public string  { get; set; }
            //public string  { get; set; }
        }
    }
}
