# Different Observability POCs for .NET

Each POC is hosted in a different branch.

The master branch is focusing on covering all three pillars (logs, traces and metrics) by using fluentbit (logs) and OpenTelemetry (traces and metrics) to consolidate all in the same observability platform (OpenSearch).

## What we have so far (branches)

### logs-fluentbit-opensearch

Sending logs to OpenSearch through fluentbit, the logs are captured from the .NET process stdout. The logs are formatted in JSON using the serilog library with the proper sink. Using serilog allows to add attributes to the logs in many ways by using enrichers (adding trace information for example).

### logs-fluentd-dataprepper-opensearch

Same as `logs-fluentbit-opensearch`, but we use fluentd instead of fluentbit and using data prepper to format the logs so they can be properly ingested by OpenSearch.

### logs-fluentd-opensearch

Same as `logs-fluentd-dataprepper-opensearch`, but we remove data prepper and add configuration to the fluentd collector to format the logs so they can be ingested by OpenSearch.

## Prerequisite

- Docker

## How to use

Checkout the branch you want to use and run the following commands:

```shell
cd docker
docker-compose up -d --build 
```

And to shut it down:

```shell
cd docker
docker-compose down -v
```
