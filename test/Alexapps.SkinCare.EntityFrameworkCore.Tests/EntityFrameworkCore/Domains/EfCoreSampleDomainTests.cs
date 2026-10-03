using Alexapps.SkinCare.Samples;
using Xunit;

namespace Alexapps.SkinCare.EntityFrameworkCore.Domains;

[Collection(SkinCareTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<SkinCareEntityFrameworkCoreTestModule>
{

}



