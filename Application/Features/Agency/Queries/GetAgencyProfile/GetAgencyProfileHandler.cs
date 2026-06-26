using DataAnalysis.Application.Features.Agency.Abstractions;
using MediatR;

namespace DataAnalysis.Application.Features.Agency.Queries.GetAgencyProfile;

public sealed class GetAgencyProfileHandler(IAgencyRepository repository)
    : IRequestHandler<GetAgencyProfileQuery, GetAgencyProfileResponse>
{
    public async Task<GetAgencyProfileResponse> Handle(GetAgencyProfileQuery request, CancellationToken cancellationToken)
    {
        var profileTask = repository.GetProfileAsync(request.ProductGroup, request.AgencyCode, request.Filter, cancellationToken);
        var brandsTask  = repository.GetTopBrandsAsync(request.ProductGroup, request.AgencyCode, request.Filter, cancellationToken);

        await Task.WhenAll(profileTask, brandsTask);

        var profile = profileTask.Result;
        var brands  = brandsTask.Result;

        var aracYasi     = profile.Where(x => x.Category == "AracYasi").ToList();
        var basamak      = profile.Where(x => x.Category == "Basamak").ToList();
        var yenilemeTipi = profile.Where(x => x.Category == "YenilemeTipi").ToList();
        var sigortaliTuru= profile.Where(x => x.Category == "SigortaliTuru").ToList();

        var totalPolicy = profile.Sum(x => x.PolicyCount) / 4; // 4 kategori var, her birinde aynı toplam

        var brandTotal = brands.Sum(x => x.PolicyCount);

        return new GetAgencyProfileResponse
        {
            AracYasi = aracYasi.Select(x => new AgencyProfileItem
            {
                Type        = x.Type,
                PolicyCount = x.PolicyCount,
                NetPremium  = x.NetPremium,
                Ratio       = aracYasi.Sum(a => a.PolicyCount) > 0 
                    ? Math.Round(x.PolicyCount * 100m / aracYasi.Sum(a => a.PolicyCount), 1) 
                    : 0
            }).ToList(),

            Basamak = basamak.Select(x => new AgencyProfileItem
            {
                Type        = x.Type,
                PolicyCount = x.PolicyCount,
                NetPremium  = x.NetPremium,
                Ratio       = basamak.Sum(a => a.PolicyCount) > 0 
                    ? Math.Round(x.PolicyCount * 100m / basamak.Sum(a => a.PolicyCount), 1) 
                    : 0
            }).ToList(),

            YenilemeTipi = yenilemeTipi.Select(x => new AgencyProfileItem
            {
                Type        = x.Type,
                PolicyCount = x.PolicyCount,
                NetPremium  = x.NetPremium,
                Ratio       = yenilemeTipi.Sum(a => a.PolicyCount) > 0 
                    ? Math.Round(x.PolicyCount * 100m / yenilemeTipi.Sum(a => a.PolicyCount), 1) 
                    : 0
            }).ToList(),

            SigortaliTuru = sigortaliTuru.Select(x => new AgencyProfileItem
            {
                Type        = x.Type,
                PolicyCount = x.PolicyCount,
                NetPremium  = x.NetPremium,
                Ratio       = sigortaliTuru.Sum(a => a.PolicyCount) > 0 
                    ? Math.Round(x.PolicyCount * 100m / sigortaliTuru.Sum(a => a.PolicyCount), 1) 
                    : 0
            }).ToList(),

            TopBrands = brands.Select(x => new AgencyTopBrandItem
            {
                Brand       = x.Brand,
                PolicyCount = x.PolicyCount,
                NetPremium  = x.NetPremium,
                Ratio       = brandTotal > 0 
                    ? Math.Round(x.PolicyCount * 100m / brandTotal, 1) 
                    : 0
            }).ToList()
        };
    }
}