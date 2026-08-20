# Asynchronous Web Page download

## Exercise 1

Create a program to download multiple web pages asynchronously. 


## Implementation

I decided to simulate a potential real world case scenario with a producer (Web.Poc.WorkerService.Producer) and a consumer (Web.Poc.WorkerService.Producer)


### Messaging

For the sake of semplicty I used `Redis Streams` with `Consumer Groups`, but in a real world scenario we should use RabbitMQ, Kafka, Azure Event Hub...

### Producer

- I simulated a case where a list of urls from csv files, in `Web.Poc.WorkerService.Producer\Assets\UrlLists\` 
([csv source](https://github.com/citizenlab/test-lists)), are sent from the `Producer`.  
- The csv file can be configured in `Web.Poc.WorkerService.Producer\appsettings.json` in the `UrlFileName` property

### Consumers

#### UrlConsumerWorker

- It reads from `Redis Streams` the urls sent from the `Producer` with `urls:new` key
- If they're valid urls are added to `Redis Streams` with `urls:pending` key otherwise with `urls:invalid` key

### UrlMonitorWorker

- It reads from `Redis Streams` the urls with `urls:pending` key
- If the `_channel` has capacity are added to it, the channel works as a queue system for the conncurrent downloads, he type of channel created is `Bounded`, it means has Capacity Limit, 
the amoun can be configured in `Web.Poc.WorkerService.Consumer\appsettings.json` in the `QueueCapacity` property

#### UrlDownloaderWorker

- Reads from `_channel`, and if there is any url, it's added to `DownloadUrlAsync`, 
tasks are executed in parallel and the urls are downloaded in the `Web.Poc.WorkerService.Consumer/Downloads` folder

###  Web.Poc.Persistence

Is currently empty but should be use for a potential db, for example EF with sql server for download attempts storing (url, attmepts, status, success, output path, etc) 

## Build And Run

1. Open the CLI, go to the src directory and run `docker compose up`
2. Check `Producer` port, e.g. `55119` and open the browser to `http://localhost:55119/swagger/index.html`
3. Run `/api/publish` endpoint to run the simulation