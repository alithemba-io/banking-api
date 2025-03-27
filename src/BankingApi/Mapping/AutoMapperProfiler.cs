using AutoMapper;
using BankingApi.DTOs;  
using BankingApi.Models;

namespace BankingApi.Mapping;

public class AutoMapperProfiler : Profile{

    public AutoMapperProfiler(){
        CreateMap<BankAccount, BankAccountDto>()
        .ForMember(dest => dest.AccountType, opt => opt.MapFrom(src => src.AccountType.ToString()))
        .ForMember(dest => dest.Status,  opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<Withdrawal, WithdrawalResponseDto>()
        .ForMember(dest => dest.NewBalance, opt => opt.Ignore());    
    }
}