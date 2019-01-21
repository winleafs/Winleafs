using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using Nanoleaf_Api.Endpoints.Interfaces;
using Nanoleaf_Models.Models.Effects;

namespace Nanoleaf_Api_Fake.Endpoints
{
    public class FakeEffectsEndpoint : IEffectsEndpoint
    {
        private readonly Faker _faker = new Faker();

        public Task<IEnumerable<string>> GetEffectsListAsync()
        {
            IEnumerable<string> result = _faker.Random.WordsArray(5, 50);
            return Task.FromResult(result);
        }

        public Task<string> GetSelectedEffectAsync()
        {
            return Task.FromResult(_faker.Random.Word());
        }

        public Task SetSelectedEffectAsync(string effectName)
        {
            if (_faker.Random.Number(50) < 5)
            {
                throw new Exception();
            }

            return Task.CompletedTask;
        }

        public Task<Effect> GetEffectDetailsAsync(string effectName)
        {
            return Task.FromResult(new Faker<Effect>()
                .RuleFor(x => x.AnimationType, f => f.Random.Word())
                .RuleFor(x => x.ColorType, f => f.Random.Word())
                .RuleFor(x => x.Direction, f => f.Random.Word())
                .RuleFor(x => x.IsOnLoop, f => f.Random.Bool())
                .RuleFor(x => x.ExplodeFactor, f => f.Random.Int())
                .RuleFor(x => x.Name, f => f.Random.Word())
                .Generate());
        }
    }
}
