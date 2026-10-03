using Alexapps.SkinCare.Samples;
using Xunit;

namespace Alexapps.SkinCare.EntityFrameworkCore.Applications;

[Collection(SkinCareTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<SkinCareEntityFrameworkCoreTestModule>
{

}



