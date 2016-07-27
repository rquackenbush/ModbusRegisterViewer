namespace ModbusTools.Scanner.ViewModel
{
    public class ResultViewModel
    {
        public bool WasFound { get; set; } 

        public byte SlaveAddress { get; set; }

        public string Reason { get; set; }
    }
}