namespace MData
{
    /// <summary>
    /// Specifies how a command string is interpreted.
    /// </summary>
    public enum CommandType
    {
        /// <summary>
        /// A SQL text command. (Default.)
        /// </summary>
        Text = System.Data.CommandType.Text,

        /// <summary>
        /// The name of a stored procedure.
        /// </summary>
        StoredProcedure = System.Data.CommandType.StoredProcedure,

        /// <summary>
        /// The name of a table.
        /// </summary>
        TableDirect = System.Data.CommandType.TableDirect
    }
}