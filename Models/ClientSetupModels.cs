namespace MauiHybridApp.Models;

/// <summary>
/// Client setup configuration model
/// </summary>
public class ClientSetupModel
{
    public long ClientId { get; set; }
    public string ClientCode { get; set; } = string.Empty;
    public string APILink { get; set; } = string.Empty;
    public string Passkey { get; set; } = string.Empty;
    public string? LoginScreenImage { get; set; }
    public string? HomeScreenImage { get; set; }
    public string? LoginScreenImageType { get; set; }
    public string? HomeScreenImageType { get; set; }
    public string? LogoImage { get; set; }
    public string? LogoImageType { get; set; }
    public long? ThemeConfigId { get; set; }
    public string? BrandingImage { get; set; }
    public string? BrandingImageType { get; set; }
    public string? HomePageImage { get; set; }
    public string? HomePageImageType { get; set; }
}

/// <summary>
/// Client setup request
/// </summary>
public class ClientSetupRequest
{
    public string ClientCode { get; set; } = string.Empty;
    public string PassKey { get; set; } = string.Empty;
}

/// <summary>
/// Client setup response
/// </summary>
public class ClientSetupResponse
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public ClientSetupData? ClientSetup { get; set; }
}

public class ClientSetupData
{
    public long ClientSetupId { get; set; }
    public string ClientCode { get; set; } = string.Empty;
    public string ApiUrl { get; set; } = string.Empty;
    public string PassKey { get; set; } = string.Empty;
    public string? LoginScreenImage { get; set; }
    public string? HomeScreenImage { get; set; }
    public string? LoginScreenImageType { get; set; }
    public string? HomeScreenImageType { get; set; }
    public string? LogoImage { get; set; }
    public string? LogoImageType { get; set; }
    public long? ThemeConfigId { get; set; }
    public string? BrandingImage { get; set; }
    public string? BrandingImageType { get; set; }
    public string? HomePageImage { get; set; }
    public string? HomePageImageType { get; set; }
}
