using Riok.Mapperly.Abstractions;
using SACA.Entities;
using SACA.Entities.Identity;
using SACA.Entities.Requests;
using SACA.Entities.Responses;

namespace SACA;

[Mapper(PropertyNameMappingStrategy = PropertyNameMappingStrategy.CaseInsensitive)]
public partial class MapperlyMapper
{
    public partial Image MapToImage(Image image);
    public partial Image MapToImage(ImageRequest imageRequest);
    public partial ImageRequest MapToImageRequest(Image image);
    public partial Image MapToImage(ImageResponse imageResponse);
    public partial ImageResponse MapToImageResponse(Image image);
    public partial ApplicationUser MapToApplicationUser(UserResponse userResponse);
    public partial UserResponse MapToUserResponse(ApplicationUser applicationUser);
    public partial ICollection<UserResponse> MapToUsersResponse(ICollection<ApplicationUser> applicationUser);
    public partial UserResponse MapToUserResponse(SignInRequest signInRequest);
    public partial SignInRequest MapToSignInRequest(UserResponse userResponse);
}