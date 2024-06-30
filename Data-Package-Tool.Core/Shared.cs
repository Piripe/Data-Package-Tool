using AutoMapper;
using DataPackageTool.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DataPackageTool.Core
{
    public static class Shared
    {
        public static JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions();

        public static IMapper Mapper = new MapperConfiguration(cfg => {
            cfg.CreateMap<Guild, Guild>()
            .BeforeMap((source, dest) =>
            {
                source.Invites.AddRange(dest.Invites);
            }).ForAllMembers(opts=> opts.Condition((_,_,srcMember)=>srcMember!=null));
        }).CreateMapper();
    }
}
