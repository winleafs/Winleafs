using System.Threading.Tasks;
using Winleafs.Api;

namespace Winleafs.Wpf.Api.Effects
{
    public class TurnOffEffect : ICustomEffect
    {
        private readonly INanoleafClient _nanoleafClient;

        public TurnOffEffect(INanoleafClient nanoleafClient)
        {
            _nanoleafClient = nanoleafClient;
        }

        public Task Activate()
        {
            return _nanoleafClient.StateEndpoint.SetStateWithStateCheckAsync(false);
        }

        public async Task Deactivate()
        {
            //Do nothing since this is not a continious effect
        }

        public bool IsActive()
        {
            return false;
        }

        public bool IsContinuous()
        {
            return false;
        }
    }
}
