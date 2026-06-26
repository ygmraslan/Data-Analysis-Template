namespace DataAnalysis.Application.Features.Company.Dtos;

public sealed record StepDistributionDto(
    string Week,
    int Step,
    int PolicyCount
);