namespace ModbusTools.SlaveSimulator.Model
{
    internal enum SlaveSimulatorState
    {
        /// <summary>
        /// Nothing is happening right now
        /// </summary>
        Idle,

        /// <summary>
        /// We're listening to our 
        /// </summary>
        ListeningToRequest,

        /// <summary>
        /// We're listening to another slave's 
        /// </summary>
        ListeningToOtherSlave       
    }
}
