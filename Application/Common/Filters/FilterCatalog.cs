using DataAnalysis.Application.Common.Enums;

namespace DataAnalysis.Application.Common.Filters;

public static class FilterCatalog
{
    public static string ToDbValue(this InsuredType v) => v switch
    {
        InsuredType.GERCEK => "GERCEK",
        InsuredType.TUZEL  => "TUZEL",
        _ => throw new ArgumentOutOfRangeException(nameof(v))
    };

    public static string ToDbValue(this BusinessSource v) => v switch
    {
        BusinessSource.YeniIs   => "Yeni İş",
        BusinessSource.Yenileme => "Yenileme",
        BusinessSource.Transfer => "Transfer",
        _ => throw new ArgumentOutOfRangeException(nameof(v))
    };

    public static string ToDbValue(this VehicleType v) => v switch
    {
        VehicleType.Otomobil        => "Otomobil",
        VehicleType.Kamyonet        => "Kamyonet",
        VehicleType.Motosiklet      => "Motosiklet ve Yük Motosikleti",
        VehicleType.Minibus         => "Minibüs (10 - 17)",
        VehicleType.Traktor         => "Traktör",
        VehicleType.Kamyon          => "Kamyon",
        VehicleType.Cekici          => "Çekici",
        VehicleType.KucukOtobus     => "K.Otobüs (18 - 30)",
        VehicleType.OzelAmacliTasit => "Özel Amaçlı Taşıt",
        VehicleType.BuyukOtobus     => "Otobus(31+)",
        VehicleType.Taksi           => "Taksi",
        VehicleType.IsMakinesi      => "İş Makinesi",
        VehicleType.Romork          => "Römork",
        VehicleType.Tanker          => "Tanker",
        _ => throw new ArgumentOutOfRangeException(nameof(v))
    };

    public static string ToLabel(this InsuredType v) => v switch
    {
        InsuredType.GERCEK => "Gerçek Kişi",
        InsuredType.TUZEL  => "Tüzel Kişi",
        _ => v.ToString()
    };

    public static string ToLabel(this BusinessSource v) => v switch
    {
        BusinessSource.YeniIs   => "Yeni İş",
        BusinessSource.Yenileme => "Yenileme",
        BusinessSource.Transfer => "Transfer",
        _ => v.ToString()
    };

    public static string ToLabel(this VehicleType v) => v.ToDbValue();

    public sealed record ProductItem(string Code, string DbValue, string Label);

    public static readonly IReadOnlyDictionary<ProductGroup, IReadOnlyList<ProductItem>> Products =
        new Dictionary<ProductGroup, IReadOnlyList<ProductItem>>
        {
            [ProductGroup.KASKO] = new List<ProductItem>
            {
                //KASKO
            },
            [ProductGroup.TRAFIK] = new List<ProductItem>
            {
                //TRAFIK
            },
        };

    public static bool TryGetProductDbValue(ProductGroup group, string code, out string dbValue)
    {
        dbValue = "";
        return Products.TryGetValue(group, out var list)
            && (dbValue = list.FirstOrDefault(x => x.Code == code)?.DbValue ?? "") != "";
    }

    public static FilterOptionsResponse GetOptions() => new()
    {
        InsuredType    = Enum.GetValues<InsuredType>().Select(v => new FilterOptionItem(v.ToString(), v.ToLabel())).ToList(),
        BusinessSource = Enum.GetValues<BusinessSource>().Select(v => new FilterOptionItem(v.ToString(), v.ToLabel())).ToList(),
        VehicleType    = Enum.GetValues<VehicleType>().Select(v => new FilterOptionItem(v.ToString(), v.ToLabel())).ToList(),
        Product = Products.ToDictionary(
            kv => kv.Key.ToString(),
            kv => kv.Value.Select(p => new ProductOptionItem(p.Code, p.Label)).ToList())
    };
}