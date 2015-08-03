using ModbusTools.SlaveExplorer.Model;

namespace ModbusTools.SlaveExplorer.Interfaces
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
