using UnitsNet.Metadata.Annotations;
using UnitsNet.Units;

namespace UnitsNet.Metadata.Tests.TestData;

public interface IHardDrive
{
    string Manufacturer { get; set; }
    string Model { get; set; }
    string SerialNumber { get; set; }

    [Quantity(InformationUnit.Gigabyte)]
    double Capacity { get; set; }

    [Quantity(InformationUnit.Gigabyte)]
    double FreeSpace { get; set; }
}
