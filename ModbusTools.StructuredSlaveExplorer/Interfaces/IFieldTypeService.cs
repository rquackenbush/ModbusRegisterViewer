using ModbusTools.StructuredSlaveExplorer.Model;

namespace ModbusTools.StructuredSlaveExplorer.Interfaces
{
    public interface IFieldTypeService
    {
        FieldType FieldType { get; }

        IRuntimeField CreateRuntimeField(FieldModel fieldModel);

        int GetSize(FieldModel fieldModel);

        bool SupportsOptions { get; }

        FieldOptionModel[] EditOptions(FieldOptionModel[] Options);
    }
}
