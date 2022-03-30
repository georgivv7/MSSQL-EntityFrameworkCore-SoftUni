namespace BusTicketSystem.Client.Core
{
    using Commands;
    using System;
    using System.Linq;
    public class CommandDispatcher
    {
        public string DispatchCommand(string[] commandParameters)
        {
            string commandName = commandParameters[0];
            commandParameters = commandParameters.Skip(1).ToArray();

            string result = null;

            switch (commandName)
            {
                case "print-info":
                    int busStationId = int.Parse(commandParameters[0]);
                    result = PrintInfoCommand.Execute(busStationId);
                    break;
                case "buy-ticket":
                    result = BuyTicketCommand.Execute(commandParameters); 
                    break;
                case "publish-review":
                    result = PublishReviewCommand.Execute(commandParameters); 
                    break;
                case "print-reviews":
                    int busCompanyId = int.Parse(commandParameters[0]);
                    result = PrintReviewsCommand.Execute(busCompanyId);
                    break;
                case "exit":
                    ExitCommand.Execute();
                    break;
                default:
                    throw new InvalidOperationException($"Command {commandName} not valid!");
            }

            return result;
        }
    }
}
