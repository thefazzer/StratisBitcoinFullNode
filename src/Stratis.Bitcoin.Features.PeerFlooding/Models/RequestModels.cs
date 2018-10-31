using Newtonsoft.Json;

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
            [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Wallet name is required.")]
            public string WalletName { get; set; }

            [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Wallet password is required.")]
            public string WalletPassword { get; set; }
        }
    }
}
