# Asynchronous Web Page download

## Exercise 1

Create a program to download multiple web pages asynchronously. 

## Implementation

I decided to simulate a potential real world case scenario with a producer and a consumer

### Messaging

For the sake of semplicty I used `SignalR`, but in a real scenario we should use RabbitMq, Kafka...

### Producer

- I simulated a case where 100/1000 urls per time are sent from the producer, the amount can be set in `Web.Poc.WorkerService.Producer\Helpers\UrlsHelper.cs`
- The urls can be generate from a `Faker` or from csv files in `Web.Poc.WorkerService.Producer\Assets\UrlLists\` ([csv source](https://github.com/citizenlab/test-lists)) this can be choosed in `UrlProducerWorker/TrySendUrlsAsync()`

### Consumers

#### UrlConsumerWorker

- It reads from `SignalR Hub` the urls sent from the `Producer` and add them to `_urlQueue` typeof `IItemQueue` (that under the hood use a `Channel`, choosed over `concurrentqueue` and `blockingcollection` for perfomance reasons)
- The type of channel created is `Unbounded`, it means has no Capacity Limit, in a real world scenario we should use a db or a cache (eg. redis) where we update the status (success, pending, failed) of every url
- `OnAddUrls` is called everytime a new list of urls arrive, it checks if they're valid urls and the're added to _urlQueue in background without blocking the thread

#### UrlDownloaderWorker

- It reads from `_urlQueue` and try to add to `_urlDownloadTaskQueue` typeof `ITaskQueue` (that under the hood use a `Channel`, choosed over `concurrentqueue` and `blockingcollection` for perfomance reasons)
- The type of channel created is `Bounded`, it means has Capacity Limit, it defaults to `100` but can be set in the `consumer appsettings`
- If there `_urlDownloadTaskQueue` has capacity, a new url from `_urlQueue` is added, and `DownloadUrlAsync` is added to the queue
- All the `DownloadUrlAsync` tasks are exectude in parallel and the page downloaded in the `consumer/Downloads` folder

## TODO

- Make sure eeverything works end to end without mocks, randoms, fake delayes, etc.
- Add simple EF + SQLite for download attempts storing (url, attmepts, status, success, output path, etc) or even Redis that can be used for data persistency and replace SignalR by pub/sub (Redis Streams)
- Add retries with exponential backoff for unsuccessful attempts as it's really beneficial for HTTP-focused projects.
- Docker compose to run workers and Redis
- Reorganize and cleanup files and projects
