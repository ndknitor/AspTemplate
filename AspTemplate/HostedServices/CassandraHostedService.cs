
// using Cassandra;

// public class CassandraHostedService : IHostedService
// {
//     private readonly Cassandra.ISession _session;
//     private readonly CancellationTokenSource _cancellationTokenSource;
//     private Task _eventProcessingTask;

//     public CassandraHostedService(string contactPoint, int port, string keyspace)
//     {
//         _session = Cluster.Builder()
//             .AddContactPoint(contactPoint)
//             .WithPort(port)
//             .Build()
//             .Connect(keyspace);

//         _cancellationTokenSource = new CancellationTokenSource();
//     }

//     public Task StartAsync(CancellationToken cancellationToken)
//     {
//         _eventProcessingTask = Task.Run(async () =>
//         {
//             var cdcQuery = "LISTEN TO cdc_table"; // Replace with your CDC table name
//             var subscribeStatement = new SimpleStatement(cdcQuery);

//             var cdcResultSet = await _session.ExecuteAsync(subscribeStatement, "select * from Product");

//             foreach (var cdcEvent in cdcResultSet)
//             {
//                 // Process the CDC event here
//                 // Example: Console.WriteLine($"CDC Event: {cdcEvent["id"]}, {cdcEvent["data"]}");
//             }
//         }, _cancellationTokenSource.Token);

//         return Task.CompletedTask;
//     }

//     public Task StopAsync(CancellationToken cancellationToken)
//     {
//         _cancellationTokenSource.Cancel();
//         return _eventProcessingTask;
//     }
// }
