using AutoMapper;
using DataPackageTool.Core.Models;
using System.Text.Json;

namespace DataPackageTool.Core
{
    public static class Shared
    {
        public static JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions();

        public static IMapper Mapper = new MapperConfiguration(cfg => {
            cfg.CreateMap<Guild, Guild>()
            .BeforeMap((source, dest) =>
            {
                source.Invites.AddRange(dest.Invites.Where(x => !source.Invites.Contains(x)));
                foreach (Channel channel in dest.Channels.Where(x => source.Channels.Any(y => y.Id == x.Id)))
                {
                    Channel sourceChannel = source.Channels.First(x=> x.Id == channel.Id);
                    Mapper!.Map(channel,sourceChannel);
                }
                source.Channels.AddRange(dest.Channels.Where(x => !source.Channels.Any(y=>y.Id==x.Id)));
            }).ForAllMembers(opts=> opts.Condition((_,_,srcMember)=>srcMember!=null));

        }).CreateMapper();
    }
}
