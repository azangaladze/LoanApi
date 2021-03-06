using AutoMapper;
using LoanProject.Core.Entities;
using LoanProject.Infrastructure.Models;

namespace LoanProject.Api.Helpers
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<UserModel, User>();
            CreateMap<LoanModel, Loan>();

        }
    }
}
