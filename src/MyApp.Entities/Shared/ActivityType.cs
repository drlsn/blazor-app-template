using Dunet;

namespace MyApp.Entities.Shared;

[Union]
public partial record ActivityType
{
    partial record Session;
    partial record Rest;
}