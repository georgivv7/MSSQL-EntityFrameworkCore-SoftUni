namespace BusTicketSystem.Client.Commands
{
    using System;
    public class ExitCommand 
    {
        public static void Execute()
        {
            Environment.Exit(0);
        }
    }
}
