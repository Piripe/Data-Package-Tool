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
                source.Invites.AddRange(dest.Invites.Where(x=>!source.Invites.Contains(x)));
            }).ForAllMembers(opts=> opts.Condition((_,_,srcMember)=>srcMember!=null));
        }).CreateMapper();
    }
}
