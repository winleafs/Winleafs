using System;
using Winleafs.Api.DTOs;

namespace Winleafs.Api.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var nanoLeafClient = new NanoleafClient(new ClientDto("192.168.178.160", 16021));
            Console.WriteLine("Authorizing..");
            nanoLeafClient.AuthorizationEndpoint.GetAuthTokenAsync().GetAwaiter().GetResult();
            Console.WriteLine("Authorized!");
            Console.WriteLine("Getting effects...");
            var effects = nanoLeafClient.EffectsEndpoint.GetEffectsListAsync().GetAwaiter().GetResult();
            foreach (var effect in effects)
            {
                Console.WriteLine(effect);
            }

            while (true)
            {
                var effectName = Console.ReadLine();
                nanoLeafClient.EffectsEndpoint.SetSelectedEffectAsync(effectName).GetAwaiter().GetResult();
                Console.WriteLine("Set {0}", effectName);
            }
        }
    }
}
